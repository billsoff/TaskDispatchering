namespace A.TaskDispatching
{
    public abstract class SchedulerTaskEventArgs(DateTimeOffset timestamp, SchedulerTask task) : TaskEventArgs(timestamp)
    {
        public SchedulerTask Task { get; } = task;
    }
}
