namespace A.TaskDispatching;

/// <summary>
/// 工作任务事件参数
/// </summary>
/// <param name="timestamp"></param>
public abstract class TaskEventArgs(DateTimeOffset timestamp) : EventArgs
{
    public DateTimeOffset Timestamp { get; } = timestamp;
}
