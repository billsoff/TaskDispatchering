namespace TaskDispatching;

public sealed class TaskDispatcher
{
    public TaskDispatcher(IList<SequentialDispatcher> sequentialDispatchers, IList<(SchedulerTask Task, int Number)> schedulerTask)
    {
        SequentialDispatchers = sequentialDispatchers;
        SchedulerTasks = schedulerTask;

        foreach (SchedulerTask task in GetSchedulerTasks())
        {
            task.TaskStatusChanged += OnTaskStatusChanged;
            task.TaskProgressReported += OnTaskProgressReported;
        }
    }

    public IList<SequentialDispatcher> SequentialDispatchers { get; }

    public IList<(SchedulerTask Task, int Number)> SchedulerTasks { get; }

    public event EventHandler<SchedulerTaskStatusChangedEventArgs> TaskStatusChanged;

    public event EventHandler<SchedulerTaskProgressReportedEventArgs> TaskProgressReported;

    public event EventHandler Completed;

    public IEnumerable<SchedulerTask> GetSchedulerTasks() =>
        SequentialDispatchers
        .SelectMany(d => d.Tasks)
        .OrderBy(t => t.Name);

    public async Task<string> ExecuteAsync()
    {
        List<Task<string>> tasks = SequentialDispatchers
                                   .Select(s => s.ExecuteAsync())
                                   .ToList();

        string[] log = await Task.WhenAll(tasks);

        Completed?.Invoke(this, EventArgs.Empty);

        return string.Join(Environment.NewLine, log);
    }

    private void OnTaskStatusChanged(object sender, SchedulerTaskStatusChangedEventArgs e)
    {
        TaskStatusChanged?.Invoke(this, e);
    }

    private void OnTaskProgressReported(object sender, SchedulerTaskProgressReportedEventArgs e)
    {
        TaskProgressReported?.Invoke(this, e);
    }
}
