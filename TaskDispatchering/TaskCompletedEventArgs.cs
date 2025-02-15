namespace A.TaskDispatching;

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
