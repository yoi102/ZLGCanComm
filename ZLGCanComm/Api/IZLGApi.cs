using System.Diagnostics.CodeAnalysis;
using ZLGCanComm.Records;

namespace ZLGCanComm.Api;

public interface IZLGApi
{
    bool OpenDevice(uint deviceType, uint deviceIndex, uint reserved);

    bool CloseDevice(uint deviceType, uint deviceIndex);

    bool InitCAN(uint deviceType, uint deviceIndex, uint canIndex, InitConfig initConfig);

    bool ReadBoardInfo(uint deviceType, uint deviceIndex, [NotNullWhen(true)] out BoardInfo? boardInfo);

    bool ReadErrInfo(uint deviceType, uint deviceIndex, uint canIndex, [NotNullWhen(true)] out ErrorInfo? pErrInfo);

    bool ReadCANStatus(uint deviceType, uint deviceIndex, uint canIndex, [NotNullWhen(true)] out CanControllerStatus? pCANStatus);

    bool GetReference(uint deviceType, uint deviceIndex, uint canIndex, uint RefType, byte pData);

    bool SetReference(uint deviceType, uint deviceIndex, uint canIndex, uint RefType, byte pData);

    uint GetReceiveNum(uint deviceType, uint deviceIndex, uint canIndex);

    bool ClearBuffer(uint deviceType, uint deviceIndex, uint canIndex);

    bool StartCAN(uint deviceType, uint deviceIndex, uint canIndex);

    bool ResetCAN(uint deviceType, uint deviceIndex, uint canIndex);

    uint Transmit(uint deviceType, uint deviceIndex, uint canIndex, CanObject[] pSend);

    CanObject[] Receive(uint deviceType, uint deviceIndex, uint canIndex, uint length, int waitTime);
}