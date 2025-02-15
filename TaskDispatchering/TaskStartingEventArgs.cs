namespace A.TaskDispatching;

/// <summary>
/// 工作任务启动事件参数。
/// </summary>
/// <param name="timestamp"></param>
public sealed class TaskStartingEventArgs(DateTimeOffset timestamp) : TaskEventArgs(timestamp)
{
}
