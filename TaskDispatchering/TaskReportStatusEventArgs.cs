namespace A.TaskDispatching;

/// <summary>
/// 工作任务状态报告事件参数
/// </summary>
/// <param name="timestamp"></param>
/// <param name="message"></param>
public sealed class TaskReportStatusEventArgs(DateTimeOffset timestamp, string message) : TaskEventArgs(timestamp)
{
    public string Message { get; } = message;
}
