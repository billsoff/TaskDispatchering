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
