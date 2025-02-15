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
            task.TaskCreated += OnTaskCreated;
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
    /// 任务创建事件
    /// </summary>
    public event EventHandler<SchedulerTaskCreatedEventArgs> TaskCreated;

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
    /// 异步创建所有任务。
    /// </summary>
    /// <returns></returns>
    public Task CreateTasksAsync()
    {
        return Task.Run(DoCreateTasks);

        void DoCreateTasks()
        {
            foreach (PrimitiveSchedulerTask schedulerTask in PrimitiveTasks)
            {
                schedulerTask.Worker.Create();
            }
        }
    }

    /// <summary>
    /// 异步执行任务列表。
    /// </summary>
    /// <returns></returns>
    public async Task ExecuteAsync()
    {
        List<Task> allRemainderTasks = [];
        bool canRunNext = true;

        foreach (SchedulerTask schedulerTask in TaskQueue)
        {
            if (!canRunNext)
            {
                schedulerTask.Pending();

                continue;
            }

            await schedulerTask.ExecuteAsync(allRemainderTasks);
            canRunNext = schedulerTask.CanRunNext();
        }

        await Task.WhenAll(allRemainderTasks);

        Completed?.Invoke(this, EventArgs.Empty);
    }

    private void OnTaskCreated(object sender, SchedulerTaskCreatedEventArgs e)
    {
        TaskCreated?.Invoke(this, e);
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
