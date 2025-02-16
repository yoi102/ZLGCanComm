using System.Net;
using System.Runtime.InteropServices;
using ZLGCan.Enums;
using ZLGCan.Structs;

namespace ZLGCan.Devices;

public class TcpCanDevice : BaseDevice
{
    private readonly string ip;

    private readonly string port;

    public TcpCanDevice(string ip, string port, uint canIndex = 0) : base(canIndex)
    {
        if (!IsValidPort(port))
            throw new ArgumentException(nameof(port));
        if (!IsValidIp(ip))
            throw new ArgumentException(nameof(ip));

        this.ip = ip;
        this.port = port;
    }

    public override uint UintDeviceType => (uint)DeviceType.VCI_CANETTCP;

    public static bool IsValidPort(string port)
    {
        // 检查字符串是否为空或只包含空白字符
        if (string.IsNullOrWhiteSpace(port))
        {
            return false;
        }

        // 尝试将字符串转换为整数
        if (uint.TryParse(port, out _))
        {
            return true;
        }

        // 如果转换失败或不在范围内，返回 false
        return false;
    }

    public override bool TryConnect()
    {
        if (IsConnected)
            return true;

        var device_index = DeviceRegistry.GetUniqueDeviceIndex(DeviceType.VCI_CANETTCP);

        if (ControlCan.VCI_OpenDevice(UintDeviceType, device_index, 0) != (uint)OperationStatus.Success)
            return false;

        if (!SetIp(device_index))
            return false;
        if (!SetPort(device_index))
            return false;

        if (ControlCan.VCI_StartCAN(UintDeviceType, device_index, canIndex) != (uint)OperationStatus.Success)
            return false;
        deviceIndex = device_index;
        IsConnected = true;
        ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(CanObject)));

        return true;
    }

    private static byte[] IntToBytes(uint num, int bits)
    {
        byte[] b = new byte[bits];
        for (int i = 0; i < bits; i++)
        {
            b[i] = (byte)((num >> (i * 8)) & 0xff);
        }
        return b;
    }

    private static bool IsValidIp(string ip)
    {
        if (string.IsNullOrWhiteSpace(ip))
        {
            return false;
        }
        return IPAddress.TryParse(ip, out _);
    }

    private bool SetIp(uint device_index)
    {
        char[] ipChars = ip.ToCharArray();
        byte[] ipBytes = new byte[50];
        for (int i = 0; i < ipChars.Length; ++i)
        {
            ipBytes[i] = Convert.ToByte(ipChars[i]);
        }

        if (ControlCan.VCI_SetReference(UintDeviceType, device_index, canIndex, (uint)CommandType.SetDestinationIP, ref ipBytes[0]) != (uint)OperationStatus.Success)
            return false;
        return true;
    }

    private bool SetPort(uint device_index)
    {
        uint _port = Convert.ToUInt32(port);
        byte[] ports = IntToBytes(_port, 4);

        if (ControlCan.VCI_SetReference(UintDeviceType, device_index, canIndex, (uint)CommandType.SetSourcePort, ref ports[0]) != (uint)OperationStatus.Success)
            return false;
        return true;
    }
}