using TaskDispatching;

namespace ClodPos.Service
{
    internal static class TaskConfigExtensions
    {
        public static TaskDispatcher BuildTaskDispatcher(this TaskConfig taskConfig)
        {
            List<SequentialDispatcher> dispatchers = [];

            List<(SchedulerTask Task, int Number)> all = [];
            List<SchedulerTask> group = [];

            foreach (TaskItem item in taskConfig.Tasks.OrderBy(t => t.Number))
            {
                IList<SchedulerTask> tasks = BuildTask(item);

                if (group.Count != 0 && !item.ShouldWait)
                {
                    dispatchers.Add(new SequentialDispatcher(group));
                    group = [];
                }

                group.AddRange(tasks);

                foreach (SchedulerTask task in tasks)
                {
                    all.Add((task, item.Number));
                }
            }

            if (group.Count != 0)
            {
                dispatchers.Add(new SequentialDispatcher(group));
            }

            return new TaskDispatcher(dispatchers, all);
        }

        private static List<SchedulerTask> BuildTask(TaskItem item)
        {
            List<SchedulerTask> tasks = [];
            tasks.Add(ConstructWorker().ArrangeScheduler(Delay));

            if (item.Times > 1)
            {
                for (int i = 0; i < item.Times - 1; i++)
                {
                    tasks.Add(
                            ConstructWorker()
                            .ArrangeScheduler(
                                    TimeSpan.FromSeconds(Math.Max(3, item.IntervalTime.TotalSeconds))
                                )
                        );
                }
            }

            return tasks;


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

            TimeSpan Delay()
            {
                if (!item.IsStartTimeSpecified)
                {
                    return TimeSpan.Zero;
                }

                TimeSpan delay = item.StartTime - MinashiDateTime.Now;

                if (delay < TimeSpan.Zero)
                {
                    delay += TimeSpan.FromHours(24);
                }

                return delay;
            }
        }
    }
}
