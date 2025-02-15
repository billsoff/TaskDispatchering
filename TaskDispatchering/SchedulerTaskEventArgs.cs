namespace A.TaskDispatching;

/// <summary>
/// 调度任务事件参数
/// </summary>
/// <param name="timestamp"></param>
/// <param name="task"></param>
public abstract class SchedulerTaskEventArgs(DateTimeOffset timestamp, SchedulerTask task) : TaskEventArgs(timestamp)
{
    public SchedulerTask Task { get; } = task;
}
