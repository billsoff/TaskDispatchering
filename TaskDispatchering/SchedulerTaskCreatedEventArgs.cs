namespace A.TaskDispatching
{
    public sealed class SchedulerTaskCreatedEventArgs(DateTimeOffset timestamp, SchedulerTask task)
        : SchedulerTaskEventArgs(timestamp, task)
    {
    }
}
