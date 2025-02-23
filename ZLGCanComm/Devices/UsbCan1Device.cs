using ZLGCanComm.Api;
using ZLGCanComm.Enums;
using ZLGCanComm.Records;

namespace ZLGCanComm.Devices;

public class UsbCan1Device : CanDeviceBase
{
    private readonly InitConfig initConfig;
    private bool isOpened;

    /// <summary>
    /// 适用于VCI_USBCAN1
    /// </summary>
    /// <param name="deviceIndex">设备引索</param>
    /// <param name="canIndex">CAN 通道号</param>
    public UsbCan1Device(uint deviceIndex = 0, uint canIndex = 0) : base(deviceIndex, canIndex)
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

    /// <summary>
    /// 适用于VCI_USBCAN1类型的设备
    /// </summary>
    /// <param name="initConfig">初始化设置</param>
    /// <param name="deviceIndex">设备引索</param>
    /// <param name="canIndex">CAN 通道号</param>
    public UsbCan1Device(InitConfig initConfig, uint deviceIndex = 0, uint canIndex = 0) : base(deviceIndex, canIndex)
    {
        this.initConfig = initConfig;
    }

    public override uint UintDeviceType => (uint)DeviceType.VCI_USBCAN1;

    /// <summary>
    /// 连接设备
    /// </summary>
    /// <returns></returns>
    public override void Connect()
    {
        if (isDisposed)
            throw new InvalidOperationException();
        if (IsConnected)
            return;

        if (isOpened)
        {
            ZLGApiProvider.Instance.CloseDevice(UintDeviceType, DeviceIndex);
        }

        if (!ZLGApiProvider.Instance.OpenDevice(UintDeviceType, DeviceIndex, 0))
            throw new CanDeviceOperationException();
        isOpened = true;
        if (!ZLGApiProvider.Instance.InitCAN(UintDeviceType, DeviceIndex, CanIndex, initConfig))
            throw new CanDeviceOperationException();

        ZLGApiProvider.Instance.ClearBuffer(UintDeviceType, DeviceIndex, CanIndex);

        base.Connect();
    }
}