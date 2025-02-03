namespace TaskDispatching
{
    public sealed class TaskReportStatusEventArgs(DateTimeOffset timestamp, string message) : TaskEventArgs(timestamp)
    {
        public string Message { get; } = message;
    }
}
