using ZLGCanComm.Enums;
using ZLGCanComm.Structs;

namespace ZLGCanComm.Interfaces;

public interface ICanDevice : IDisposable
{
    /// <summary>
    /// 设备连接类型
    /// </summary>
    DeviceType DeviceType { get; }

    /// <summary>
    /// 是否已经连接
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    /// 当设备意外断开时，将触发次事件、所有监听内容将被清除
    /// </summary>
    event Action<ICanDevice>? ConnectionLost;

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
    void RegisterListener(Action<CanObject> onChange, int pollingTimeout = 100, uint length = 1, int waitTime = 0);

    /// <summary>
    /// 取消监听设备。
    /// <para>当当前实例和入参的 <paramref name="pollingTimeout"/>，<paramref name="length"/>，<paramref name="waitTime"/> 一致时，视为同一个监听者</para>
    /// </summary>
    /// <param name="onChange">当去读的值发生变化时将触发</param>
    /// <param name="pollingTimeout">长轮询的Delay时长、单位毫秒,默认为一百毫秒</param>
    /// <param name="length">读取设备用的 api的入参</param>
    /// <param name="waitTime">读取设备用的 api的入参</param>
    void UnregisterListener(Action<CanObject> onChange, int pollingTimeout = 100, uint length = 1, int waitTime = 0);

    /// <summary>
    /// 尝试连接设备，如果连接上将返回True，否则返回 false
    /// </summary>
    /// <returns></returns>
    bool TryConnect();

    CanControllerStatus ReadCanControllerStatus();

    ErrorInfo ReadErrorInfo();

    CanObject ReadMessage(uint length = 1, int waitTime = 0);

    CanObject WriteMessage(CanObject canObject, uint length = 1);

    CanObject WriteMessage(uint id, byte[] message, uint length = 1);

    bool TryReadCanControllerStatus(out CanControllerStatus status);

    bool TryReadErrorInfo(out ErrorInfo errorInfo);

    bool TryReadMessage(out CanObject canObject, uint length = 1, int waitTime = 0);

    bool TryWriteMessage(ref CanObject canObject, uint length = 1);

    bool TryWriteMessage(uint id, byte[] message, uint length = 1);
}