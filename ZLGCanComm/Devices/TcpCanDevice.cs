﻿using System.Net;
using ZLGCanComm.Enums;

namespace ZLGCanComm.Devices;

public class TcpCanDevice : BaseDevice
{
    private readonly string ip;
    private readonly string port;
    private bool isOpened;

    /// <summary>
    /// 适用于VCI_CANETTCP
    /// </summary>
    /// <param name="deviceIndex">设备引索</param>
    /// <param name="canIndex">CAN 通道号</param>
    public TcpCanDevice(string ip, string port, uint deviceIndex = 0, uint canIndex = 0) : base(deviceIndex, canIndex)
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

    /// <summary>
    /// 试连接设备
    /// </summary>
    /// <exception cref="InvalidOperationException">该实例被 Dispose后，调用此方法将抛出此异常</exception>
    /// <exception cref="CanDeviceOperationException">若ZLGCan的Api返回值为0时，将抛出此异常</exception>
    public override void Connect()
    {
        if (disposed)
            throw new InvalidOperationException();
        if (IsConnected)
            return;
        if (isOpened)
        {
            ZLGApi.VCI_CloseDevice(UintDeviceType, DeviceIndex);
        }
        if (ZLGApi.VCI_OpenDevice(UintDeviceType, DeviceIndex, 0) == (uint)OperationStatus.Failure)
            throw new CanDeviceOperationException();
        isOpened = true;
        if (!SetIp(DeviceIndex))
            throw new CanDeviceOperationException();
        if (!SetPort(DeviceIndex))
            throw new CanDeviceOperationException();

        base.Connect();
    }

    private static byte[] IntToBytes(uint num, int bits)
    {
        byte[] b = new byte[bits];
        for (int i = 0; i < bits; i++)
        {
            b[i] = (byte)(num >> i * 8 & 0xff);
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

        if (ZLGApi.VCI_SetReference(UintDeviceType, device_index, CanIndex, (uint)CommandType.SetDestinationIP, ref ipBytes[0]) == (uint)OperationStatus.Failure)
            return false;
        return true;
    }

    private bool SetPort(uint device_index)
    {
        uint _port = Convert.ToUInt32(port);
        byte[] ports = IntToBytes(_port, 4);

        if (ZLGApi.VCI_SetReference(UintDeviceType, device_index, CanIndex, (uint)CommandType.SetDestinationPort, ref ports[0]) == (uint)OperationStatus.Failure)
            return false;
        return true;
    }
}