namespace A.TaskDispatching;

/// <summary>
/// 调度任务状态
/// </summary>
public enum SchedulerTaskStatus
{
    /// <summary>
    /// 待機
    /// </summary>
    Waiting,

    /// <summary>
    /// 実行中
    /// </summary>
    Running,

    /// <summary>
    /// 正常終了
    /// </summary>
    Succeeded,

    /// <summary>
    /// 異常終了
    /// </summary>
    Failed,

    /// <summary>
    /// 中止
    /// </summary>
    Pending,
}

public static class SchedulerTaskStatusExtensions
{
    /// <summary>
    /// 获取任务状态显示名称。
    /// </summary>
    /// <param name="taskStatus"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static string GetDisplayName(this SchedulerTaskStatus taskStatus) =>
        taskStatus switch
        {
            SchedulerTaskStatus.Waiting => "待機",
            SchedulerTaskStatus.Running => "実行中",
            SchedulerTaskStatus.Succeeded => "正常終了",
            SchedulerTaskStatus.Failed => "異常終了",
            SchedulerTaskStatus.Pending => "中止",
            _ => throw new InvalidOperationException($"Cannot recognize scheduler task status: {taskStatus}")
        };
}
