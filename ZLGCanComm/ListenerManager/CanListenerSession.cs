using ZLGCanComm.Records;

namespace ZLGCanComm.ListenerManager;
internal record CanListenerSession
{
    public required Task Task { get; init; }
    public required HashSet<Action<CanObject[]>> CallBacks { get; init; }
    public required CancellationTokenSource CancellationTokenSource { get; init; }

    public void InvokeAll(CanObject[] newCanObject, SynchronizationContext? syncContext)
    {
        foreach (var callback in CallBacks)
        {
            if (syncContext != null)
            {
                syncContext.Post(_ => callback(newCanObject), null);
            }
            else
            {
                callback(newCanObject);
            }
        }
    }
}