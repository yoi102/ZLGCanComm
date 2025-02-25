using System.Diagnostics.CodeAnalysis;
using ZLGCanComm.Enums;
using ZLGCanComm.Records;

namespace ZLGCanComm.Api;

public interface IZLGApi
{
    bool ClearBuffer(uint deviceType, uint deviceIndex, uint canIndex);

    bool CloseDevice(uint deviceType, uint deviceIndex);

    uint GetReceiveNum(uint deviceType, uint deviceIndex, uint canIndex);

    bool GetReference(uint deviceType, uint deviceIndex, uint canIndex, CommandType referenceType, byte[] data);

    bool InitCAN(uint deviceType, uint deviceIndex, uint canIndex, InitConfig initConfig);

    bool OpenDevice(uint deviceType, uint deviceIndex, uint reserved);

    bool ReadBoardInfo(uint deviceType, uint deviceIndex, [NotNullWhen(true)] out BoardInfo? boardInfo);

    bool ReadCANStatus(uint deviceType, uint deviceIndex, uint canIndex, [NotNullWhen(true)] out CanControllerStatus? pCANStatus);

    bool ReadErrInfo(uint deviceType, uint deviceIndex, uint canIndex, [NotNullWhen(true)] out ErrorInfo? pErrInfo);

    CanObject[] Receive(uint deviceType, uint deviceIndex, uint canIndex, uint length, int waitTime);

    bool ResetCAN(uint deviceType, uint deviceIndex, uint canIndex);

    bool SetReference(uint deviceType, uint deviceIndex, uint canIndex, CommandType referenceType, byte[] data);

    bool StartCAN(uint deviceType, uint deviceIndex, uint canIndex);

    uint Transmit(uint deviceType, uint deviceIndex, uint canIndex, CanObject[] pSend);
}