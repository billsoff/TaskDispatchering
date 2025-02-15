namespace A.TaskDispatching;

/// <summary>
/// 调度任务状态变化事件参数
/// </summary>
/// <param name="timestamp"></param>
/// <param name="task"></param>
public sealed class SchedulerTaskStatusChangedEventArgs(DateTimeOffset timestamp, SchedulerTask task)
    : SchedulerTaskEventArgs(timestamp, task)
{
    public SchedulerTaskStatus NewStatus => Task.Status;
}
