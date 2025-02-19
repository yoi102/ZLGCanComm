using ZLGCanComm.Interfaces;
using ZLGCanComm.Structs;

namespace ZLGCanComm.Extensions;

public static class CanDeviceAsyncExtensions
{
    public static async Task ConnectAsync(this ICanDevice canDevice, CancellationToken cancellationToken = default)
    {
        await Task.Run(canDevice.Connect, cancellationToken);
    }

    public static async Task<BoardInfo> ReadBoardInfoAsync(this ICanDevice canDevice, CancellationToken cancellationToken = default)
    {
        return await Task.Run(canDevice.ReadBoardInfo, cancellationToken);
    }

    public static async Task<ErrorInfo> ReadErrorInfoAsync(this ICanDevice canDevice, CancellationToken cancellationToken = default)
    {
        return await Task.Run(canDevice.ReadErrorInfo, cancellationToken);
    }

    public static async Task<CanObject> ReadMessageAsync(this ICanDevice canDevice, uint length = 1, int waitTime = 0, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => canDevice.ReadMessage(length, waitTime), cancellationToken);
    }

    public static async Task<CanControllerStatus> ReadStatusAsync(this ICanDevice canDevice, CancellationToken cancellationToken = default)
    {
        return await Task.Run(canDevice.ReadStatus, cancellationToken);
    }

    public static async Task<bool> TryConnectAsync(this ICanDevice canDevice, CancellationToken cancellationToken = default)
    {
        return await Task.Run(canDevice.TryConnect, cancellationToken);
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