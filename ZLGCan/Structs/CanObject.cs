using System.Runtime.InteropServices;

namespace ZLGCan.Structs;

/////////////////////////////////////////////////////
//2.定义CAN信息帧的数据类型。
[StructLayout(LayoutKind.Sequential)]
public struct CanObject
{
    // CAN 帧 ID
    public uint Id;

    // 时间戳，接收到信息帧时的时间标识，从 CAN 控制器初始化开始计时。仅在接收帧时有效
    public uint TimeStamp;

    // 是否使用时间标识。为 1 时，TimeStamp 有效。仅在接收帧时有效
    public byte TimeFlag;

    // 发送类型：0 - 正常发送，1 - 单次发送，2 - 自发自收，3 - 单次自发自收，仅在发送帧时有意义
    public byte SendType;

    // 是否是远程帧
    public byte RemoteFlag;

    // 是否是扩展帧
    public byte ExternFlag;

    // 数据长度，最大为 8 字节
    public byte DataLength;

    // CAN 帧的数据，最多 8 字节
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    public byte[] Data;

    // 系统保留字段，大小为 3 字节
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public byte[] Reserved;
}

