namespace TaskDispatching
{
    public sealed class TaskStartingEventArgs(DateTimeOffset timestamp) : TaskEventArgs(timestamp)
    {
    }
}
