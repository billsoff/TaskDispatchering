namespace A.TaskDispatching
{
    public sealed class TaskCreatedEventArgs(DateTimeOffset timestamp) : TaskEventArgs(timestamp)
    {
    }
}
