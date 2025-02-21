namespace ZLGCanComm.Records;

public record ChangeDestinationIpAndPort
{
    /// <summary>
    /// 密码 (最多 10 字节)
    /// 更改目标 IP 和端口所需要的密码，长度小于 10，比如为“11223344”。
    /// </summary>
    public required byte[] Password { get; set; }

    /// <summary>
    /// 目标 IP 地址 (最多 20 字节)
    /// 所要更改的目标 IP，比如为“192.168.0.111”。
    /// </summary>
    public required byte[] DestinationIp { get; set; }

    /// <summary>
    /// 目标端口
    /// 所要更改的目标端口，比如为 4000。
    /// </summary>
    public int DestinationPort { get; set; }
}