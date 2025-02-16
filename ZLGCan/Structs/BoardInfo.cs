using System.Runtime.InteropServices;

namespace ZLGCan.Structs;

//1.ZLGCAN系列接口卡信息的数据类型。
[StructLayout(LayoutKind.Sequential)]
public struct BoardInfo
{
    public ushort HardwareVersion;           // 硬件版本号，16进制，比如0x0100表示V1.00。
    public ushort FirmwareVersion;           // 固件版本号，16进制。
    public ushort DriverVersion;             // 驱动程序版本号，16进制。
    public ushort InterfaceVersion;          // 接口库版本号，16进制。
    public ushort IrqNumber;                 // 板卡所使用的中断号。
    public byte CanChannelCount;             // CAN 通道数量,表示有几路通道。

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
    public byte[] SerialNumber;              // 板卡的序列号，比如” USBCAN V1.00”（注意：包括字符串结束符’\0’）。

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
    public byte[] HardwareType;              // 硬件类型

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    public byte[] Reserved;                  // 仅作保留，不设置。

}
