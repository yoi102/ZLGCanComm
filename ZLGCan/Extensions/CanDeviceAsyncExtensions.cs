using ZLGCan.Interfaces;
using ZLGCan.Structs;

namespace ZLGCan.Extensions;

public static class CanDeviceAsyncExtensions
{
    public static async Task<bool> TryConnectAsync(this ICanDevice canDevice, CancellationToken cancellationToken = default)
    {
        return await Task.Run(canDevice.TryConnect, cancellationToken);
    }

    public static async Task<CanControllerStatus> ReadCanControllerStatusAsync(this ICanDevice canDevice, CancellationToken cancellationToken = default)
    {
        return await Task.Run(canDevice.ReadCanControllerStatus, cancellationToken);
    }

    public static async Task<ErrorInfo> ReadErrorInfoAsync(this ICanDevice canDevice, CancellationToken cancellationToken = default)
    {
        return await Task.Run(canDevice.ReadErrorInfo, cancellationToken);
    }

    public static async Task<CanObject> ReadMessageAsync(this ICanDevice canDevice, uint length = 1, int waitTime = 0, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => canDevice.ReadMessage(length, waitTime), cancellationToken);
    }

    public static async Task<CanObject> WriteMessageAsync(this ICanDevice canDevice, CanObject canObject, uint length = 1, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => canDevice.WriteMessage(canObject, length), cancellationToken);
    }

    public static async Task<CanObject> WriteMessageAsync(this ICanDevice canDevice, uint id, byte[] message, uint length = 1, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => canDevice.WriteMessage(id, message, length), cancellationToken);
    }
}