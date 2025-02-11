namespace TaskDispatching
{
    public static class SchedulerTaskStatusExtensions
    {
        public static string GetDisplayName(this SchedulerTaskStatus taskStatus) =>
            taskStatus switch
            {
                SchedulerTaskStatus.Waiting => "待機",
                SchedulerTaskStatus.Running => "実行中",
                SchedulerTaskStatus.Succeeded => "正常終了",
                SchedulerTaskStatus.Failed => "異常終了",
                SchedulerTaskStatus.Pending => "中止",
                _ => throw new InvalidOperationException($"Cannot recognize scheduler task status: {taskStatus}")
            };
    }
}
