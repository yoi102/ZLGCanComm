using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZLGCan.Devices;
using ZLGCan.Extensions;

namespace ZLGCan;

record ListenerRecord
{
    public Task? Task { get; init; }
    public List<Action<(uint, byte[])>> CallBacks { get; init; } = new();
    public required CancellationTokenSource CancellationTokenSource { get; init; }
    public (uint, byte[])? OldValue { get; set; }

    public void InvokeAll((uint, byte[]) newValue, SynchronizationContext? syncContext)
    {
        foreach (var callback in CallBacks)
        {
            if (syncContext != null)
            {
                syncContext.Post(_ => callback(newValue), null);
            }
            else
            {
                callback(newValue);
            }
        }
    }
}

internal class ListenerService
{
    private static readonly ConcurrentDictionary<BaseDevice, ListenerRecord> listeners = new();
    private static readonly SynchronizationContext? _syncContext = SynchronizationContext.Current;
    private static readonly object _lock = new object();

    public static void ListenDevice(BaseDevice device, Action<(uint, byte[])> onChange, uint length = 1, int waitTime = 0)
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

            (uint, byte[]) result;
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

    public static bool AreMessagesEqual((uint, byte[])? message1, (uint, byte[])? message2)
    {
        if (message1 == null && message2 == null)
        {
            return true;
        }
        if (message1 == null || message2 == null)
        {
            return false;
        }

        if (message1.Value.Item1 != message2.Value.Item1)
        {
            return false;
        }

        return message1.Value.Item2.SequenceEqual(message2.Value.Item2);
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