namespace A.TaskDispatching
{
    public sealed class SchedulerTaskStatusChangedEventArgs(DateTimeOffset timestamp, SchedulerTask task)
        : SchedulerTaskEventArgs(timestamp, task)
    {
        public SchedulerTaskStatus NewStatus => Task.Status;
    }
}
