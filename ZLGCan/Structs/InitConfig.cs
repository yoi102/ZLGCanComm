using System.Runtime.InteropServices;

namespace ZLGCan.Structs;

//5.定义初始化CAN的数据类型
[StructLayout(LayoutKind.Sequential)]
public struct InitConfig
{
    // 验收码
    public uint AcceptanceCode;

    // 屏蔽码
    public uint AcceptanceMask;

    // 保留字段，通常设置为 0
    public uint Reserved;

    // 滤波方式
    public byte Filter;

    // 定时器 0 设置
    public byte Timing0;

    // 定时器 1 设置
    public byte Timing1;

    // 工作模式
    public byte Mode;
}