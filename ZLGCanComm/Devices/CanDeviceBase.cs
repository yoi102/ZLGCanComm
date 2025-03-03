using ZLGCanComm.Api;
using ZLGCanComm.Enums;
using ZLGCanComm.Extensions;
using ZLGCanComm.Interfaces;
using ZLGCanComm.Records;

namespace ZLGCanComm.Devices;

public abstract class CanDeviceBase : ICanDevice
{
    protected bool isDisposed;
    protected bool isOpened;
    private static readonly SynchronizationContext? _syncContext = SynchronizationContext.Current;
    private CancellationTokenSource? _errorMonitoringCts;

    public CanDeviceBase(uint deviceIndex, uint canIndex)
    {
        this.DeviceIndex = deviceIndex;
        this.CanIndex = canIndex;
    }

    /// <summary>
    /// 设备发生错误时将触发此事件。
    /// <para>连接设备成功后将长轮询读取设备错误信息。</para>
    /// <para>连接时会读取一次ErrorInfo，长轮询读取的ErrorInfo不等于连接时的ErrorInfo且错误码不为0x00000100时，将触发此事件。</para>
    /// <para>发生错误不会清空所有的订阅</para>
    public event Action<ErrorInfo>? ErrorOccurred;

    /// <summary>
    /// 连接意外丢失，设备意外丢失。
    /// <para>连接设备成功后将长轮询读取设备错误信息，读取错误信息失败或者错误码为0x00001000时，将视为设备丢失。</para>
    /// <para>设备丢失不会清空所有的订阅</para>
    /// </summary>
    public event Action<ICanDevice>? LostConnection;

    public uint CanIndex { get; }

    public uint DeviceIndex { get; }

    /// <summary>
    /// 设备连接类型
    /// </summary>
    public DeviceType DeviceType => (DeviceType)UintDeviceType;

    /// <summary>
    /// 长轮询读取错误信息时，时间间隔,单位毫秒，默认设置为500。
    /// <para>连接设备后将长轮询读取设备错误信息</para>
    /// </summary>
    public int ErrorPollingInterval { get; set; } = 500;

    /// <summary>
    /// 是否已经连接
    /// </summary>
    public bool IsConnected { get; protected set; }

    /// <summary>
    /// 上次的错误状态
    /// </summary>
    public ErrorInfo? LastErrorInfo { get; protected set; }

    public abstract uint UintDeviceType { get; }

    public virtual void Connect()
    {
        this.TryReadErrorInfo(out var errorInfo);
        if (!ZLGApiProvider.Instance.StartCAN(UintDeviceType, DeviceIndex, CanIndex))
            throw new CanDeviceOperationException();
        LastErrorInfo = errorInfo;
        IsConnected = true;

        _errorMonitoringCts = new CancellationTokenSource();
        Task.Run(() => MonitorErrorsAsync(_errorMonitoringCts.Token));
    }

    /// <summary>
    /// 关闭设备,断开连接。
    /// <para>将清空所有的订阅</para>
    /// </summary>
    public virtual void Disconnect()
    {
        ZLGApiProvider.Instance.CloseDevice(UintDeviceType, DeviceIndex);
        this.Unsubscribe();
        _errorMonitoringCts?.Cancel();
        isOpened = false;
        IsConnected = false;
    }

    /// <summary>
    /// Dispose 当前实例
    /// <para>连接将会断开，当前设备的订阅将会全部清除</para>
    /// <para>Dispose之后将不允许任何操作</para>
    /// </summary>
    public void Dispose()
    {
        if (isDisposed)
            return;
        Disconnect();
        isDisposed = true;
    }

    /// <summary>
    /// 获取接收缓冲区中，接收到但尚未被读取的帧数量
    /// </summary>
    /// <returns>接收到但尚未被读取的帧数量</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public virtual uint GetCanReceiveCount()
    {
        if (isDisposed)
            throw new InvalidOperationException();
        if (!IsConnected)
            throw new InvalidOperationException();
        return ZLGApiProvider.Instance.GetReceiveNum(UintDeviceType, DeviceIndex, CanIndex);
    }

    /// <summary>
    /// 获取设备信息
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">该实例被 Dispose后，或处于未连接状态时，调用此方法将抛出此异常</exception>
    /// <exception cref="CanDeviceOperationException">若ZLGCan的Api返回值为0时，将抛出此异常</exception>
    public virtual BoardInfo ReadBoardInfo()
    {
        if (isDisposed)
            throw new InvalidOperationException();

        if (!ZLGApiProvider.Instance.ReadBoardInfo(UintDeviceType, DeviceIndex, out var info))
        {
            throw new CanDeviceOperationException();
        }

        return info;
    }

    /// <summary>
    /// 获取ZLGCan控制器的最近一次错误信息。
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">该实例被 Dispose后，或处于未连接状态时，调用此方法将抛出此异常</exception>
    /// <exception cref="CanDeviceOperationException">若ZLGCan的Api返回值为0时，将抛出此异常</exception>
    public virtual ErrorInfo ReadErrorInfo()
    {
        if (isDisposed)
            throw new InvalidOperationException();

        if (!ZLGApiProvider.Instance.ReadErrInfo(UintDeviceType, DeviceIndex, CanIndex, out var errorInfo))
        {
            throw new CanDeviceOperationException();
        }

        return errorInfo;
    }

    /// <summary>
    /// 读取Can控制器信息
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">该实例被 Dispose后，或处于未连接状态时，调用此方法将抛出此异常</exception>
    /// <exception cref="CanDeviceOperationException">若ZLGCan的Api返回值为0时，将抛出此异常</exception>
    public virtual CanControllerStatus ReadStatus()
    {
        if (isDisposed)
            throw new InvalidOperationException();

        if (!ZLGApiProvider.Instance.ReadCANStatus(UintDeviceType, DeviceIndex, CanIndex, out var status))
        {
            throw new CanDeviceOperationException();
        }

        return status;
    }

    /// <summary>
    /// 接收缓冲区中读取数据。
    /// </summary>
    /// <param name="waitTime">缓冲区无数据，函数阻塞等待时间，以毫秒为单位。若为-1 则表示无超时，一直等待。</param>
    /// <returns>反回所读取到的数据，当控制器没有数据可读时、 CanObject[0]</returns>
    /// <exception cref="InvalidOperationException">该实例被 Dispose后，或处于未连接状态时，调用此方法将抛出此异常</exception>
    public virtual CanObject[] Receive(int waitTime = 0)
    {
        if (isDisposed)
            throw new InvalidOperationException();
        if (!IsConnected)
            throw new InvalidOperationException();

        var totalToReceive = ZLGApiProvider.Instance.GetReceiveNum(UintDeviceType, DeviceIndex, CanIndex);
        if (totalToReceive == 0)
        {
            return []; // 没有数据，直接返回空数组
        }
        List<CanObject> canObjects = new();

        //强制读取完存在的帧数据
        while (totalToReceive > 0)
        {
            var newCanObjects = Receive(totalToReceive, waitTime); // 继续读取剩余数据

            if (newCanObjects.Length == 0)
                break; // 避免死循环，如果 `Receive` 没有数据可读，退出循环

            canObjects.AddRange(newCanObjects);

            if (newCanObjects.Length < totalToReceive)
            {
                //没有读取完就再更新下未读帧数据数目
                totalToReceive = ZLGApiProvider.Instance.GetReceiveNum(UintDeviceType, DeviceIndex, CanIndex);
            }
            else
            {
                break;//读取完就退出
            }
        }
        return canObjects.ToArray();
    }

    /// <summary>
    /// 复位，将会断开连接，所有订阅将会被取消
    /// </summary>
    public virtual void Reset()
    {
        if (isDisposed)
            throw new InvalidOperationException();
        ZLGApiProvider.Instance.ResetCAN(UintDeviceType, DeviceIndex, CanIndex);
        this.Unsubscribe();
        _errorMonitoringCts?.Cancel();
        IsConnected = false;
    }

    /// <summary>
    /// 向ZLGCan控制器发送数据。返回值为实际发送成功的帧数。
    /// </summary>
    /// <param name="canObject"></param>
    /// <returns>返回实际发送成功的帧数。</returns>
    /// <exception cref="InvalidOperationException">该实例被 Dispose后，或处于未连接状态时，调用此方法将抛出此异常</exception>
    public virtual uint Transmit(params CanObject[] canObject)
    {
        if (canObject.Length == 0)
            throw new InvalidOperationException();
        if (isDisposed)
            throw new InvalidOperationException();
        if (!IsConnected)
            throw new InvalidOperationException();

        var sendCount = ZLGApiProvider.Instance.Transmit(UintDeviceType, DeviceIndex, CanIndex, canObject);

        return sendCount;
    }

    /// <summary>
    ///  以此配置    //SendType = 0:正常发送
    ///             //RemoteFlag = 0:数据帧
    ///             //ExternFlag = 0:标准帧
    ///  向ZLGCan控制器发送单帧数据
    ///  </summary>
    /// <param name="id">帧 ID。32 位变量，数据格式为靠右对齐</param>
    /// <param name="data">CAN 帧的数据，最多 8 字节 ；长度可小于8</param>
    /// <returns>发送的帧数与实际发送的帧数是否相等</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public virtual bool Transmit(uint id, byte[] data)
    {
        if (isDisposed)
            throw new InvalidOperationException();
        if (!IsConnected)
            throw new InvalidOperationException();

        var canObject = new CanObject
        {
            Id = id,
            SendType = 0,            //0:正常发送 1:单次发送 2:自发自收 3:单次自发自收
            RemoteFlag = 0,          //0:数据帧 1: 远程帧
            ExternFlag = 0,          //0:标准帧 1:扩展帧
            Data = data
        };

        return Transmit(canObject) == 1;
    }

    private async Task MonitorErrorsAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                var errorInfo = this.ReadErrorInfo();
                if (errorInfo == LastErrorInfo)
                    continue;

                LastErrorInfo = errorInfo;
                if (errorInfo.ErrorCode == 0x00000100)//设备已经打开
                {
                    continue;
                }
                OnErrorOccurred(errorInfo);

                if (errorInfo.ErrorCode == 0x00001000)//此设备不存在
                {
                    OnLostConnection();
                }
            }
            catch (CanDeviceOperationException)
            {
                OnLostConnection();
            }
            catch (InvalidOperationException)
            {
                break;
            }

            await Task.Delay(ErrorPollingInterval, token); // 读取 `ErrorPollingInterval` 作为轮询间隔
        }
    }

    private void OnErrorOccurred(ErrorInfo errorInfo)
    {
        if (_syncContext != null)
        {
            _syncContext.Post(_ => ErrorOccurred?.Invoke(errorInfo), null);
        }
        else
        {
            ErrorOccurred?.Invoke(errorInfo);
        }
    }

    private void OnLostConnection()
    {
        IsConnected = false;
        if (_syncContext != null)
        {
            _syncContext.Post(_ => LostConnection?.Invoke(this), null);
        }
        else
        {
            LostConnection?.Invoke(this);
        }
    }

    /// <summary>
    /// 接收缓冲区中读取数据。
    /// 以获取Can信息帧
    /// </summary>
    /// <param name="length">用来接收的帧结构体数组的长度（本次接收的最大帧数，实际返回值小于等于这个值）。</param>
    /// <param name="waitTime">缓冲区无数据，函数阻塞等待时间，以毫秒为单位。若为-1 则表示无超时，一直等待。</param>
    /// <returns>当控制器没有数据可读时、将返回Empty</returns>
    /// <exception cref="InvalidOperationException">该实例被 Dispose后，或处于未连接状态时，调用此方法将抛出此异常</exception>
    private CanObject[] Receive(uint length = 1, int waitTime = 0)
    {
        if (isDisposed)
            throw new InvalidOperationException();
        if (!IsConnected)
            throw new InvalidOperationException();

        var canObjects = ZLGApiProvider.Instance.Receive(UintDeviceType, DeviceIndex, CanIndex, length, waitTime);

        return canObjects;
    }
}