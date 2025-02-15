namespace A.TaskDispatching;

/// <summary>
/// 单一调度任务，负责一条工作任务的执行。
/// </summary>
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

    /// <summary>
    /// 任务状态变化事件。
    /// </summary>
    public event EventHandler<SchedulerTaskStatusChangedEventArgs> TaskStatusChanged;

    /// <summary>
    /// 任务进度报告事件。
    /// </summary>
    public event EventHandler<SchedulerTaskProgressReportedEventArgs> TaskProgressReported;

    /// <summary>
    /// 工作任务，用于执行具体的任务，比如起动进程。
    /// </summary>
    public ITask Worker { get; }

    /// <summary>
    /// 任务名称。
    /// </summary>
    public override string Name => Worker.Name;

    /// <summary>
    /// 任务编号。
    /// </summary>
    public override int Number { get; }

    private SchedulerTaskStatus _status;

    /// <summary>
    /// 任务状态。
    /// </summary>
    public override SchedulerTaskStatus Status
    {
        get { return _status; }
    }

    /// <summary>
    /// 任务执行日志。
    /// </summary>
    public IList<string> Log { get; } = [];

    /// <summary>
    /// 错误。
    /// </summary>
    public Exception Error { get; private set; }

    private string _errorMessage;

    /// <summary>
    /// 错误消息。
    /// </summary>
    public string ErrorMessage => _errorMessage ?? Error?.ToString();

    /// <summary>
    /// 当本条任务失败进，是否执行下条任务。
    /// </summary>
    public override bool RunNextOnFailed { get; }

    /// <summary>
    /// 异步执行任务。
    /// </summary>
    /// <param name="remainderTaskCollector"></param>
    /// <returns></returns>
    public override Task ExecuteAsync(List<Task> remainderTaskCollector) => ExecuteAsync();

    /// <summary>
    /// 异步执行任务。
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// 中上任务。
    /// </summary>
    /// <returns></returns>
    public override bool Pending()
    {
        if (Status != SchedulerTaskStatus.Waiting)
        {
            return false;
        }

        ChangeStatus(SchedulerTaskStatus.Pending);

        return true;
    }

    public override string ToString() => Name;

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
