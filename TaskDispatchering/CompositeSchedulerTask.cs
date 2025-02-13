namespace TaskDispatching
{
    public abstract class CompositeSchedulerTask : SchedulerTask
    {
        public CompositeSchedulerTask(IList<SchedulerTask> schedulerTasks)
        {
            if (schedulerTasks.Count < 2)
            {
                throw new ArgumentException(
                        message: "任务必须多于2个。", 
                        paramName: nameof(schedulerTasks)
                    );
            }

            SchedulerTasks = schedulerTasks;
        }

        public IList<SchedulerTask> SchedulerTasks { get; }
    }
}
