namespace A.TaskDispatching;

/// <summary>
/// 调度任务进度报告事件参数
/// </summary>
/// <param name="timestamp"></param>
/// <param name="task"></param>
/// <param name="message"></param>
public sealed class SchedulerTaskProgressReportedEventArgs(DateTimeOffset timestamp, SchedulerTask task, string message)
    : SchedulerTaskEventArgs(timestamp, task)
{
    public string Message { get; } = message;
}
