﻿using ZLGCanComm.Api;
using ZLGCanComm.Enums;
using ZLGCanComm.Extensions;
using ZLGCanComm.Interfaces;
using ZLGCanComm.ListenerManager;
using ZLGCanComm.Records;

namespace ZLGCanComm.Devices;

public abstract class CanDeviceBase : ICanDevice
{
    protected bool isDisposed;

    public CanDeviceBase(uint deviceIndex, uint canIndex)
    {
        this.DeviceIndex = deviceIndex;
        this.CanIndex = canIndex;
    }

    /// <summary>
    /// ZLGCAN系列接口卡信息的数据类型
    /// </summary>
    public BoardInfo? BoardInfo { get; private set; }

    public uint CanIndex { get; }

    public uint DeviceIndex { get; }

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

    /// <summary>
    /// 关闭设备
    /// </summary>
    public virtual void Close()
    {
        ZLGApiProvider.Instance.CloseDevice(UintDeviceType, DeviceIndex);

        IsConnected = false;
    }

    public virtual void Connect()
    {
        this.TryReadErrorInfo(out _);
        this.TryReadBoardInfo(out _);
        this.TryReadStatus(out _);

        if (!ZLGApiProvider.Instance.StartCAN(UintDeviceType, DeviceIndex, CanIndex))
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
        if (isDisposed)
            return;
        Unsubscribe();
        ZLGApiProvider.Instance.CloseDevice(UintDeviceType, DeviceIndex);
        IsConnected = false;
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

        BoardInfo = info;
        return BoardInfo;
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

        ErrorInfo = errorInfo;
        return ErrorInfo;
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

        Status = status;
        return Status;
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
    /// 注册监听设备。必须连接后再注册
    /// <para>当设备有未读取的未被读取的帧数时，将触发 <paramref name="onReceived"/>。</para>
    /// <para>不允许多次注册。仅当前实例和入参的 <paramref name="pollingTimeout"/>，<paramref name="waitTime"/> 一致时，视为同一个监听者</para>
    /// <para>同一个监听者第二次之后的注册将不会有任何动作。</para>
    /// <para>允许注册多个 <paramref name="onReceived"/> 回调，同一个监听者同一个回调多次注册时，仅第一次有效。</para>
    /// </summary>
    /// <param name="onReceived">当CAN 通道的接收缓冲区中，存在接收到但尚未被读取的帧数时，将触发此回调</param>
    /// <param name="pollingTimeout">长轮询的Delay时长、单位毫秒,默认为一百毫秒</param>
    /// <param name="waitTime">读取设备用的 api的入参,缓冲区无数据，函数阻塞等待时间，以毫秒为单位。若为-1 则表示无超时，一直等待。</param>
    /// <exception cref="InvalidOperationException">该实例被 Dispose后，或处于未连接状态时，调用此方法将抛出此异常</exception>
    public virtual void Subscribe(Action<CanObject[]> onReceived, int pollingTimeout = 100, int waitTime = 0)
    {
        if (isDisposed)
            throw new InvalidOperationException();
        if (!IsConnected)
            throw new InvalidOperationException();
        var canListenerConfig = new CanListenerConfig()
        {
            Device = this,
            PollingTimeout = pollingTimeout,
            WaitTime = waitTime
        };
        CanListenerManager.Subscribe(canListenerConfig, onReceived);
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

    /// <summary>
    /// 取消监听设备。
    /// <para>仅当前实例和入参的 <paramref name="pollingTimeout"/>，<paramref name="waitTime"/> 一致时，视为同一个监听者</para>
    /// </summary>
    /// <param name="onReceived">当CAN 通道的接收缓冲区中，存在接收到但尚未被读取的帧数时，将触发此回调</param>
    /// <param name="pollingTimeout">长轮询的Delay时长、单位毫秒,默认为一百毫秒</param>
    /// <param name="waitTime">读取设备用的 api的入参,缓冲区无数据，函数阻塞等待时间，以毫秒为单位。若为-1 则表示无超时，一直等待。</param>
    public virtual void Unsubscribe(Action<CanObject[]> onReceived, int pollingTimeout = 100, int waitTime = 0)
    {
        var record = new CanListenerConfig()
        {
            Device = this,
            PollingTimeout = pollingTimeout,
            WaitTime = waitTime
        };
        CanListenerManager.Unsubscribe(record, onReceived);
    }

    /// <summary>
    /// 取消当前设备的所有监听
    /// </summary>
    public void Unsubscribe()
    {
        CanListenerManager.Unsubscribe(this);
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