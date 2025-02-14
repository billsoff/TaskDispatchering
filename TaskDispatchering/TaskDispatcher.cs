namespace A.TaskDispatching;

public sealed class TaskDispatcher
{
    public TaskDispatcher(IList<SchedulerTask> taskQueue, IList<PrimitiveSchedulerTask> primitiveTasks)
    {
        TaskQueue = taskQueue;
        PrimitiveTasks = primitiveTasks;

        foreach (PrimitiveSchedulerTask task in PrimitiveTasks)
        {
            task.TaskStatusChanged += OnTaskStatusChanged;
            task.TaskProgressReported += OnTaskProgressReported;
        }
    }

    public IList<SchedulerTask> TaskQueue { get; }

    public IList<PrimitiveSchedulerTask> PrimitiveTasks { get; }

    public event EventHandler<SchedulerTaskStatusChangedEventArgs> TaskStatusChanged;

    public event EventHandler<SchedulerTaskProgressReportedEventArgs> TaskProgressReported;

    public event EventHandler Completed;

    public async Task ExecuteAsync()
    {
        int index = -1;
        List<Task> allRemainderTasks = [];

        foreach (SchedulerTask schedulerTask in TaskQueue)
        {
            index++;

            await schedulerTask.ExecuteAsync(allRemainderTasks);

            bool canRunNext = schedulerTask.CanRunNext();

            if (canRunNext)
            {
                continue;
            }

            PendingRemainderTasks(index + 1);

            break;
        }

        await Task.WhenAll(allRemainderTasks);

        Completed?.Invoke(this, EventArgs.Empty);
    }

    private void PendingRemainderTasks(int startIndex)
    {
        for (int i = startIndex; i < TaskQueue.Count; i++)
        {
            TaskQueue[i].Pending();
        }
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
