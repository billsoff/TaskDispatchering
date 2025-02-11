namespace TaskDispatching
{
    public sealed class SchedulerTask
    {
        public SchedulerTask(ITask task, Func<TimeSpan> delayEvaluator = default)
        {
            Worker = task;
            DelayEvaluator = delayEvaluator ?? (() => TimeSpan.Zero);

            task.Starting += OnWorkerStarting;
            task.ReportStatus += OnTaskReportStatus;
            task.Completed += OnWorkerCompleted;
        }

        public SchedulerTask(ITask task, TimeSpan delay = default)
            : this(task, () => delay)
        {
        }

        public event EventHandler<SchedulerTaskStatusChangedEventArgs> TaskStatusChanged;

        public event EventHandler<SchedulerTaskProgressReportedEventArgs> TaskProgressReported;

        public ITask Worker { get; }

        public Func<TimeSpan> DelayEvaluator { get; }

        public string Name => Worker.Name;

        public SchedulerTaskStatus Status { get; private set; } = SchedulerTaskStatus.Waiting;

        public IList<string> Log { get; } = [];

        public Exception Error { get; private set; }

        private string _errorMessage;

        public string ErrorMessage => _errorMessage ?? Error?.ToString();

        public async Task<string> ExecuteAsync()
        {
            TimeSpan delay = DelayEvaluator();

            if (delay > TimeSpan.Zero)
            {
                await Task.Delay(delay);
            }

            try
            {
                await Worker.ExecuteAsync();
            }
            catch (Exception ex)
            {
                DateTimeOffset timestamp = MinashiDateTime.Now;
                Error = ex;

                ChangeStatus(SchedulerTaskStatus.Failed, timestamp);
            }

            return string.Join(Environment.NewLine, Log);
        }

        public bool Pending()
        {
            if (Status != SchedulerTaskStatus.Waiting)
            {
                return false;
            }

            ChangeStatus(SchedulerTaskStatus.Pending);

            return true;
        }

        private void OnWorkerStarting(object sender, TaskStartingEventArgs e)
        {
            Log.Add(BuildLog(e.Timestamp, "Starting..."));
            ChangeStatus(SchedulerTaskStatus.Running, e.Timestamp);
        }

        private void OnTaskReportStatus(object sender, TaskReportStatusEventArgs e)
        {
            Log.Add(BuildLog(e.Timestamp, e.Message));
            TaskProgressReported?.Invoke(this, new SchedulerTaskProgressReportedEventArgs(e.Timestamp, this, e.Message));
        }

        private void OnWorkerCompleted(object sender, TaskCompletedEventArgs e)
        {
            _errorMessage = e.ErrorMessage;

            Log.Add(
                    BuildLog(
                            e.Timestamp,
                            e.Success ? "Succeeded" : $"Failed: {e.ErrorMessage}"
                        )
                );
            ChangeStatus(e.Success ? SchedulerTaskStatus.Succeeded : SchedulerTaskStatus.Failed, e.Timestamp);
        }

        private void ChangeStatus(SchedulerTaskStatus status, DateTimeOffset timestamp = default)
        {
            if (timestamp == default)
            {
                timestamp = MinashiDateTime.Now;
            }

            Status = status;

            TaskStatusChanged?.Invoke(this, new SchedulerTaskStatusChangedEventArgs(timestamp, this));
        }

        private static string BuildLog(DateTimeOffset timestamp, string message) =>
            $"{(timestamp.LocalDateTime):HH:mm:ss} {message}";
    }
}
