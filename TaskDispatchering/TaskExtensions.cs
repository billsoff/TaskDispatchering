namespace A.TaskDispatching
{
    public static class TaskExtensions
    {
        public static PrimitiveSchedulerTask ArrangeScheduler(this ITask worker, int number, bool runNextOnFailed) => new(worker, number, runNextOnFailed);
    }
}
