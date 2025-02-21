namespace ZLGCanComm.Records;

//1.ZLGCAN系列接口卡信息的数据类型。
public record BoardInfo
{
    public BoardInfo()
    {

    }
    /// <summary>
    /// 硬件版本号，16进制，比如0x0100表示V1.00。
    /// </summary>
    public ushort HardwareVersion { get; set; }

    /// <summary>
    /// 固件版本号，16进制。
    /// </summary>
    public ushort FirmwareVersion { get; set; }

    /// <summary>
    /// 驱动程序版本号，16进制。
    /// </summary>
    public ushort DriverVersion { get; set; }

    /// <summary>
    /// 接口库版本号，16进制。
    /// </summary>
    public ushort InterfaceVersion { get; set; }

    /// <summary>
    /// 板卡所使用的中断号。
    /// </summary>
    public ushort IrqNumber { get; set; }

    /// <summary>
    /// CAN 通道数量,表示有几路通道。
    /// </summary>
    public byte CanChannelCount { get; set; }

    /// <summary>
    /// 板卡的序列号，比如” USBCAN V1.00”（注意：包括字符串结束符’\0’）。
    /// </summary>
    public byte[] SerialNumber { get; set; } = new byte[20];

    /// <summary>
    /// 硬件类型
    /// </summary>
    public byte[] HardwareType { get; set; } = new byte[40];

    /// <summary>
    /// 系统保留字段，大小为 4 字节
    /// </summary>
    public byte[] Reserved { get; set; } = new byte[8];
}