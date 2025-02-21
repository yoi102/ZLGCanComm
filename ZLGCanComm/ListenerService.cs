using System.Collections.Concurrent;
using ZLGCanComm.Devices;
using ZLGCanComm.Records;

namespace ZLGCanComm;

internal record ListenerTaskRecord
{
    public required Task Task { get; init; }
    public required HashSet<Action<CanObject>> CallBacks { get; init; }
    public required CancellationTokenSource CancellationTokenSource { get; init; }

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
    private static readonly SynchronizationContext? _syncContext = SynchronizationContext.Current;
    private static readonly ConcurrentDictionary<ListenerObjectRecord, ListenerTaskRecord> listeners = new();

    internal static void RegisterListener(ListenerObjectRecord listenerObjectRecord, Action<CanObject> onChange)
    {
        if (listeners.TryGetValue(listenerObjectRecord, out var existingListener))
        {
            if (existingListener.Task.IsCanceled || existingListener.Task.IsCompleted)
            {
                listeners.TryRemove(listenerObjectRecord, out _);
            }
            else
            {
                if (!existingListener.CallBacks.Any(x => x.Target == onChange.Target && x.Method == onChange.Method))
                {
                    existingListener.CallBacks.Add(onChange);
                }
                return;
            }
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
            CallBacks = [onChange]
        };

        listeners[listenerObjectRecord] = newRecord;
    }

    internal static void StopListenDevice(BaseDevice device)
    {
        var pairs = listeners.Where(x => x.Key.Device == device)
                                                                         .ToArray();

        foreach (var item in pairs)
        {
            listeners.TryRemove(item);
            item.Value.CancellationTokenSource.Cancel();
        }
    }

    internal static void UnregisterListener(ListenerObjectRecord listenerObjectRecord, Action<CanObject> callBack)
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
                var count = listenerObjectRecord.Device.GetCanReceiveCount();
                if (count == 0)
                {
                    continue;
                }
                result = listenerObjectRecord.Device.ReadMessageDirect(listenerObjectRecord.Length, listenerObjectRecord.WaitTime); // 读取端口数据
            }
            catch (CanDeviceOperationException)
            {
                break;
            }
            catch (InvalidOperationException)
            {
                break;
            }

            if (listeners.TryGetValue(listenerObjectRecord, out var listener))
            {
                // 仅当值变化时触发回调
                listener.InvokeAll(result, _syncContext);
            }
        }
    }
}