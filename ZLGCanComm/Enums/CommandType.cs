namespace ZLGCanComm.Enums;
public enum CommandType
{
    SetDestinationIP = 0,
    SetDestinationPort = 1,
    SetSourcePort = 2,
    ChangeDestinationIPAndPort = 2, // 保留值为2，但可以考虑是否需要修改
    TcpMode = 4 // TCP 工作方式：服务器:1 或 客户端:0
}
