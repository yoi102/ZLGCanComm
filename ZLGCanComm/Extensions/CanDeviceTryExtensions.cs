using System.Diagnostics.CodeAnalysis;
using ZLGCanComm.Interfaces;
using ZLGCanComm.Records;

namespace ZLGCanComm.Extensions;

public static class CanDeviceTryExtensions
{
    /// <summary>
    /// 尝试连接设备
    /// </summary>
    /// <param name="canDevice"></param>
    /// <returns>成功时将返回 true，反之返回 false</returns>
    public static bool TryConnect(this ICanDevice canDevice)
    {
        try
        {
            canDevice.Connect();
        }
        catch (InvalidOperationException)
        {
            return false;
        }
        catch (CanDeviceOperationException)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 尝试获取接收缓冲区中，接收到但尚未被读取的帧数量
    /// </summary>
    /// <param name="canDevice"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static bool TryGetCanReceiveCount(this ICanDevice canDevice, out uint count)
    {
        count = 0;
        try
        {
            count = canDevice.GetCanReceiveCount();
        }
        catch (InvalidOperationException)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 尝试获取设备信息
    /// </summary>
    /// <param name="canDevice"></param>
    /// <param name="info"></param>
    /// <returns>成功时将返回 true，反之返回 false</returns>
    public static bool TryReadBoardInfo(this ICanDevice canDevice, [NotNullWhen(true)] out BoardInfo? info)
    {
        info = null;
        try
        {
            info = canDevice.ReadBoardInfo();
        }
        catch (InvalidOperationException)
        {
            return false;
        }
        catch (CanDeviceOperationException)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 尝试获取ZLGCan控制器的最近一次错误信息
    /// </summary>
    /// <param name="canDevice"></param>
    /// <param name="errorInfo"></param>
    /// <returns>成功时将返回 true，反之返回 false</returns>
    public static bool TryReadErrorInfo(this ICanDevice canDevice, [NotNullWhen(true)] out ErrorInfo? errorInfo)
    {
        errorInfo = null;
        try
        {
            errorInfo = canDevice.ReadErrorInfo();
        }
        catch (InvalidOperationException)
        {
            return false;
        }
        catch (CanDeviceOperationException)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 尝试读取Can控制器信息
    /// </summary>
    /// <param name="canDevice"></param>
    /// <param name="status"></param>
    /// <returns>成功时将返回 true，反之返回 false</returns>
    public static bool TryReadStatus(this ICanDevice canDevice, [NotNullWhen(true)] out CanControllerStatus? status)
    {
        status = null;
        try
        {
            status = canDevice.ReadStatus();
        }
        catch (InvalidOperationException)
        {
            return false;
        }
        catch (CanDeviceOperationException)
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 尝试接收缓冲区中读取数据。
    /// </summary>
    /// <param name="canDevice"></param>
    /// <param name="canObjects"></param>
    /// <param name="waitTime">缓冲区无数据，函数阻塞等待时间，以毫秒为单位。若为-1 则表示无超时，一直等待。</param>
    /// <returns>成功时将返回 true，反之返回 false</returns>
    public static bool TryReceive(this ICanDevice canDevice, [NotNullWhen(true)] out CanObject[]? canObjects, int waitTime = 0)
    {
        canObjects = null;
        try
        {
            canObjects = canDevice.Receive(waitTime);
            return true;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    /// <summary>
    /// 尝试向ZLGCan控制器发送单帧数据
    /// </summary>
    /// <param name="canDevice"></param>
    /// <param name="id">帧 ID。32 位变量，数据格式为靠右对齐</param>
    /// <param name="data">CAN 帧的数据，最多 8 字节 ；长度可小于8</param>
    /// <returns>成功时将返回 true，反之返回 false</returns>
    public static bool TryTransmit(this ICanDevice canDevice, uint id, byte[] data)
    {
        try
        {
            return canDevice.Transmit(id, data);
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }

    /// <summary>
    /// 尝试向ZLGCan控制器发送数据。
    /// </summary>
    /// <param name="canDevice"></param>
    /// <param name="transmittedCount">为实际发送成功的帧数</param>
    /// <param name="canObjects"></param>
    /// <returns>当发送的帧数等于实际发送的帧数时时将返回 true，反之返回 false</returns>
    public static bool TryTransmit(this ICanDevice canDevice, out uint transmittedCount, params CanObject[] canObjects)
    {
        transmittedCount = 0;
        try
        {
            transmittedCount = canDevice.Transmit(canObjects);
            return transmittedCount == canObjects.Length;
        }
        catch (InvalidOperationException)
        {
            return false;
        }
    }
}