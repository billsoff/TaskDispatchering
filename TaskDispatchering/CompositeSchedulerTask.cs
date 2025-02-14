namespace A.TaskDispatching;

public abstract class CompositeSchedulerTask : SchedulerTask
{
    public CompositeSchedulerTask(IList<PrimitiveSchedulerTask> schedulerTasks)
    {
        if (schedulerTasks.Count < 2)
        {
            throw new ArgumentException(
                    message: "任务必须多于2个。", 
                    paramName: nameof(schedulerTasks)
                );
        }

        PrimitiveSchedulerTasks = schedulerTasks;
    }

    public IList<PrimitiveSchedulerTask> PrimitiveSchedulerTasks { get; }
}
