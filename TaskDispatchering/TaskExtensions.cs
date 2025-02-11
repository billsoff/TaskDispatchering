namespace TaskDispatching
{
    public static class TaskExtensions
    {
        public static SchedulerTask ArrangeScheduler(this ITask worker, TimeSpan delay = default) => new(worker, delay);

        public static SchedulerTask ArrangeScheduler(this ITask worker, Func<TimeSpan> delayEvaluator) => new(worker, delayEvaluator);
    }
}
