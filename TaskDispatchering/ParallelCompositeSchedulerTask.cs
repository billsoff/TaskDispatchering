namespace A.TaskDispatching;

/// <summary>
/// 并行调度任务，里面包含可并行执行的多个单一任务。
/// </summary>
/// <param name="primitiveSchedulerTasks"></param>
public sealed class ParallelCompositeSchedulerTask(IList<PrimitiveSchedulerTask> primitiveSchedulerTasks)
    : CompositeSchedulerTask(primitiveSchedulerTasks)
{
    /// <summary>
    /// 任务状态，取最后一个单一任务的状态。
    /// </summary>
    public override SchedulerTaskStatus Status => LastSchedulerTask.Status;

    /// <summary>
    /// 当本条任务失败时，可否执行下条任务，由组中的最后一条任务来决定。
    /// </summary>
    public override bool RunNextOnFailed => LastSchedulerTask.RunNextOnFailed;

    /// <summary>
    /// 异步执行任务，返回最后一条任务的等待对象。(其他任务的等待对象放到参数 remainderTaskCollector)
    /// </summary>
    /// <param name="remainderTaskCollector"></param>
    /// <returns></returns>
    public override Task ExecuteAsync(List<Task> remainderTaskCollector)
    {
        List<Task> all = PrimitiveSchedulerTasks.Select(t => t.ExecuteAsync()).ToList();

        remainderTaskCollector.AddRange(all[0..^1]);

        return all[^1];
    }

    /// <summary>
    /// 通知中止本条任务的执行。
    /// </summary>
    /// <returns></returns>
    public override bool Pending()
    {
        bool hasAtOneSet = false;

        foreach (SchedulerTask schedulerTask in PrimitiveSchedulerTasks)
        {
            bool result = schedulerTask.Pending();

            if (result)
            {
                hasAtOneSet = true;
            }
        }

        return hasAtOneSet;
    }

    public SchedulerTask LastSchedulerTask => PrimitiveSchedulerTasks[^1];
}
