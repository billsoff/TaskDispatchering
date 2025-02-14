using A.TaskDispatching;

namespace A.UI.Service
{
    internal static class TaskConfigExtensions
    {
        public static TaskDispatcher BuildTaskDispatcher(this TaskConfig taskConfig)
        {
            List<SchedulerTask> taskQueue = [];
            List<PrimitiveSchedulerTask> primitiveTasks = [];

            List<PrimitiveSchedulerTask> group = [];

            foreach (TaskItem item in taskConfig.Tasks.OrderBy(t => t.Number))
            {
                PrimitiveSchedulerTask primitiveTask = BuildTask(item);
                primitiveTasks.Add(primitiveTask);

                if (group.Count != 0 && item.ShouldWait)
                {
                    if (group.Count == 1)
                    {
                        taskQueue.Add(group[0]);
                    }
                    else
                    {
                        taskQueue.Add(new ParallelCompositeSchedulerTask(group.Cast<SchedulerTask>().ToList()));
                    }

                    group = [];
                }

                group.AddRange(primitiveTasks);
            }

            if (group.Count != 0)
            {
                if (group.Count == 1)
                {
                    taskQueue.Add(group[0]);
                }
                else
                {
                    taskQueue.Add(new ParallelCompositeSchedulerTask(group.Cast<SchedulerTask>().ToList()));
                }
            }

            return new TaskDispatcher(taskQueue, primitiveTasks);
        }

        private static PrimitiveSchedulerTask BuildTask(TaskItem item)
        {
            return ConstructWorker().ArrangeScheduler(item.Number, item.RunNextOnFailed);


            WorkerTask ConstructWorker() =>
                new(
                        item.Name,
                        command: ComposeDemoPath(item.LocalPath),
                        arguments: item.Arguments
                    );

            // Demo
            static string ComposeDemoPath(string localPath) =>
                Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        @"..\..\..\..\..\Mock\bin\Debug\net8.0-windows\",
                        localPath
                    );
        }
    }
}
