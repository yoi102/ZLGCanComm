namespace ZLGCan;
using System;
using System.Runtime.InteropServices;
using ZLGCan.Structs;

public static class ControlCan
{
    public const uint NODE = 0X400;//CAN节点的下行命令帧ID的前缀;


    [DllImport("controlcan.dll")]
    public static extern uint VCI_OpenDevice(uint DeviceType, uint DeviceInd, uint Reserved);
    [DllImport("controlcan.dll")]
    public static extern uint VCI_CloseDevice(uint DeviceType, uint DeviceInd);
    [DllImport("controlcan.dll")]
    public static extern uint VCI_InitCAN(uint DeviceType, uint DeviceInd, uint CANInd, ref InitConfig pInitConfig);
    [DllImport("controlcan.dll")]
    public static extern uint VCI_ReadBoardInfo(uint DeviceType, uint DeviceInd, ref BoardInfo pInfo);
    [DllImport("controlcan.dll")]
    public static extern uint VCI_ReadErrInfo(uint DeviceType, uint DeviceInd, uint CANInd, ref ErrorInfo pErrInfo);
    [DllImport("controlcan.dll")]
    public static extern uint VCI_ReadCANStatus(uint DeviceType, uint DeviceInd, uint CANInd, ref CanControllerStatus pCANStatus);

    [DllImport("controlcan.dll")]
    public static extern uint VCI_GetReference(uint DeviceType, uint DeviceInd, uint CANInd, uint RefType, ref byte pData);
    [DllImport("controlcan.dll")]
    public static extern uint VCI_SetReference(uint DeviceType, uint DeviceInd, uint CANInd, uint RefType, ref byte pData);

    [DllImport("controlcan.dll")]
    public static extern uint VCI_GetReceiveNum(uint DeviceType, uint DeviceInd, uint CANInd);
    [DllImport("controlcan.dll")]
    public static extern uint VCI_ClearBuffer(uint DeviceType, uint DeviceInd, uint CANInd);

    [DllImport("controlcan.dll")]
    public static extern uint VCI_StartCAN(uint DeviceType, uint DeviceInd, uint CANInd);
    [DllImport("controlcan.dll")]
    public static extern uint VCI_ResetCAN(uint DeviceType, uint DeviceInd, uint CANInd);

    [DllImport("controlcan.dll")]
    public static extern uint VCI_Transmit(uint DeviceType, uint DeviceInd, uint CANInd,ref CanObject pSend, uint Len);

    [DllImport("controlcan.dll", CharSet = CharSet.Ansi)]
    public static extern uint VCI_Receive(uint DeviceType, uint DeviceInd, uint CANInd, IntPtr pReceive, uint Len, int WaitTime);
};





