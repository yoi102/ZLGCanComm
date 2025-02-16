namespace ZLGCan;
public class CanDeviceOperationException : Exception
{
    public CanDeviceOperationException() { }

    public CanDeviceOperationException(string message)
        : base(message) { }

    public CanDeviceOperationException(string message, Exception inner)
        : base(message, inner) { }
}