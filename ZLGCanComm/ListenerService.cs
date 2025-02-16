using System.Collections.Concurrent;
using ZLGCanComm.Devices;
using ZLGCanComm.Structs;

namespace ZLGCanComm;

internal record ListenerTaskRecord
{
    public Task? Task { get; init; }
    public HashSet<Action<CanObject>> CallBacks { get; init; } = [];
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
internal record ListenerObjectRecord
{
    public required BaseDevice Device { get; init; }
    public required int PollingTimeout { get; init; }
    public required uint Length { get; init; }
    public required int WaitTime { get; init; }
}

internal class ListenerService
{
    private static readonly ConcurrentDictionary<ListenerObjectRecord, ListenerTaskRecord> listeners = new();
    private static readonly SynchronizationContext? _syncContext = SynchronizationContext.Current;
    private static readonly object _lock = new();

    public static void RegisterListener(ListenerObjectRecord listenerObjectRecord, Action<CanObject> onChange)
    {
        if (listeners.TryGetValue(listenerObjectRecord, out var existingListener))
        {
            existingListener.CallBacks.Add(onChange);
            return;
        }

        var cts = new CancellationTokenSource();
        var token = cts.Token;

        var task = Task.Run(async () =>
        {
            await ReadLoopAsync(listenerObjectRecord, token);
        }, token);

        var newRecord = new ListenerTaskRecord
        {
            Task = task,
            CancellationTokenSource = cts,
        };

        listeners[listenerObjectRecord] = newRecord;
    }

    /// <summary>
    /// 读取设备端口数据并检测变化
    /// </summary>
    private static async Task ReadLoopAsync(ListenerObjectRecord listenerObjectRecord, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            await Task.Delay(listenerObjectRecord.PollingTimeout, token);

            CanObject result;
            try
            {
                result = listenerObjectRecord.Device.ReadMessage(listenerObjectRecord.Length, listenerObjectRecord.WaitTime); // 读取端口数据
            }
            catch (CanDeviceOperationException)
            {
                StopListen(listenerObjectRecord.Device);
                break;
            }
            catch (InvalidOperationException)
            {
                break;
            }

            if (listeners.TryGetValue(listenerObjectRecord, out var listener))
            {
                // 仅当值变化时触发回调
                if (listener.OldValue == result)
                {
                    listener.OldValue = result;
                    listener.InvokeAll(result, _syncContext);
                }
            }
        }
    }

    public static void UnregisterListener(ListenerObjectRecord listenerObjectRecord, Action<CanObject> callBack)
    {
        if (!listeners.TryGetValue(listenerObjectRecord, out var removedListener))
            return;
        if (removedListener.CallBacks.Count <= 1)
        {
            listeners.Remove(listenerObjectRecord, out _);
            removedListener.CancellationTokenSource.Cancel();
        }
        else
        {
            removedListener.CallBacks.Remove(callBack);
        }
    }

    public static void StopListen(BaseDevice device)
    {
        lock (_lock)
        {
            var pairs = listeners.Where(x => x.Key.Device == device)
                                                                             .ToArray();
            if (pairs.Length == 0)
                return;
            device.OnConnectionLost();

            foreach (var item in pairs)
            {
                listeners.TryRemove(item);
                item.Value.CancellationTokenSource.Cancel();
            }
        }
    }
}