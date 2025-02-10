namespace IpcSessions;

public sealed class TextMessageReceivedEventArgs(TextMessage message) : EventArgs
{
    public TextMessage Message { get; } = message;
}
