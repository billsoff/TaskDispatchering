namespace A.TaskDispatching;

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

    public IList<PrimitiveSchedulerTask> PrimitiveSchedulerTasks { get; }

    public override string ToString() => string.Join(", ", PrimitiveSchedulerTasks);
}
