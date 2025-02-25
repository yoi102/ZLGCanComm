namespace ZLGCanComm.Api;

using System.Runtime.InteropServices;

//1.ZLGCAN系列接口卡信息的数据类型。
[StructLayout(LayoutKind.Sequential)]
public struct VCI_BOARD_INFO
{
    public ushort hw_Version;
    public ushort fw_Version;
    public ushort dr_Version;
    public ushort in_Version;
    public ushort irq_Num;
    public byte can_Num;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
    public byte[] str_Serial_Num;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
    public byte[] str_hw_Type;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    public byte[] Reserved;
}

//2.定义CAN信息帧的数据类型。
[StructLayout(LayoutKind.Sequential)]
public struct VCI_CAN_OBJ
{
    public uint ID;
    public uint TimeStamp;
    public byte TimeFlag;
    public byte SendType;
    public byte RemoteFlag; //是否是远程帧
    public byte ExternFlag; //是否是扩展帧
    public byte DataLen;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    public byte[] Data;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public byte[] Reserved;
}

//3.定义CAN控制器状态的数据类型。
[StructLayout(LayoutKind.Sequential)]
public struct VCI_CAN_STATUS
{
    public byte ErrInterrupt;
    public byte regMode;
    public byte regStatus;
    public byte regALCapture;
    public byte regECCapture;
    public byte regEWLimit;
    public byte regRECounter;
    public byte regTECounter;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public byte[] Reserved;
}

//4.定义错误信息的数据类型。
[StructLayout(LayoutKind.Sequential)]
public struct VCI_ERR_INFO
{
    public uint ErrCode;
    public byte Passive_ErrData1;
    public byte Passive_ErrData2;
    public byte Passive_ErrData3;
    public byte ArLost_ErrData;
}

//5.定义初始化CAN的数据类型
[StructLayout(LayoutKind.Sequential)]
public struct VCI_INIT_CONFIG
{
    public uint AccCode;
    public uint AccMask;
    public uint Reserved;
    public byte Filter;
    public byte Timing0;
    public byte Timing1;
    public byte Mode;
}

public static class ZLGApi
{
    [DllImport("controlcan.dll")]
    public static extern uint VCI_OpenDevice(uint DeviceType, uint DeviceInd, uint Reserved);

    [DllImport("controlcan.dll")]
    public static extern uint VCI_CloseDevice(uint DeviceType, uint DeviceInd);

    [DllImport("controlcan.dll")]
    public static extern uint VCI_InitCAN(uint DeviceType, uint DeviceInd, uint CANInd, ref VCI_INIT_CONFIG pInitConfig);

    [DllImport("controlcan.dll")]
    public static extern uint VCI_ReadBoardInfo(uint DeviceType, uint DeviceInd, ref VCI_BOARD_INFO pInfo);

    [DllImport("controlcan.dll")]
    public static extern uint VCI_ReadErrInfo(uint DeviceType, uint DeviceInd, uint CANInd, ref VCI_ERR_INFO pErrInfo);

    [DllImport("controlcan.dll")]
    public static extern uint VCI_ReadCANStatus(uint DeviceType, uint DeviceInd, uint CANInd, ref VCI_CAN_STATUS pCANStatus);

    [DllImport("controlcan.dll")]
    public static extern uint VCI_GetReference(uint DeviceType, uint DeviceInd, uint CANInd, uint RefType, byte[] pData);

    [DllImport("controlcan.dll")]
    public static extern uint VCI_SetReference(uint DeviceType, uint DeviceInd, uint CANInd, uint RefType, byte[] pData);

    [DllImport("controlcan.dll")]
    public static extern uint VCI_GetReceiveNum(uint DeviceType, uint DeviceInd, uint CANInd);

    [DllImport("controlcan.dll")]
    public static extern uint VCI_ClearBuffer(uint DeviceType, uint DeviceInd, uint CANInd);

    [DllImport("controlcan.dll")]
    public static extern uint VCI_StartCAN(uint DeviceType, uint DeviceInd, uint CANInd);

    [DllImport("controlcan.dll")]
    public static extern uint VCI_ResetCAN(uint DeviceType, uint DeviceInd, uint CANInd);

    [DllImport("controlcan.dll")]
    public static extern uint VCI_Transmit(uint DeviceType, uint DeviceInd, uint CANInd, VCI_CAN_OBJ[] pSend, uint Len);

    [DllImport("controlcan.dll", CharSet = CharSet.Ansi)]
    public static extern uint VCI_Receive(uint DeviceType, uint DeviceInd, uint CANInd, nint pReceive, uint Len, int WaitTime);
};