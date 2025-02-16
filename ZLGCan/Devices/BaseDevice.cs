using System.Runtime.InteropServices;
using ZLGCan.Enums;
using ZLGCan.Interfaces;
using ZLGCan.Structs;

namespace ZLGCan.Devices;

public abstract class BaseDevice : ICanDevice
{
    protected readonly uint canIndex;

    protected uint deviceIndex;

    protected bool disposed;

    protected IntPtr ptr;

    public BaseDevice(uint canIndex)
    {
        this.canIndex = canIndex;
    }

    public event Action<ICanDevice>? ConnectionLost;

    public DeviceType DeviceType => (DeviceType)UintDeviceType;
    public bool IsConnected { get; protected set; }
    public abstract uint UintDeviceType { get; }

    public int PollingTimeout { get; set; } = 100;

    public virtual CanControllerStatus ReadCanControllerStatus()
    {
        if (disposed)
            throw new InvalidOperationException();
        if (!IsConnected)
            throw new InvalidOperationException();
        var status = new CanControllerStatus();

        if (ZLGApi.VCI_ReadCANStatus(UintDeviceType, deviceIndex, canIndex, ref status) != (uint)OperationStatus.Success)
            throw new CanDeviceOperationException();

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
            throw new CanDeviceOperationException();

        return errorInfo;
    }

    public virtual CanObject ReadMessage(uint length = 1, int waitTime = 0)
    {
        if (disposed)
            throw new InvalidOperationException();
        if (!IsConnected)
            throw new InvalidOperationException();

        if (ZLGApi.VCI_GetReceiveNum(UintDeviceType, deviceIndex, canIndex) != (uint)OperationStatus.Success)
            throw new CanDeviceOperationException();

        if (ZLGApi.VCI_Receive(UintDeviceType, deviceIndex, canIndex, ptr, length, waitTime) != (uint)OperationStatus.Success)
            throw new CanDeviceOperationException();

        Marshal.WriteByte(ptr, 0x00);

        var received = Marshal.PtrToStructure((IntPtr)((uint)ptr), typeof(CanObject));
        if (received is not CanObject canObject)
            throw new CanDeviceOperationException();

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
            throw new CanDeviceOperationException();

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

    internal virtual void OnConnectionLost()
    {
        IsConnected = false;
        ConnectionLost?.Invoke(this);
    }

    public virtual void RegisterListener(Action<CanObject> onChange, uint length = 1, int waitTime = 0)
    {
        var record = new ListenerObjectRecord()
        {
            Device = this,
            Length = length,
            WaitTime = waitTime
        };
        ListenerService.RegisterListener(record, onChange);
    }

    public virtual void UnregisterListener(Action<CanObject> onChange, uint length = 1, int waitTime = 0)
    {
        var record = new ListenerObjectRecord()
        {
            Device = this,
            Length = length,
            WaitTime = waitTime
        };
        ListenerService.UnregisterListener(record, onChange);
    }

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