using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using ZLGCanComm.Records;

namespace ZLGCanComm.Api;

public class ZLGApiWrapper : IZLGApi
{
    public bool OpenDevice(uint deviceType, uint deviceIndex, uint reserved)
    {
        return ZLGApi.VCI_OpenDevice(deviceType, deviceIndex, reserved) != 0;
    }

    public bool CloseDevice(uint deviceType, uint deviceIndex)
    {
        return ZLGApi.VCI_CloseDevice(deviceType, deviceIndex) != 0;
    }

    public bool InitCAN(uint deviceType, uint deviceIndex, uint canIndex, InitConfig initConfig)
    {
        var config = DataConverter.Convert(initConfig);
        var result = ZLGApi.VCI_InitCAN(deviceType, deviceIndex, canIndex, ref config) != 0;

        return result;
    }

    public bool ReadBoardInfo(uint deviceType, uint deviceIndex, [NotNullWhen(true)] out BoardInfo? boardInfo)
    {
        boardInfo = null;
        var pInfo = DataConverter.Convert(new BoardInfo());
        var result = ZLGApi.VCI_ReadBoardInfo(deviceType, deviceIndex, ref pInfo) != 0;
        if (result)
        {
            boardInfo = DataConverter.Convert(pInfo);
        }

        return result;
    }

    public bool ReadErrInfo(uint deviceType, uint deviceIndex, uint canIndex, [NotNullWhen(true)] out ErrorInfo? errorInfo)
    {
        errorInfo = null;
        var pErrInfo = DataConverter.Convert(new ErrorInfo());
        var result = ZLGApi.VCI_ReadErrInfo(deviceType, deviceIndex, canIndex, ref pErrInfo) != 0;
        if (result)
        {
            errorInfo = DataConverter.Convert(pErrInfo);
        }

        return result;
    }

    public bool ReadCANStatus(uint deviceType, uint deviceIndex, uint canIndex, [NotNullWhen(true)] out CanControllerStatus? canControllerStatus)
    {
        canControllerStatus = null;
        var pCANStatus = DataConverter.Convert(new CanControllerStatus());
        var result = ZLGApi.VCI_ReadCANStatus(deviceType, deviceIndex, canIndex, ref pCANStatus) != 0;
        if (result)
        {
            canControllerStatus = DataConverter.Convert(pCANStatus);
        }

        return result;
    }

    public bool GetReference(uint deviceType, uint deviceIndex, uint canIndex, uint RefType, byte pData)
    {
        return ZLGApi.VCI_GetReference(deviceType, deviceIndex, canIndex, RefType, ref pData) != 0;
    }

    public bool SetReference(uint deviceType, uint deviceIndex, uint canIndex, uint RefType, byte pData)
    {
        return ZLGApi.VCI_SetReference(deviceType, deviceIndex, canIndex, RefType, ref pData) != 0;
    }

    public uint GetReceiveNum(uint deviceType, uint deviceIndex, uint canIndex)
    {
        return ZLGApi.VCI_GetReceiveNum(deviceType, deviceIndex, canIndex);
    }

    public bool ClearBuffer(uint deviceType, uint deviceIndex, uint canIndex)
    {
        return ZLGApi.VCI_ClearBuffer(deviceType, deviceIndex, canIndex) != 0;
    }

    public bool StartCAN(uint deviceType, uint deviceIndex, uint canIndex)
    {
        return ZLGApi.VCI_StartCAN(deviceType, deviceIndex, canIndex) != 0;
    }

    public bool ResetCAN(uint deviceType, uint deviceIndex, uint canIndex)
    {
        return ZLGApi.VCI_ResetCAN(deviceType, deviceIndex, canIndex) != 0;
    }

    public uint Transmit(uint deviceType, uint deviceIndex, uint canIndex, CanObject[] sendObjects)
    {
        return ZLGApi.VCI_Transmit(deviceType, deviceIndex, canIndex, DataConverter.Convert(sendObjects), (uint)sendObjects.Length);
    }

    public CanObject[] Receive(uint deviceType, uint deviceIndex, uint canIndex, uint length, int waitTime)
    {
        int structSize = Marshal.SizeOf<VCI_CAN_OBJ>(); // 避免重复计算

        IntPtr pt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VCI_CAN_OBJ)) * (int)length);

        try
        {
            var receivedCount = ZLGApi.VCI_Receive(deviceType, deviceIndex, canIndex, pt, length, waitTime);
            if (receivedCount == 0)
                return []; // 无数据时直接返回，避免不必要的计算

            CanObject[] result = new CanObject[receivedCount]; // 预分配数组，避免 List<T> 额外开销

            for (int i = 0; i < receivedCount; i++)
            {
                IntPtr currentPtr = IntPtr.Add(pt, i * structSize);
                VCI_CAN_OBJ received = Marshal.PtrToStructure<VCI_CAN_OBJ>(currentPtr);
                result[i] = DataConverter.Convert(received);
            }

            return result;
        }
        finally
        {
            Marshal.FreeHGlobal(pt);
        }
    }
}