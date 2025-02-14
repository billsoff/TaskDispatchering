namespace A.TaskDispatching;

public class PrimitiveSchedulerTask : SchedulerTask
{
    public PrimitiveSchedulerTask(ITask task, int number, bool runNextOnFailed)
    {
        Worker = task;

        Number = number;
        RunNextOnFailed = runNextOnFailed;

        task.Starting += OnWorkerStarting;
        task.ReportStatus += OnTaskReportStatus;
        task.Completed += OnWorkerCompleted;
    }

    public event EventHandler<SchedulerTaskStatusChangedEventArgs> TaskStatusChanged;

    public event EventHandler<SchedulerTaskProgressReportedEventArgs> TaskProgressReported;

    public ITask Worker { get; }

    public override string Name => Worker.Name;

    public override int Number { get; }

    private SchedulerTaskStatus _status;

    public override SchedulerTaskStatus Status
    {
        get { return _status; }
    }

    public IList<string> Log { get; } = [];

    public Exception Error { get; private set; }

    private string _errorMessage;

    public string ErrorMessage => _errorMessage ?? Error?.ToString();

    public override bool RunNextOnFailed { get; }

    public override Task ExecuteAsync(List<Task> remainderTaskCollector) => ExecuteAsync();

    public async Task ExecuteAsync()
    {
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
    }

    public override bool Pending()
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
            timestamp = DateTime.Now;
        }

        _status = status;

        TaskStatusChanged?.Invoke(this, new SchedulerTaskStatusChangedEventArgs(timestamp, this));
    }

    private static string BuildLog(DateTimeOffset timestamp, string message) =>
        $"{(timestamp.LocalDateTime):HH:mm:ss} {message}";
}
