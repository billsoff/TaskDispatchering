using A.TaskDispatching;

namespace A.UI.Service;

public static class TaskConfigExtensions
{
    /// <summary>
    /// 从配置文件构建任务执行器(任务总调度)
    /// </summary>
    /// <param name="taskConfig"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    public static TaskDispatcher BuildTaskDispatcher(this TaskConfig taskConfig)
    {
        if (taskConfig.Tasks.Length == 0)
        {
            throw new ArgumentException(message: "应至少指定一个任务", paramName: nameof(taskConfig));
        }

        List<SchedulerTask> taskQueue = [];
        List<PrimitiveSchedulerTask> primitiveTasks = [];

        List<PrimitiveSchedulerTask> group = [];

        foreach (TaskItem item in taskConfig.Tasks.OrderBy(t => t.Number))
        {
            PrimitiveSchedulerTask primitiveTask = BuildTask(item, taskConfig);
            primitiveTasks.Add(primitiveTask);

            // 当本条任务需要等待前一条任务时，生成一个组.  group.Count != 0 第一条不参与分组
            if (item.ShouldWait && group.Count != 0)
            {
                // 需要等待的。 ref内部修改会传到外部
                taskQueue.Add(BuildGroup(ref group));
            }

            group.Add(primitiveTask);
        }
        // 最后一组
        taskQueue.Add(BuildGroup(ref group));

        return new TaskDispatcher(taskQueue, primitiveTasks);


        static SchedulerTask BuildGroup(ref List<PrimitiveSchedulerTask> group)
        {
            group[0].Delay = TimeSpan.FromSeconds(2);

            for (int i = 1; i < group.Count; i++)
            {
                // 后续任务延迟 5 秒启动
                group[i].Delay = TimeSpan.FromSeconds(5 * i);
            }

            SchedulerTask schedulerTask = new ParallelCompositeSchedulerTask(group);

            // 创建一个新的组
            group = [];

            return schedulerTask;
        }
    }

    private static PrimitiveSchedulerTask BuildTask(TaskItem item, TaskConfig taskConfig)
    {
        return ConstructWorker().ArrangeScheduler(item.Number, item.RunNextOnFailed);


        WorkerTask ConstructWorker() =>
            new(
                    item.Name,
                    command: ComposeDemoPath(item.LocalPath),
                    argument: item.GetTaskArgument(taskConfig)
                );

        // Demo
        static string ComposeDemoPath(string localPath) =>
            Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    @"..\..\..\..\Mock2\bin\Debug\net8.0-windows\",
                    localPath
                );
    }
}
