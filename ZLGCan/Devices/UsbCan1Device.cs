using System.Runtime.InteropServices;
using ZLGCan.Enums;
using ZLGCan.Structs;

namespace ZLGCan.Devices;

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

    public override bool TryConnect()
    {
        if (IsConnected)
            return true;

        var device_index = DeviceRegistry.GetUniqueDeviceIndex(DeviceType.VCI_CANETTCP);

        if (ControlCan.VCI_OpenDevice(UintDeviceType, device_index, 0) != (uint)OperationStatus.Success)
            return false;
        var config = initConfig;
        if (ControlCan.VCI_InitCAN(UintDeviceType, device_index, canIndex, ref config) != (uint)OperationStatus.Success)
            return false;
        initConfig = config;

        ControlCan.VCI_ClearBuffer(UintDeviceType, device_index, canIndex);

        if (ControlCan.VCI_StartCAN(UintDeviceType, device_index, canIndex) != (uint)OperationStatus.Success)
            return false;
        deviceIndex = device_index;
        IsConnected = true;
        ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(CanObject)));
        return true;
    }
}