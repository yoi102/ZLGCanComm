using System.Runtime.InteropServices;
using ZLGCanComm.Enums;
using ZLGCanComm.Extensions;
using ZLGCanComm.Interfaces;
using ZLGCanComm.Structs;

namespace ZLGCanComm.Devices;

public abstract class BaseDevice : ICanDevice
{
    protected readonly uint canIndex;
    protected uint deviceIndex;
    protected bool disposed;
    protected nint ptr;
    private static readonly object _lock = new();
    private static readonly SynchronizationContext? _syncContext = SynchronizationContext.Current;

    public BaseDevice(uint canIndex)
    {
        this.canIndex = canIndex;
    }

    /// <summary>
    /// <para>当设备意外断开时，将触发此事件。</para>
    /// <para>注意：此事件无法实时反映Can的是否断开、只有在读写失败后才会触发此事件。</para>
    /// <para>断开后，所有监听内容将被清除，在重新连接时请重新设置监听。</para>
    /// </summary>
    public event Action<ICanDevice>? ConnectionLost;

    /// <summary>
    /// ZLGCAN系列接口卡信息的数据类型
    /// </summary>
    public BoardInfo? BoardInfo { get; private set; }

    /// <summary>
    /// 设备连接类型
    /// </summary>
    public DeviceType DeviceType => (DeviceType)UintDeviceType;

    /// <summary>
    /// 最近一次错误信息
    /// </summary>
    public ErrorInfo? ErrorInfo { get; private set; }

    /// <summary>
    /// 是否已经连接
    /// </summary>
    public bool IsConnected { get; protected set; }

    /// <summary>
    /// 设备连接后，将间隔<see cref="CanPollingDelay"/>毫秒更新状态
    /// </summary>
    public CanControllerStatus? Status { get; private set; }

    public abstract uint UintDeviceType { get; }

    public virtual void Connect()
    {
        ptr = Marshal.AllocHGlobal(Marshal.SizeOf<VCI_CAN_OBJ>());
        this.TryReadErrorInfo(out _);
        this.TryReadBoardInfo(out _);
        this.TryReadStatus(out _);

        if (ZLGApi.VCI_StartCAN(UintDeviceType, deviceIndex, canIndex) == (uint)OperationStatus.Failure)
            throw new CanDeviceOperationException();
        IsConnected = true;
    }

    /// <summary>
    /// Dispose 当前实例
    /// <para>连接将会断开，当前设备的监听将会全部清除</para>
    /// <para>Dispose之后将不允许任何操作</para>
    /// </summary>
    public void Dispose()
    {
        if (disposed)
            return;
        UnregisterAllListener();
        Marshal.FreeHGlobal(ptr);
        ZLGApi.VCI_CloseDevice(UintDeviceType, deviceIndex);
        var keyPair = DeviceRegistry.DeviceTypeIndexTracker.Single(x => x.Key == DeviceType && x.Value == deviceIndex);
        DeviceRegistry.DeviceTypeIndexTracker.Remove(keyPair);
        if (ConnectionLost != null)
        {
            foreach (Delegate d in ConnectionLost.GetInvocationList())
            {
                ConnectionLost -= (Action<ICanDevice>)d;
            }
        }
        IsConnected = false;
        disposed = true;
    }

    /// <summary>
    /// 获取接收缓冲区中，接收到但尚未被读取的帧数量
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public virtual uint GetCanReceiveCount()
    {
        if (disposed)
            throw new InvalidOperationException();
        if (!IsConnected)
            throw new InvalidOperationException();
        return ZLGApi.VCI_GetReceiveNum(UintDeviceType, deviceIndex, canIndex);
    }

    /// <summary>
    /// 获取设备信息
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">该实例被 Dispose后，或处于未连接状态时，调用此方法将抛出此异常</exception>
    /// <exception cref="CanDeviceOperationException">若ZLGCan的Api返回值为0时，将抛出此异常</exception>
    public virtual BoardInfo ReadBoardInfo()
    {
        if (disposed)
            throw new InvalidOperationException();

        var info = new VCI_BOARD_INFO();
        BoardInfo = new();
        if (ZLGApi.VCI_ReadBoardInfo(UintDeviceType, deviceIndex, ref info) == (uint)OperationStatus.Failure)
        {
            throw new CanDeviceOperationException();
        }
        BoardInfo = StructConverter.Converter(info);

        return BoardInfo;
    }

    /// <summary>
    /// 获取ZLGCan控制器的最后一次错误信息。
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">该实例被 Dispose后，或处于未连接状态时，调用此方法将抛出此异常</exception>
    /// <exception cref="CanDeviceOperationException">若ZLGCan的Api返回值为0时，将抛出此异常</exception>
    public virtual ErrorInfo ReadErrorInfo()
    {
        if (disposed)
            throw new InvalidOperationException();
        var errorInfo = new VCI_ERR_INFO();
        ErrorInfo = new ErrorInfo();
        if (ZLGApi.VCI_ReadErrInfo(UintDeviceType, deviceIndex, canIndex, ref errorInfo) == (uint)OperationStatus.Failure)
        {
            throw new CanDeviceOperationException();
        }
        ErrorInfo = StructConverter.Converter(errorInfo);
        return ErrorInfo;
    }

    /// <summary>
    /// 获取ZLGCan控制器接收缓冲区中接收到但尚未被读取的帧数。
    /// 以获取Can信息帧
    /// </summary>
    /// <param name="length"></param>
    /// <param name="waitTime"></param>
    /// <returns>当控制器没有数据可读时、将返回Empty</returns>
    /// <exception cref="InvalidOperationException">该实例被 Dispose后，或处于未连接状态时，调用此方法将抛出此异常</exception>
    /// <exception cref="CanDeviceOperationException">若ZLGCan的Api返回值为0时，将抛出此异常</exception>
    public virtual CanObject? ReadMessage(uint length = 1, int waitTime = 0)
    {
        if (disposed)
            throw new InvalidOperationException();
        if (!IsConnected)
            throw new InvalidOperationException();

        if (ZLGApi.VCI_GetReceiveNum(UintDeviceType, deviceIndex, canIndex) == 0)
        {
            return null;
        }
        return ReadMessageDirect(length, waitTime);
    }

    /// <summary>
    /// 直接获取ZLGCan控制器接收缓冲区中接收到但尚未被读取的帧数。
    /// 以获取Can信息帧
    /// </summary>
    /// <param name="length"></param>
    /// <param name="waitTime"></param>
    /// <returns>当控制器没有数据可读时、将返回Empty</returns>
    /// <exception cref="InvalidOperationException">该实例被 Dispose后，或处于未连接状态时，调用此方法将抛出此异常</exception>
    /// <exception cref="CanDeviceOperationException">若ZLGCan的Api返回值为0时，将抛出此异常</exception>
    public virtual CanObject ReadMessageDirect(uint length = 1, int waitTime = 0)
    {
        if (disposed)
            throw new InvalidOperationException();
        if (!IsConnected)
            throw new InvalidOperationException();

        if (ZLGApi.VCI_Receive(UintDeviceType, deviceIndex, canIndex, ptr, length, waitTime) == (uint)OperationStatus.Failure)
        {
            OnConnectionLost();
            throw new CanDeviceOperationException();
        }
        Marshal.WriteByte(ptr, 0x00);

        var received = Marshal.PtrToStructure((nint)(uint)ptr, typeof(VCI_CAN_OBJ));

        if (received is not VCI_CAN_OBJ oBJ)
        {
            OnConnectionLost();
            throw new CanDeviceOperationException();
        }
        return StructConverter.Converter(oBJ);
    }

    /// <summary>
    /// 读取Can控制器信息
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">该实例被 Dispose后，或处于未连接状态时，调用此方法将抛出此异常</exception>
    /// <exception cref="CanDeviceOperationException">若ZLGCan的Api返回值为0时，将抛出此异常</exception>
    public virtual CanControllerStatus ReadStatus()
    {
        if (disposed)
            throw new InvalidOperationException();

        var status = new VCI_CAN_STATUS();
        Status = new();

        if (ZLGApi.VCI_ReadCANStatus(UintDeviceType, deviceIndex, canIndex, ref status) == (uint)OperationStatus.Failure)
        {
            throw new CanDeviceOperationException();
        }
        Status = StructConverter.Converter(status);
        return Status;
    }

    /// <summary>
    /// 注册监听设备。
    /// <para>会先读取一次并且调用一次 <paramref name="onChange"/>。</para>
    /// <para>之后当读取的信息发生变化时，将触发 <paramref name="onChange"/>。</para>
    /// <para>不允许多次注册。仅当前实例和入参的 <paramref name="pollingTimeout"/>，<paramref name="length"/>，<paramref name="waitTime"/> 一致时，视为同一个监听者</para>
    /// <para>同一个监听者第二次之后的注册将不会有任何动作。</para>
    /// <para>允许注册多个 <paramref name="onChange"/> 回调，同一个监听者同一个回调多次注册时，仅第一次有效。</para>
    /// </summary>
    /// <param name="onChange">当去读的值发生变化时将触发</param>
    /// <param name="pollingTimeout">长轮询的Delay时长、单位毫秒,默认为一百毫秒</param>
    /// <param name="length">读取设备用的 api的入参</param>
    /// <param name="waitTime">读取设备用的 api的入参</param>
    /// <exception cref="InvalidOperationException">该实例被 Dispose后，或处于未连接状态时，调用此方法将抛出此异常</exception>
    /// <exception cref="CanDeviceOperationException">若ZLGCan的Api返回值为0时，将抛出此异常</exception>
    public virtual void RegisterListener(Action<CanObject> onChange, int pollingTimeout = 100, uint length = 1, int waitTime = 0)
    {
        if (disposed)
            throw new InvalidOperationException();
        if (!IsConnected)
            throw new InvalidOperationException();
        var record = new ListenerObjectRecord()
        {
            Device = this,
            PollingTimeout = pollingTimeout,
            Length = length,
            WaitTime = waitTime
        };
        ListenerService.RegisterListener(record, onChange);
    }

    public void UnregisterAllListener()
    {
        ListenerService.StopListenDevice(this);
    }

    /// <summary>
    /// 取消监听设备。
    /// <para>仅当前实例和入参的 <paramref name="pollingTimeout"/>，<paramref name="length"/>，<paramref name="waitTime"/> 一致时，视为同一个监听者</para>
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
    /// 向ZLGCan控制器发送帧数
    /// </summary>
    /// <param name="canObject"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">该实例被 Dispose后，或处于未连接状态时，调用此方法将抛出此异常</exception>
    /// <exception cref="CanDeviceOperationException">若ZLGCan的Api返回值为0时，将抛出此异常</exception>
    public virtual CanObject WriteMessage(CanObject canObject, uint length = 1)
    {
        if (disposed)
            throw new InvalidOperationException();
        if (!IsConnected)
            throw new InvalidOperationException();
        var send = StructConverter.Converter(canObject);
        if (ZLGApi.VCI_Transmit(UintDeviceType, deviceIndex, canIndex, ref send, length) == (uint)OperationStatus.Failure)
        {
            OnConnectionLost();
            throw new CanDeviceOperationException();
        }
        return StructConverter.Converter(send);
    }

    /// <summary>
    ///  以此配置    //SendType = 0:正常发送
    ///             //RemoteFlag = 0:数据帧
    ///             //ExternFlag = 0:标准帧
    ///  向ZLGCan控制器发送帧数
    ///  </summary>
    /// <param name="id"></param>
    /// <param name="message"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
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

    protected virtual void OnConnectionLost()
    {
        lock (_lock)
        {
            if (!IsConnected)
                return;

            IsConnected = false;
            this.TryReadErrorInfo(out _);
            this.TryReadStatus(out _);
            ListenerService.StopListenDevice(this);
            if (_syncContext is null)
            {
                ConnectionLost?.Invoke(this);
            }
            else
            {
                _syncContext?.Post(_ => ConnectionLost?.Invoke(this), null);
            }
        }
    }
}