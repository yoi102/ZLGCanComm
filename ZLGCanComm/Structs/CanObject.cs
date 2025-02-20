namespace ZLGCanComm.Structs;

/////////////////////////////////////////////////////
//2.定义CAN信息帧的数据类型。
public record CanObject
{
    public CanObject()
    {
        Data = new byte[8];
        DataLength = (byte)Data.Length;
        Reserved = new byte[3];
    }

    /// <summary>
    /// CAN 帧 ID
    /// </summary>
    public uint Id { get; set; }

    /// <summary>
    /// 时间戳，接收到信息帧时的时间标识，从 CAN 控制器初始化开始计时。仅在接收帧时有效
    /// </summary>
    public uint TimeStamp { get; set; }

    /// <summary>
    /// 是否使用时间标识。为 1 时，TimeStamp 有效。仅在接收帧时有效
    /// </summary>
    public byte TimeFlag { get; set; }

    /// <summary>
    /// 发送类型：0 - 正常发送，1 - 单次发送，2 - 自发自收，3 - 单次自发自收，仅在发送帧时有意义
    /// </summary>
    public byte SendType { get; set; }

    /// <summary>
    /// 是否是远程帧
    /// </summary>
    public byte RemoteFlag { get; set; }

    /// <summary>
    /// 是否是扩展帧
    /// </summary>
    public byte ExternFlag { get; set; }

    /// <summary>
    /// 数据长度，最大为 8 字节
    /// </summary>
    public byte DataLength { get; set; }

    /// <summary>
    /// CAN 帧的数据，最多 8 字节
    /// </summary>
    public byte[] Data { get; set; }

    /// <summary>
    /// 系统保留字段，大小为 3 字节
    /// </summary>
    public byte[] Reserved { get; set; }
}