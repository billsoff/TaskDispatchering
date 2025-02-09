namespace IpcSessions;

public sealed class MessageReceivedEventArgs(string data) : EventArgs
{
    public string Data { get; } = data;
}
