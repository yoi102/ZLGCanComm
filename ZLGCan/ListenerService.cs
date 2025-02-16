using System.Collections.Concurrent;
using ZLGCan.Devices;
using ZLGCan.Structs;

namespace ZLGCan;

internal record ListenerRecord
{
    public Task? Task { get; init; }
    public List<Action<CanObject>> CallBacks { get; init; } = new();
    public required CancellationTokenSource CancellationTokenSource { get; init; }
    public CanObject? OldValue { get; set; }

    public void InvokeAll(CanObject newCanObject, SynchronizationContext? syncContext)
    {
        foreach (var callback in CallBacks)
        {
            if (syncContext != null)
            {
                syncContext.Post(_ => callback(newCanObject), null);
            }
            else
            {
                callback(newCanObject);
            }
        }
    }
}

internal class ListenerService
{
    private static readonly ConcurrentDictionary<BaseDevice, ListenerRecord> listeners = new();
    private static readonly SynchronizationContext? _syncContext = SynchronizationContext.Current;
    private static readonly object _lock = new object();

    public static void ListenDevice(BaseDevice device, Action<CanObject> onChange, uint length = 1, int waitTime = 0)
    {
        if (listeners.TryGetValue(device, out var existingListener))
        {
            existingListener.CallBacks.Add(onChange);
            return;
        }

        var cts = new CancellationTokenSource();
        var token = cts.Token;

        var task = Task.Run(async () =>
        {
            await ReadLoopAsync(device, token, length, waitTime);
        }, token);

        var newRecord = new ListenerRecord
        {
            Task = task,
            CancellationTokenSource = cts,
        };

        listeners[device] = newRecord;
    }

    /// <summary>
    /// 读取设备端口数据并检测变化
    /// </summary>
    private static async Task ReadLoopAsync(BaseDevice device, CancellationToken token, uint length = 1, int waitTime = 0)
    {
        while (!token.IsCancellationRequested)
        {
            await Task.Delay(100);

            CanObject result;
            try
            {
                result = device.ReadMessage(length, waitTime); // 读取端口数据
            }
            catch (CanDeviceOperationException)
            {
                StopListen(device);
                break;
            }

            if (listeners.TryGetValue(device, out var listener))
            {
                // 仅当值变化时触发回调
                if (AreMessagesEqual(listener.OldValue, result))
                {
                    listener.OldValue = result;
                    listener.InvokeAll(result, _syncContext);
                }
            }
        }
    }

    public static bool AreMessagesEqual(CanObject? canObject1, CanObject? canObject2)
    {
        if (canObject1 == null && canObject2 == null)
        {
            return true;
        }
        if (canObject1 == null || canObject2 == null)
        {
            return false;
        }

        return canObject1 == canObject2;

    }

    public static void StopListen(BaseDevice device)
    {
        lock (_lock)
        {
            device.OnConnectionLost();
            if (listeners.TryRemove(device, out var removedListener))
            {
                removedListener.CancellationTokenSource.Cancel();
            }
            foreach (var item in listeners.Values)
            {
                item.CancellationTokenSource.Cancel();
            }
        }
    }
}