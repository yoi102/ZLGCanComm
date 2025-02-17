﻿using System.Runtime.InteropServices;
using ZLGCanComm.Enums;
using ZLGCanComm.Structs;

namespace ZLGCanComm.Devices;

public class UsbCan1Device : BaseDevice
{
    private InitConfig initConfig;

    public UsbCan1Device() : base(0)
    {
        initConfig = new InitConfig()
        {
            AcceptanceCode = 0x00000000, //验收码
            AcceptanceMask = 0xFFFFFFFF, //验收屏蔽码
            Filter = 1,          //滤波方式 1: 单滤波 0: 双滤波
            Mode = 0,          //0:正常模式 1:只听模式

            Timing0 = 0x01,       //通讯速率  250Kbps
            Timing1 = 0x1C,
        };
    }

    public UsbCan1Device(uint canIndex, InitConfig initConfig) : base(canIndex)
    {
        this.initConfig = initConfig;
    }

    public override uint UintDeviceType => (uint)DeviceType.VCI_USBCAN1;

    /// <summary>
    /// 尝试连接设备，如果连接上将返回True，否则返回 false
    /// </summary>
    /// <returns></returns>
    public override void Connect()
    {
        if (disposed)
            throw new InvalidOperationException();
        if (IsConnected)
            return;

        var device_index = DeviceRegistry.GetUniqueDeviceIndex(DeviceType.VCI_CANETTCP);

        if (ZLGApi.VCI_OpenDevice(UintDeviceType, device_index, 0) != (uint)OperationStatus.Success)
            throw new CanDeviceOperationException();
        var config = StructConverter.InitConfigToVCI_INIT_CONFIG(initConfig);
        if (ZLGApi.VCI_InitCAN(UintDeviceType, device_index, canIndex, ref config) != (uint)OperationStatus.Success)
            throw new CanDeviceOperationException();
        initConfig = StructConverter.VCI_INIT_CONFIGToInitConfig(config);

        ZLGApi.VCI_ClearBuffer(UintDeviceType, device_index, canIndex);

        if (ZLGApi.VCI_StartCAN(UintDeviceType, device_index, canIndex) != (uint)OperationStatus.Success)
            throw new CanDeviceOperationException();
        deviceIndex = device_index;
        IsConnected = true;
        ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(CanObject)));

        base.Connect();
    }
}