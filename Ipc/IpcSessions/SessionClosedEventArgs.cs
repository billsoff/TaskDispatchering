namespace IpcSessions;

public class SessionClosedEventArgs(SessionCloseRequestMessage message) : EventArgs
{
    public SessionCloseRequestMessage Message { get; } = message;
}
