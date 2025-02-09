namespace IpcSessions;

public sealed class SessionCreatedEventArgs(SessionCreateRequestMessage message) : EventArgs
{
    public SessionCreateRequestMessage Message { get; } = message;
}
