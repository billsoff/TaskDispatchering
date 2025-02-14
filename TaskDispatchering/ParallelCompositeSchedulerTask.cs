namespace A.TaskDispatching
{
    public sealed class ParallelCompositeSchedulerTask(IList<PrimitiveSchedulerTask> primitiveSchedulerTasks)
        : CompositeSchedulerTask(primitiveSchedulerTasks)
    {
        public override SchedulerTaskStatus Status => LastSchedulerTask.Status;

        public override bool RunNextOnFailed => LastSchedulerTask.RunNextOnFailed;

        public override Task ExecuteAsync(List<Task> remainderTaskCollector)
        {
            List<Task> all = PrimitiveSchedulerTasks.Select(t => t.ExecuteAsync()).ToList();

            remainderTaskCollector.AddRange(all[0..^1]);

            return all[^1];
        }

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
}
