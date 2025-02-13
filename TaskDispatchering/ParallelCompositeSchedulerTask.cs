namespace TaskDispatching
{
    public sealed class ParallelCompositeSchedulerTask(IList<SchedulerTask> schedulerTasks)
        : CompositeSchedulerTask(schedulerTasks)
    {
        public override SchedulerTaskStatus Status => LastSchedulerTask.Status;

        public override bool RunNextOnFailed => LastSchedulerTask.RunNextOnFailed;

        public override Task ExecuteAsync(out IList<Task> remainderTasks)
        {
            List<Task> all = [];

            foreach (SchedulerTask schedulerTask in SchedulerTasks)
            {
                Task currentTask = schedulerTask.ExecuteAsync(out IList<Task> others);

                all.AddRange(others);
                all.Add(currentTask);
            }

            remainderTasks = all[0..^1];

            return all[^1];
        }

        public override bool Pending()
        {
            bool hasAtOneSet = false;

            foreach (SchedulerTask schedulerTask in SchedulerTasks)
            {
                bool result = schedulerTask.Pending();

                if (result)
                {
                    hasAtOneSet = true;
                }
            }

            return hasAtOneSet;
        }

        public SchedulerTask LastSchedulerTask => SchedulerTasks[^1];
    }
}
