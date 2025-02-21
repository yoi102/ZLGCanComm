namespace ZLGCanComm.Records;

//5.定义初始化CAN的数据类型
public record InitConfig
{
    /// <summary>
    /// 验收码
    /// </summary>
    public uint AcceptanceCode { get; set; }

    /// <summary>
    /// 屏蔽码
    /// </summary>
    public uint AcceptanceMask { get; set; }

    /// <summary>
    /// 保留字段，通常设置为 0
    /// </summary>
    public uint Reserved { get; set; }

    /// <summary>
    /// 滤波方式
    /// </summary>
    public byte Filter { get; set; }

    /// <summary>
    /// 定时器 0 设置
    /// </summary>
    public byte Timing0 { get; set; }

    /// <summary>
    /// 定时器 1 设置
    /// </summary>
    public byte Timing1 { get; set; }

    /// <summary>
    /// 工作模式
    /// </summary>
    public byte Mode { get; set; }
}