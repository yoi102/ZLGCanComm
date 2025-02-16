using System.Runtime.InteropServices;

namespace ZLGCanComm.Structs;
//3.定义CAN控制器状态的数据类型。
[StructLayout(LayoutKind.Sequential)]
public struct CanControllerStatus
{
    // 中断记录，读操作会清除。
    public byte ErrorInterrupt;

    // 控制器模式寄存器
    public byte ModeRegister;

    // 控制器状态寄存器
    public byte StatusRegister;

    // 自动唤醒捕获寄存器
    public byte AutoWakeupCapture;

    // 错误计数捕获寄存器
    public byte ErrorCounterCapture;

    // 误差警告限值寄存器
    public byte ErrorWarningLimit;

    // 接收错误计数器
    public byte ReceiveErrorCounter;

    // 传输错误计数器
    public byte TransmitErrorCounter;

    // 系统保留字段，大小为 4 字节
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public byte[] Reserved;
}
