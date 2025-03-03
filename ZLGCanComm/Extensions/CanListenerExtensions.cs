using ZLGCanComm.Interfaces;
using ZLGCanComm.ListenerManager;
using ZLGCanComm.Records;

namespace ZLGCanComm.Extensions;

public static class CanListenerExtensions
{
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
    public static void Subscribe(this ICanDevice canDevice, Action<CanObject[]> onReceived, int pollingTimeout = 100, int waitTime = 0)
    {
        var canListenerConfig = new CanListenerConfig()
        {
            Device = canDevice,
            PollingTimeout = pollingTimeout,
            WaitTime = waitTime
        };
        CanListenerManager.Subscribe(canListenerConfig, onReceived);
    }

    /// <summary>
    /// 取消监听设备。
    /// <para>仅当前实例和入参的 <paramref name="pollingTimeout"/>，<paramref name="waitTime"/> 一致时，视为同一个监听者</para>
    /// </summary>
    /// <param name="onReceived">当CAN 通道的接收缓冲区中，存在接收到但尚未被读取的帧数时，将触发此回调</param>
    /// <param name="pollingTimeout">长轮询的Delay时长、单位毫秒,默认为一百毫秒</param>
    /// <param name="waitTime">读取设备用的 api的入参,缓冲区无数据，函数阻塞等待时间，以毫秒为单位。若为-1 则表示无超时，一直等待。</param>
    public static void Unsubscribe(this ICanDevice canDevice, Action<CanObject[]> onReceived, int pollingTimeout = 100, int waitTime = 0)
    {
        var record = new CanListenerConfig()
        {
            Device = canDevice,
            PollingTimeout = pollingTimeout,
            WaitTime = waitTime
        };
        CanListenerManager.Unsubscribe(record, onReceived);
    }

    /// <summary>
    /// 取消当前设备的所有监听
    /// </summary>
    public static void Unsubscribe(this ICanDevice canDevice)
    {
        CanListenerManager.Unsubscribe(canDevice);
    }
}