using System.Runtime.InteropServices;
using ZLGCanComm;
using ZLGCanComm.Enums;
using ZLGCanComm.Interfaces;
using ZLGCanComm.Structs;

namespace ZLGCanComm.Devices;

public abstract class BaseDevice : ICanDevice
{
    protected readonly uint canIndex;

    protected uint deviceIndex;

    protected bool disposed;

    protected nint ptr;

    public BaseDevice(uint canIndex)
    {
        this.canIndex = canIndex;
    }

    public event Action<ICanDevice>? ConnectionLost;

    public DeviceType DeviceType => (DeviceType)UintDeviceType;
    public bool IsConnected { get; protected set; }
    public abstract uint UintDeviceType { get; }

    public virtual CanControllerStatus ReadCanControllerStatus()
    {
        if (disposed)
            throw new InvalidOperationException();
        if (!IsConnected)
            throw new InvalidOperationException();
        var status = new CanControllerStatus();

        if (ZLGApi.VCI_ReadCANStatus(UintDeviceType, deviceIndex, canIndex, ref status) != (uint)OperationStatus.Success)
        {
            StopListen();
            throw new CanDeviceOperationException();
        }

        return status;
    }

    public virtual ErrorInfo ReadErrorInfo()
    {
        if (disposed)
            throw new InvalidOperationException();
        if (!IsConnected)
            throw new InvalidOperationException();
        var errorInfo = new ErrorInfo();

        if (ZLGApi.VCI_ReadErrInfo(UintDeviceType, deviceIndex, canIndex, ref errorInfo) != (uint)OperationStatus.Success)
        {
            StopListen();
            throw new CanDeviceOperationException();
        }
        return errorInfo;
    }

    public virtual CanObject ReadMessage(uint length = 1, int waitTime = 0)
    {
        if (disposed)
            throw new InvalidOperationException();
        if (!IsConnected)
            throw new InvalidOperationException();

        if (ZLGApi.VCI_GetReceiveNum(UintDeviceType, deviceIndex, canIndex) != (uint)OperationStatus.Success)
        {
            StopListen();
            throw new CanDeviceOperationException();
        }
        if (ZLGApi.VCI_Receive(UintDeviceType, deviceIndex, canIndex, ptr, length, waitTime) != (uint)OperationStatus.Success)
        {
            StopListen();
            throw new CanDeviceOperationException();
        }
        Marshal.WriteByte(ptr, 0x00);

        var received = Marshal.PtrToStructure((nint)(uint)ptr, typeof(CanObject));
        if (received is not CanObject canObject)
        {
            StopListen();
            throw new CanDeviceOperationException();
        }
        return canObject;
    }

    public abstract bool TryConnect();

    public virtual bool TryReadCanControllerStatus(out CanControllerStatus status)
    {
        status = new CanControllerStatus();
        try
        {
            status = ReadCanControllerStatus();
        }
        catch (InvalidOperationException)
        {
            return false;
        }
        catch (CanDeviceOperationException)
        {
            return false;
        }
        return true;
    }

    public virtual bool TryReadErrorInfo(out ErrorInfo errorInfo)
    {
        errorInfo = new ErrorInfo();
        try
        {
            errorInfo = ReadErrorInfo();
        }
        catch (InvalidOperationException)
        {
            return false;
        }
        catch (CanDeviceOperationException)
        {
            return false;
        }
        return true;
    }

    public virtual bool TryReadMessage(out CanObject canObject, uint length = 1, int waitTime = 0)
    {
        canObject = new CanObject();

        try
        {
            canObject = ReadMessage();
        }
        catch (InvalidOperationException)
        {
            return false;
        }
        catch (CanDeviceOperationException)
        {
            return false;
        }
        return true;
    }

    public virtual bool TryWriteMessage(ref CanObject canObject, uint length = 1)
    {
        try
        {
            canObject = WriteMessage(canObject);
        }
        catch (InvalidOperationException)
        {
            return false;
        }
        catch (CanDeviceOperationException)
        {
            return false;
        }
        return true;
    }

    public virtual bool TryWriteMessage(uint id, byte[] message, uint length = 1)
    {
        try
        {
            WriteMessage(id, message, length);
        }
        catch (InvalidOperationException)
        {
            return false;
        }
        catch (CanDeviceOperationException)
        {
            return false;
        }
        return true;
    }

    public virtual CanObject WriteMessage(CanObject canObject, uint length = 1)
    {
        if (disposed)
            throw new InvalidOperationException();
        if (!IsConnected)
            throw new InvalidOperationException();
        var result = canObject;
        if (ZLGApi.VCI_Transmit(UintDeviceType, deviceIndex, canIndex, ref result, length) != (uint)OperationStatus.Success)
        {
            StopListen();
            throw new CanDeviceOperationException();
        }
        return result;
    }

    public virtual CanObject WriteMessage(uint id, byte[] message, uint length = 1)
    {
        if (disposed)
            throw new InvalidOperationException();
        if (!IsConnected)
            throw new InvalidOperationException();

        var canObject = new CanObject
        {
            Id = ZLGApi.NODE | id,
            SendType = 0,            //0:正常发送 1:单次发送 2:自发自收 3:单次自发自收
            RemoteFlag = 0,          //0:数据帧 1: 远程帧
            ExternFlag = 0,          //0:标准帧 1:扩展帧

            DataLength = (byte)message.Length,
            Data = message
        };

        return WriteMessage(canObject, length);
    }

    protected virtual void StopListen()
    {
        ListenerService.StopListen(this);
    }

    internal virtual void OnConnectionLost()
    {
        IsConnected = false;
        ConnectionLost?.Invoke(this);
    }

    /// <summary>
    /// 注册监听设备。
    /// <para>会先读取一次并且调用一次 <paramref name="onChange"/>。</para>
    /// <para>之后当读取的信息发生变化时，将触发 <paramref name="onChange"/>。</para>
    /// <para>不允许多次注册。当当前实例和入参的 <paramref name="pollingTimeout"/>，<paramref name="length"/>，<paramref name="waitTime"/> 一致时，视为同一个监听者，第二次之后的注册将不会有任何动作。</para>
    /// <para>允许注册多个 <paramref name="onChange"/> 回调。</para>
    /// </summary>
    /// <param name="onChange">当去读的值发生变化时将触发</param>
    /// <param name="pollingTimeout">长轮询的Delay时长、单位毫秒,默认为一百毫秒</param>
    /// <param name="length">读取设备用的 api的入参</param>
    /// <param name="waitTime">读取设备用的 api的入参</param>
    public virtual void RegisterListener(Action<CanObject> onChange, int pollingTimeout = 100, uint length = 1, int waitTime = 0)
    {
        var record = new ListenerObjectRecord()
        {
            Device = this,
            PollingTimeout = pollingTimeout,
            Length = length,
            WaitTime = waitTime
        };
        ListenerService.RegisterListener(record, onChange);
    }

    /// <summary>
    /// 取消监听设备。
    /// <para>当当前实例和入参的 <paramref name="pollingTimeout"/>，<paramref name="length"/>，<paramref name="waitTime"/> 一致时，视为同一个监听者</para>
    /// </summary>
    /// <param name="onChange">当去读的值发生变化时将触发</param>
    /// <param name="pollingTimeout">长轮询的Delay时长、单位毫秒,默认为一百毫秒</param>
    /// <param name="length">读取设备用的 api的入参</param>
    /// <param name="waitTime">读取设备用的 api的入参</param>
    public virtual void UnregisterListener(Action<CanObject> onChange, int pollingTimeout = 100, uint length = 1, int waitTime = 0)
    {
        var record = new ListenerObjectRecord()
        {
            Device = this,
            PollingTimeout = pollingTimeout,
            Length = length,
            WaitTime = waitTime
        };
        ListenerService.UnregisterListener(record, onChange);
    }

    /// <summary>
    /// Dispose 当前实例
    /// <para>连接将会断开，当前设备的监听将会全部清除</para>
    /// <para>Dispose之后将不允许任何操作</para>
    /// </summary>
    public void Dispose()
    {
        if (!IsConnected)
            return;
        ListenerService.StopListen(this);
        Marshal.FreeHGlobal(ptr);
        ZLGApi.VCI_CloseDevice(UintDeviceType, deviceIndex);
        var keyPair = DeviceRegistry.DeviceTypeIndexTracker.Single(x => x.Key == DeviceType && x.Value == deviceIndex);
        DeviceRegistry.DeviceTypeIndexTracker.Remove(keyPair);
        IsConnected = false;
        disposed = true;
    }
}