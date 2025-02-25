﻿using ZLGCanComm.Enums;
using ZLGCanComm.Records;

namespace ZLGCanComm.Interfaces;

public interface ICanDevice : IDisposable
{
    /// <summary>
    /// 设备发生错误时将触发此事件。
    /// <para>连接设备成功后将长轮询读取设备错误信息。</para>
    /// <para>连接时会读取一次ErrorInfo，长轮询读取的ErrorInfo不等于连接时的ErrorInfo且错误码不为0x00000100时，将触发此事件。</para>
    /// <para>发生错误不会清空所有的订阅</para>
    /// </summary>
    event Action<ErrorInfo>? ErrorOccurred;

    /// <summary>
    /// 连接意外丢失，设备意外丢失。
    /// <para>连接设备成功后将长轮询读取设备错误信息，读取错误信息失败或者错误码为0x00001000时，将视为设备丢失。</para>
    /// <para>设备丢失不会清空所有的订阅</para>
    /// </summary>
    event Action<ICanDevice>? LostConnection;

    /// <summary>
    /// CAN 通道号
    /// </summary>
    uint CanIndex { get; }

    /// <summary>
    /// 设备索引号
    /// </summary>
    uint DeviceIndex { get; }

    /// <summary>
    /// 设备连接类型
    /// </summary>
    DeviceType DeviceType { get; }

    /// <summary>
    /// 长轮询读取错误信息时，时间间隔,单位毫秒，默认设置为500。
    /// <para>连接设备后将长轮询读取设备错误信息</para>
    /// </summary>
    int ErrorPollingInterval { get; set; }

    /// <summary>
    /// 是否已经连接
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// 记录上次错误状态
    /// <para>连接时，将读取一次ErrorInfo，之后长轮询中如读取的值不同时将更新</para>
    /// </summary>
    ErrorInfo? LastErrorInfo { get; }

    /// <summary>
    /// 连接设备
    /// </summary>
    /// <exception cref="InvalidOperationException">该实例被 Dispose后，或处于未连接状态时，调用此方法将抛出此异常</exception>
    /// <exception cref="CanDeviceOperationException">若ZLGCan的Api返回值为0时，将抛出此异常</exception>
    void Connect();

    /// <summary>
    /// 关闭设备,断开连接。
    /// <para>将清空所有的订阅</para>
    /// </summary>
    void Disconnect();
    /// <summary>
    /// 获取接收缓冲区中，接收到但尚未被读取的帧数量
    /// </summary>
    /// <returns>接收到但尚未被读取的帧数量</returns>
    /// <exception cref="InvalidOperationException"></exception>
    uint GetCanReceiveCount();

    /// <summary>
    /// 获取设备信息
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">该实例被 Dispose后，或处于未连接状态时，调用此方法将抛出此异常</exception>
    /// <exception cref="CanDeviceOperationException">若ZLGCan的Api返回值为0时，将抛出此异常</exception>
    BoardInfo ReadBoardInfo();

    /// <summary>
    /// 获取ZLGCan控制器的最近一次错误信息。
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">该实例被 Dispose后，或处于未连接状态时，调用此方法将抛出此异常</exception>
    /// <exception cref="CanDeviceOperationException">若ZLGCan的Api返回值为0时，将抛出此异常</exception>
    ErrorInfo ReadErrorInfo();

    /// <summary>
    /// 读取Can控制器信息
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">该实例被 Dispose后，或处于未连接状态时，调用此方法将抛出此异常</exception>
    /// <exception cref="CanDeviceOperationException">若ZLGCan的Api返回值为0时，将抛出此异常</exception>
    CanControllerStatus ReadStatus();

    /// <summary>
    /// 接收缓冲区中读取数据。
    /// </summary>
    /// <param name="waitTime">缓冲区无数据，函数阻塞等待时间，以毫秒为单位。若为-1 则表示无超时，一直等待。</param>
    /// <returns>反回所读取到的数据，当控制器没有数据可读时、 CanObject[0]</returns>
    /// <exception cref="InvalidOperationException">该实例被 Dispose后，或处于未连接状态时，调用此方法将抛出此异常</exception>
    CanObject[] Receive(int waitTime = 0);

    /// <summary>
    /// 复位，将会断开连接
    /// <para>将清空所有的订阅</para>
    /// </summary>
    void Reset();

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
    void Subscribe(Action<CanObject[]> onReceived, int pollingTimeout = 100, int waitTime = 0);

    /// <summary>
    /// 向ZLGCan控制器发送数据。返回值为实际发送成功的帧数。
    /// </summary>
    /// <param name="canObject"></param>
    /// <returns>返回实际发送成功的帧数。</returns>
    /// <exception cref="InvalidOperationException">该实例被 Dispose后，或处于未连接状态时，调用此方法将抛出此异常</exception>
    uint Transmit(params CanObject[] canObject);

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
    bool Transmit(uint id, byte[] data);

    /// <summary>
    /// 取消监听设备。
    /// <para>仅当前实例和入参的 <paramref name="pollingTimeout"/>，<paramref name="waitTime"/> 一致时，视为同一个监听者</para>
    /// </summary>
    /// <param name="onReceived">当CAN 通道的接收缓冲区中，存在接收到但尚未被读取的帧数时，将触发此回调</param>
    /// <param name="pollingTimeout">长轮询的Delay时长、单位毫秒,默认为一百毫秒</param>
    /// <param name="waitTime">读取设备用的 api的入参,缓冲区无数据，函数阻塞等待时间，以毫秒为单位。若为-1 则表示无超时，一直等待。</param>
    void Unsubscribe(Action<CanObject[]> onReceived, int pollingTimeout = 100, int waitTime = 0);

    /// <summary>
    /// 取消当前设备的所有监听
    /// </summary>
    void Unsubscribe();
}