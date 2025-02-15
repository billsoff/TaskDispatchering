namespace A.TaskDispatching;

/// <summary>
/// 复合任务调度器，用于对任务进行分组。
/// </summary>
public abstract class CompositeSchedulerTask : SchedulerTask
{
    public CompositeSchedulerTask(IList<PrimitiveSchedulerTask> schedulerTasks)
    {
        if (schedulerTasks.Count == 0)
        {
            throw new ArgumentException(
                    message: "至少应包含1个任务。", 
                    paramName: nameof(schedulerTasks)
                );
        }

        PrimitiveSchedulerTasks = schedulerTasks;
    }

    /// <summary>
    /// 单一任务调度器列表。
    /// </summary>
    public IList<PrimitiveSchedulerTask> PrimitiveSchedulerTasks { get; }

    public override string ToString() => string.Join(", ", PrimitiveSchedulerTasks);
}
