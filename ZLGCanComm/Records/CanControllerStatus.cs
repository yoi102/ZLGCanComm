namespace ZLGCanComm.Records;

//3.定义CAN控制器状态的数据类型。
public record CanControllerStatus
{
    public CanControllerStatus()
    {
    }

    /// <summary>
    ///  中断记录，读操作会清除
    /// </summary>
    public byte ErrorInterrupt { get; set; }

    /// <summary>
    /// 控制器模式寄存器
    /// </summary>
    public byte ModeRegister { get; set; }

    /// <summary>
    /// 控制器状态寄存器
    /// </summary>
    public byte StatusRegister { get; set; }

    /// <summary>
    /// 自动唤醒捕获寄存器
    /// </summary>
    public byte AutoWakeupCapture { get; set; }

    /// <summary>
    /// 错误计数捕获寄存器
    /// </summary>
    public byte ErrorCounterCapture { get; set; }

    /// <summary>
    /// 误差警告限值寄存器
    /// </summary>
    public byte ErrorWarningLimit { get; set; }

    /// <summary>
    /// 接收错误计数器
    /// </summary>
    public byte ReceiveErrorCounter { get; set; }

    /// <summary>
    /// 传输错误计数器
    /// </summary>
    public byte TransmitErrorCounter { get; set; }

    /// <summary>
    /// 系统保留字段，大小为 4 字节
    /// </summary>
    public byte[] Reserved { get; set; } = new byte[4];
}