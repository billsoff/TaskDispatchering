namespace A.TaskDispatching;

/// <summary>
/// 用于执行工作任务。
/// </summary>
public abstract class SchedulerTask
{
    protected SchedulerTask()
    {
    }

    /// <summary>
    /// 任务名称
    /// </summary>
    public virtual string Name => string.Empty;
    
    /// <summary>
    /// 任务编号
    /// </summary>
    public virtual int Number { get; }

    /// <summary>
    /// 获取任务状态。
    /// </summary>
    public abstract SchedulerTaskStatus Status { get; }

    /// <summary>
    /// 用于决定当本条任务失败时，下条任务是否执行（只有当下条任务等待时才有效）。
    /// </summary>
    public abstract bool RunNextOnFailed { get; }

    /// <summary>
    /// 异步执行任务
    /// </summary>
    /// <param name="remainderTaskCollector"></param>
    /// <returns></returns>
    public abstract Task ExecuteAsync(List<Task> remainderTaskCollector);

    /// <summary>
    /// 中上任务。
    /// </summary>
    /// <returns></returns>
    public abstract bool Pending();

    /// <summary>
    /// 用于确定是下条任务可否执行。
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public bool CanRunNext()
    {
        if (!IsCompleted)
        {
            throw new InvalidOperationException($"任务 {Name} 尚未完成，无法判定下条任务可否执行。");
        }

        if (Status == SchedulerTaskStatus.Pending)
        {
            return false;
        }

        return Status == SchedulerTaskStatus.Succeeded || RunNextOnFailed;
    }

    /// <summary>
    /// 确定任务是否完成。
    /// </summary>
    public bool IsCompleted => Status == SchedulerTaskStatus.Succeeded ||
                               Status == SchedulerTaskStatus.Failed ||
                               Status == SchedulerTaskStatus.Pending;
}
