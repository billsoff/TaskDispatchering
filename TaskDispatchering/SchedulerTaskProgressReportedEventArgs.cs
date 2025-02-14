namespace A.TaskDispatching
{
    public sealed class SchedulerTaskProgressReportedEventArgs(DateTimeOffset timestamp, SchedulerTask task, string message)
        : SchedulerTaskEventArgs(timestamp, task)
    {
        public string Message { get; } = message;
    }
}
