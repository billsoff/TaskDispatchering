namespace TaskDispatching
{
    public abstract class SchedulerTask
    {
        protected SchedulerTask()
        {
        }

        public virtual string Name => string.Empty;

        public virtual int Number { get; }

        public abstract SchedulerTaskStatus Status { get; }

        public abstract bool RunNextOnFailed { get; }

        public abstract Task ExecuteAsync(out IList<Task> remainderTasks);

        public abstract bool Pending();

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

        public bool IsCompleted => Status == SchedulerTaskStatus.Succeeded ||
                                   Status == SchedulerTaskStatus.Failed ||
                                   Status == SchedulerTaskStatus.Pending;
    }
}
