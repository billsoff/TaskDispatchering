namespace A.TaskDispatching;

/// <summary>
/// 工作任务，用于具体的任务，比如起动进程。
/// </summary>
public interface ITask
{
    /// <summary>
    /// 任务名称。
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 执行任务
    /// </summary>
    void Execute();

    /// <summary>
    /// 异步执行任务。
    /// </summary>
    /// <returns></returns>
    Task ExecuteAsync() => Task.Run(Execute);

    /// <summary>
    /// 启动事件。
    /// </summary>
    event EventHandler<TaskStartingEventArgs> Starting;

    /// <summary>
    /// 状态报告事件。
    /// </summary>
    event EventHandler<TaskReportStatusEventArgs> ReportStatus;

    /// <summary>
    /// 任务完成事件。
    /// </summary>
    event EventHandler<TaskCompletedEventArgs> Completed;
}
