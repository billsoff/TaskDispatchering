namespace A.TaskDispatching
{
    public abstract class TaskEventArgs(DateTimeOffset timestamp) : EventArgs
    {
        public DateTimeOffset Timestamp { get; } = timestamp;
    }
}
