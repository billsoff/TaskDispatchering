namespace A.TaskDispatching;

public static class TaskExtensions
{
    /// <summary>
    /// 对工作任务安排调度。
    /// </summary>
    /// <param name="worker"></param>
    /// <param name="number"></param>
    /// <param name="runNextOnFailed"></param>
    /// <returns></returns>
    public static PrimitiveSchedulerTask ArrangeScheduler(this ITask worker, int number, bool runNextOnFailed) => new(worker, number, runNextOnFailed);
}
