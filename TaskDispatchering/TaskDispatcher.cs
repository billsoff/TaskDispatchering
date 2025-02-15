namespace A.TaskDispatching;

/// <summary>
/// 任务总调度器(对应于配置文件)
/// </summary>
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

    /// <summary>
    /// 调度任务对列。
    /// </summary>
    public IList<SchedulerTask> TaskQueue { get; }

    /// <summary>
    /// 单一调度任务列表。
    /// </summary>
    public IList<PrimitiveSchedulerTask> PrimitiveTasks { get; }

    /// <summary>
    /// 调度任务状态变化事件
    /// </summary>
    public event EventHandler<SchedulerTaskStatusChangedEventArgs> TaskStatusChanged;

    /// <summary>
    /// 任务进度报告事件
    /// </summary>
    public event EventHandler<SchedulerTaskProgressReportedEventArgs> TaskProgressReported;

    /// <summary>
    /// 任务完成事件
    /// </summary>
    public event EventHandler Completed;

    /// <summary>
    /// 异步执行任务列表。
    /// </summary>
    /// <returns></returns>
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
