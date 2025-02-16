namespace A.TaskDispatching;

/// <summary>
/// 工作任务事件参数
/// </summary>
/// <param name="timestamp"></param>
public abstract class TaskEventArgs(DateTimeOffset timestamp) : EventArgs
{
    public DateTimeOffset Timestamp { get; } = timestamp;
}

/// <summary>
/// 工作任务启动事件参数。
/// </summary>
/// <param name="timestamp"></param>
public sealed class TaskStartingEventArgs(DateTimeOffset timestamp) : TaskEventArgs(timestamp)
{
}

/// <summary>
/// 工作任务状态报告事件参数
/// </summary>
/// <param name="timestamp"></param>
/// <param name="message"></param>
public sealed class TaskReportStatusEventArgs(DateTimeOffset timestamp, string message) : TaskEventArgs(timestamp)
{
    public string Message { get; } = message;
}

/// <summary>
/// 工作任务完成事件参数。
/// </summary>
/// <param name="timestamp"></param>
/// <param name="errorMessage"></param>
public sealed class TaskCompletedEventArgs(DateTimeOffset timestamp, string errorMessage = null) : TaskEventArgs(timestamp)
{
    public bool Success => string.IsNullOrEmpty(errorMessage);

    public string ErrorMessage => errorMessage;
}

/// <summary>
/// 调度任务事件参数
/// </summary>
/// <param name="timestamp"></param>
/// <param name="task"></param>
public abstract class SchedulerTaskEventArgs(DateTimeOffset timestamp, SchedulerTask task) : TaskEventArgs(timestamp)
{
    public SchedulerTask Task { get; } = task;
}

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
