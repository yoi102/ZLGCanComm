using ZLGCanComm.Interfaces;
using ZLGCanComm.Structs;

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
    /// 尝试读取Can控制器信息
    /// </summary>
    /// <param name="canDevice"></param>
    /// <param name="status"></param>
    /// <returns>成功时将返回 true，反之返回 false</returns>
    public static bool TryReadStatus(this ICanDevice canDevice, out CanControllerStatus status)
    {
        status = new CanControllerStatus();
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
    /// 尝试获取设备信息
    /// </summary>
    /// <param name="canDevice"></param>
    /// <param name="info"></param>
    /// <returns>成功时将返回 true，反之返回 false</returns>
    public static bool TryReadBoardInfo(this ICanDevice canDevice, out BoardInfo info)
    {
        info = new BoardInfo();
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
    /// 尝试获取ZLGCan控制器的最后一次错误信息
    /// </summary>
    /// <param name="canDevice"></param>
    /// <param name="errorInfo"></param>
    /// <returns>成功时将返回 true，反之返回 false</returns>
    public static bool TryReadErrorInfo(this ICanDevice canDevice, out ErrorInfo errorInfo)
    {
        errorInfo = new ErrorInfo();
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
    /// 尝试获取ZLGCan控制器的最后一次错误信息
    /// </summary>
    /// <param name="canDevice"></param>
    /// <param name="canObject"></param>
    /// <param name="length"></param>
    /// <param name="waitTime"></param>
    /// <returns>成功时将返回 true，反之返回 false</returns>
    public static bool TryReadMessage(this ICanDevice canDevice, out CanObject canObject, uint length = 1, int waitTime = 0)
    {
        canObject = new CanObject();

        try
        {
            canObject = canDevice.ReadMessage();
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
    /// 尝试获取ZLGCan控制器接收缓冲区中接收到但尚未被读取的帧数
    /// </summary>
    /// <param name="canDevice"></param>
    /// <param name="canObject"></param>
    /// <param name="length"></param>
    /// <returns>成功时将返回 true，反之返回 false</returns>
    public static bool TryWriteMessage(this ICanDevice canDevice, ref CanObject canObject, uint length = 1)
    {
        try
        {
            canObject = canDevice.WriteMessage(canObject);
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
    /// 尝试向ZLGCan控制器发送帧数
    /// </summary>
    /// <param name="canDevice"></param>
    /// <param name="id"></param>
    /// <param name="message"></param>
    /// <param name="length"></param>
    /// <returns>成功时将返回 true，反之返回 false</returns>
    public static bool TryWriteMessage(this ICanDevice canDevice, uint id, byte[] message, uint length = 1)
    {
        try
        {
            canDevice.WriteMessage(id, message, length);
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
}