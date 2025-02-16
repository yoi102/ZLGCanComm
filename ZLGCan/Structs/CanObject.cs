using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace ZLGCan.Structs;

/////////////////////////////////////////////////////
//2.定义CAN信息帧的数据类型。
[StructLayout(LayoutKind.Sequential)]
public struct CanObject
{
    public CanObject()
    {
        Data = new byte[8];
        Reserved = new byte[3];
    }
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

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is not CanObject canObject)
            return false;

        if (this.Id != canObject.Id)
            return false;
        if (this.TimeStamp != canObject.TimeStamp)
            return false;
        return this.Data.SequenceEqual(canObject.Data);

    }
    public static bool operator ==(CanObject left, CanObject right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(CanObject left, CanObject right)
    {
        return !(left == right);
    }

    public override readonly int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + Id.GetHashCode();
            hash = hash * 23 + TimeStamp.GetHashCode();
            hash = hash * 23 + TimeFlag.GetHashCode();
            hash = hash * 23 + SendType.GetHashCode();
            hash = hash * 23 + RemoteFlag.GetHashCode();
            hash = hash * 23 + ExternFlag.GetHashCode();
            hash = hash * 23 + DataLength.GetHashCode();
            hash = hash * 23 + Data.Aggregate(0, (acc, b) => acc * 23 + b.GetHashCode());
            hash = hash * 23 + Reserved.Aggregate(0, (acc, b) => acc * 23 + b.GetHashCode());
            return hash;
        }
    }
}

