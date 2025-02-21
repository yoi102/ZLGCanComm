using ZLGCanComm.Enums;
using ZLGCanComm.Records;

namespace ZLGCanComm.Interfaces;

public interface ICanDevice : IDisposable
{
    /// <summary>
    /// <para>当设备意外断开时，将触发此事件。</para>
    /// <para>注意：此事件无法实时反映Can的是否断开、只有在读写失败后才会触发此事件。</para>
    /// <para>断开后，所有监听内容将被清除，在重新连接时请重新设置监听。</para>
    /// </summary>
    event Action<ICanDevice>? ConnectionLost;

    /// <summary>
    /// ZLGCAN系列接口卡信息的数据类型
    /// </summary>
    BoardInfo? BoardInfo { get; }

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
    /// 最近一次错误信息
    /// </summary>
    ErrorInfo? ErrorInfo { get; }

    /// <summary>
    /// 是否已经连接
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// 状态
    /// </summary>
    CanControllerStatus? Status { get; }

    /// <summary>
    /// 连接设备
    /// </summary>
    /// <exception cref="InvalidOperationException">该实例被 Dispose后，或处于未连接状态时，调用此方法将抛出此异常</exception>
    /// <exception cref="CanDeviceOperationException">若ZLGCan的Api返回值为0时，将抛出此异常</exception>
    void Connect();

    /// <summary>
    /// 获取接收缓冲区中，接收到但尚未被读取的帧数量
    /// </summary>
    /// <returns></returns>
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
    /// 获取ZLGCan控制器的最后一次错误信息。
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">该实例被 Dispose后，或处于未连接状态时，调用此方法将抛出此异常</exception>
    /// <exception cref="CanDeviceOperationException">若ZLGCan的Api返回值为0时，将抛出此异常</exception>
    ErrorInfo ReadErrorInfo();

    /// <summary>
    /// 获取ZLGCan控制器接收缓冲区中接收到但尚未被读取的帧数。
    /// </summary>
    /// <param name="length"></param>
    /// <param name="waitTime"></param>
    /// <returns>当没有未被读取的帧数时，将返回 null</returns>
    /// <exception cref="InvalidOperationException">该实例被 Dispose后，或处于未连接状态时，调用此方法将抛出此异常</exception>
    /// <exception cref="CanDeviceOperationException">若ZLGCan的Api返回值为0时，将抛出此异常</exception>
    CanObject? ReadMessage(uint length = 1, int waitTime = 0);

    /// <summary>
    /// 直接获取ZLGCan控制器接收缓冲区中接收到但尚未被读取的帧数。
    /// 以获取Can信息帧
    /// </summary>
    /// <param name="length"></param>
    /// <param name="waitTime"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">该实例被 Dispose后，或处于未连接状态时，调用此方法将抛出此异常</exception>
    /// <exception cref="CanDeviceOperationException">若ZLGCan的Api返回值为0时，将抛出此异常</exception>
    CanObject ReadMessageDirect(uint length = 1, int waitTime = 0);

    /// <summary>
    /// 读取Can控制器信息
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">该实例被 Dispose后，或处于未连接状态时，调用此方法将抛出此异常</exception>
    /// <exception cref="CanDeviceOperationException">若ZLGCan的Api返回值为0时，将抛出此异常</exception>
    CanControllerStatus ReadStatus();

    /// <summary>
    /// 注册监听设备。必须连接后再注册
    /// <para>当设备有未读取的未被读取的帧数时，将触发 <paramref name="onChange"/>。</para>
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
    void RegisterListener(Action<CanObject> onChange, int pollingTimeout = 100, uint length = 1, int waitTime = 0);

    /// <summary>
    /// 取消当前设备的所有监听
    /// </summary>
    void UnregisterAllListener();

    /// <summary>
    /// 取消指定监听。
    /// <para>仅当前实例和入参的 <paramref name="pollingTimeout"/>，<paramref name="length"/>，<paramref name="waitTime"/> 一致时，视为同一个监听者</para>
    /// </summary>
    /// <param name="onChange">当去读的值发生变化时将触发</param>
    /// <param name="pollingTimeout">长轮询的Delay时长、单位毫秒,默认为一百毫秒</param>
    /// <param name="length">读取设备用的 api的入参</param>
    /// <param name="waitTime">读取设备用的 api的入参</param>
    void UnregisterListener(Action<CanObject> onChange, int pollingTimeout = 100, uint length = 1, int waitTime = 0);

    /// <summary>
    /// 向ZLGCan控制器发送帧数
    /// </summary>
    /// <param name="canObject"></param>
    /// <param name="length"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">该实例被 Dispose后，或处于未连接状态时，调用此方法将抛出此异常</exception>
    /// <exception cref="CanDeviceOperationException">若ZLGCan的Api返回值为0时，将抛出此异常</exception>
    CanObject WriteMessage(CanObject canObject, uint length = 1);

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
    CanObject WriteMessage(uint id, byte[] message, uint length = 1);
}