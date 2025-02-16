using System.Runtime.InteropServices;

namespace ZLGCanComm.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct ErrorInfo
{
    // 错误代码
    public uint ErrorCode;

    // 当产生的错误中有消极错误时表示为消极错误的错误标识数据。错误数据 1
    public byte PassiveErrorData1;

    // 当产生的错误中有消极错误时表示为消极错误的错误标识数据。错误数据 2
    public byte PassiveErrorData2;

    // 当产生的错误中有消极错误时表示为消极错误的错误标识数据。错误数据 3
    public byte PassiveErrorData3;

    // 当产生的错误中有仲裁丢失错误时表示为仲裁丢失错误的错误标识数据。
    public byte ArbitrationLostErrorData;
}

