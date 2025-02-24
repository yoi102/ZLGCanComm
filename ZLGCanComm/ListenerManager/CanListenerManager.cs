using System.Collections.Concurrent;
using ZLGCanComm.Interfaces;
using ZLGCanComm.Records;

namespace ZLGCanComm.ListenerManager;

internal class CanListenerManager
{
    private static readonly SynchronizationContext? _syncContext = SynchronizationContext.Current;
    private static readonly ConcurrentDictionary<CanListenerConfig, CanListenerSession> listeners = new();

    /// <summary>
    /// 注册监听任务
    /// </summary>
    /// <param name="canListenerConfig"></param>
    /// <param name="onReceived"></param>
    internal static void Subscribe(CanListenerConfig canListenerConfig, Action<CanObject[]> onReceived)
    {
        if (listeners.TryGetValue(canListenerConfig, out var existingListener))
        {
            if (existingListener.Task.IsCanceled || existingListener.Task.IsCompleted)
            {
                listeners.TryRemove(canListenerConfig, out _);
            }
            else
            {
                if (!existingListener.CallBacks.Any(x => x.Target == onReceived.Target && x.Method == onReceived.Method))
                {
                    existingListener.CallBacks.Add(onReceived);
                    return;
                }
            }
        }

        var cts = new CancellationTokenSource();
        var token = cts.Token;

        var task = Task.Run(async () =>
        {
            await ReadLoopAsync(canListenerConfig, token);
        }, token);

        var session = new CanListenerSession
        {
            Task = task,
            CancellationTokenSource = cts,
            CallBacks = [onReceived]
        };

        listeners[canListenerConfig] = session;
    }

    /// <summary>
    /// 停止监听指定设备
    /// </summary>
    /// <param name="device"></param>
    internal static void Unsubscribe(ICanDevice device)
    {
        var pairs = listeners.Where(x => x.Key.Device == device)
                                                                         .ToArray();

        foreach (var item in pairs)
        {
            listeners.TryRemove(item);
            item.Value.CancellationTokenSource.Cancel();
        }
    }

    /// <summary>
    /// 注销监听
    /// </summary>
    /// <param name="canListenerConfig"></param>
    /// <param name="onReceived"></param>
    internal static void Unsubscribe(CanListenerConfig canListenerConfig, Action<CanObject[]> onReceived)
    {
        if (!listeners.TryGetValue(canListenerConfig, out var removedListener))
            return;
        if (removedListener.CallBacks.Count <= 1)
        {
            listeners.Remove(canListenerConfig, out _);
            removedListener.CancellationTokenSource.Cancel();
        }
        else
        {
            removedListener.CallBacks.Remove(onReceived);
        }
    }

    /// <summary>
    /// 轮询读取 CAN 设备数据，并在有读取到数据时触发回调
    /// </summary>
    /// <param name="canListenerConfig"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    private static async Task ReadLoopAsync(CanListenerConfig canListenerConfig, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await Task.Delay(canListenerConfig.PollingTimeout, token);

            CanObject[] canObjects;
            try
            {
                canObjects = canListenerConfig.Device.Receive(canListenerConfig.WaitTime); // 读取端口数据

                if (canObjects.Length == 0)
                    continue;
            }
            catch (InvalidOperationException)
            {
                break;
            }

            if (listeners.TryGetValue(canListenerConfig, out var listener))
            {
                // 仅当值变化时触发回调
                listener.InvokeAll(canObjects, _syncContext);
            }
        }
    }
}