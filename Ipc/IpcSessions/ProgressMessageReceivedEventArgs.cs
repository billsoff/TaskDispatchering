namespace IpcSessions
{
    public sealed class ProgressMessageReceivedEventArgs(ProgressMessage message) : EventArgs
    {
        public ProgressMessage Message { get; } = message;
    }
}
