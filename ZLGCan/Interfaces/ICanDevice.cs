using ZLGCan.Enums;
using ZLGCan.Structs;

namespace ZLGCan.Interfaces;

public interface ICanDevice
{
    DeviceType DeviceType { get; }
    bool IsConnected { get; }
    int ListenWaiteTime { get; }

    void Listen(Action<(uint, byte[])> onChange, uint length = 1, int waitTime = 0);

    void Close();

    bool TryConnect();

    CanControllerStatus ReadCanControllerStatus();

    ErrorInfo ReadErrorInfo();

    (uint id, byte[] message) ReadMessage(uint length = 1, int waitTime = 0);

    CanObject WriteMessage(CanObject canObject, uint length = 1);

    CanObject WriteMessage(uint id, byte[] message, uint length = 1);

    bool TryReadCanControllerStatus(out CanControllerStatus status);

    bool TryReadErrorInfo(out ErrorInfo errorInfo);

    bool TryReadMessage(out uint id, out byte[] message, uint length = 1, int waitTime = 0);

    bool TryWriteMessage(ref CanObject canObject, uint length = 1);

    bool TryWriteMessage(uint id, byte[] message, uint length = 1);
}