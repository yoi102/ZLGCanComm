using System.Runtime.InteropServices;

namespace ZLGCanComm.Structs;


[StructLayout(LayoutKind.Sequential)]
public struct ChangeDestinationIpAndPort
{
    // 密码 (最多 10 字节)
    // 更改目标 IP 和端口所需要的密码，长度小于 10，比如为“11223344”。
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
    public byte[] Password;

    // 目标 IP 地址 (最多 20 字节)
    // 所要更改的目标 IP，比如为“192.168.0.111”。
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
    public byte[] DestinationIp;

    // 目标端口
    // 所要更改的目标端口，比如为 4000。
    public int DestinationPort;
}