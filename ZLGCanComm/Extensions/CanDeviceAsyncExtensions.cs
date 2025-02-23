using ZLGCanComm.Interfaces;
using ZLGCanComm.Records;

namespace ZLGCanComm.Extensions;

public static class CanDeviceAsyncExtensions
{
    /// <summary>
    /// 获取接收缓冲区中，接收到但尚未被读取的帧数量
    /// </summary>
    /// <param name="canDevice"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>接收到但尚未被读取的帧数量</returns>
    public static async Task<uint> CanGetReceiveCountAsync(this ICanDevice canDevice, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => canDevice.GetCanReceiveCount(), cancellationToken);
    }

    /// <summary>
    /// 连接设备
    /// </summary>
    /// <param name="canDevice"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task ConnectAsync(this ICanDevice canDevice, CancellationToken cancellationToken = default)
    {
        await Task.Run(canDevice.Connect, cancellationToken);
    }

    /// <summary>
    /// 获取设备信息
    /// </summary>
    /// <param name="canDevice"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<BoardInfo> ReadBoardInfoAsync(this ICanDevice canDevice, CancellationToken cancellationToken = default)
    {
        return await Task.Run(canDevice.ReadBoardInfo, cancellationToken);
    }

    /// <summary>
    /// 获取ZLGCan控制器的最近一次错误信息
    /// </summary>
    /// <param name="canDevice"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<ErrorInfo> ReadErrorInfoAsync(this ICanDevice canDevice, CancellationToken cancellationToken = default)
    {
        return await Task.Run(canDevice.ReadErrorInfo, cancellationToken);
    }

    /// <summary>
    /// 读取Can控制器信息
    /// </summary>
    /// <param name="canDevice"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<CanControllerStatus> ReadStatusAsync(this ICanDevice canDevice, CancellationToken cancellationToken = default)
    {
        return await Task.Run(canDevice.ReadStatus, cancellationToken);
    }

    /// <summary>
    /// 接收缓冲区中读取数据
    /// </summary>
    /// <param name="canDevice"></param>
    /// <param name="waitTime"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<CanObject[]> ReceiveAsync(this ICanDevice canDevice, int waitTime = 0, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => canDevice.Receive(waitTime), cancellationToken);
    }

    /// <summary>
    /// 向ZLGCan控制器发送数据。返回值为实际发送成功的帧数。
    /// </summary>
    /// <param name="canDevice"></param>
    /// <param name="canObjects"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>实际发送成功的帧数。</returns>
    public static async Task<uint> TransmitAsync(this ICanDevice canDevice, CanObject[] canObjects, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => canDevice.Transmit(canObjects), cancellationToken);
    }

    /// <summary>
    ///  以此配置
    /// <para>SendType = 0:正常发送</para>
    /// <para>RemoteFlag = 0:数据帧</para>
    /// <para>ExternFlag = 0:标准帧</para>
    ///  向ZLGCan控制器发送单帧数据
    /// </summary>
    /// <param name="canDevice"></param>
    /// <param name="id"></param>
    /// <param name="message"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>发送的帧数与实际发送的帧数是否相等</returns>
    public static async Task<bool> TransmitAsync(this ICanDevice canDevice, uint id, byte[] message, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => canDevice.Transmit(id, message), cancellationToken);
    }

    public static async Task<bool> TryConnectAsync(this ICanDevice canDevice, CancellationToken cancellationToken = default)
    {
        return await Task.Run(canDevice.TryConnect, cancellationToken);
    }
}