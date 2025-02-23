using ZLGCanComm.Interfaces;

namespace ZLGCanComm.ListenerManager;

internal record CanListenerConfig
{
    public required ICanDevice Device { get; init; }
    public required int PollingTimeout { get; init; }
    public required int WaitTime { get; init; }
}