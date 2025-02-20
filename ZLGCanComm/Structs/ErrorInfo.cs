namespace ZLGCanComm.Structs;

public record ErrorInfo
{
    /// <summary>
    /// 错误代码
    /// </summary>
    public uint ErrorCode { get; set; }

    /// <summary>
    /// 当产生的错误中有消极错误时表示为消极错误的错误标识数据。错误数据 1
    /// </summary>
    public byte PassiveErrorData1 { get; set; }

    /// <summary>
    /// 当产生的错误中有消极错误时表示为消极错误的错误标识数据。错误数据 2
    /// </summary>
    public byte PassiveErrorData2 { get; set; }

    /// <summary>
    /// 当产生的错误中有消极错误时表示为消极错误的错误标识数据。错误数据 3
    /// </summary>
    public byte PassiveErrorData3 { get; set; }

    /// <summary>
    /// 当产生的错误中有仲裁丢失错误时表示为仲裁丢失错误的错误标识数据。
    /// </summary>
    public byte ArbitrationLostErrorData { get; set; }
}