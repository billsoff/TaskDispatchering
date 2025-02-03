using TaskDispatching;

namespace DispatchDemo
{
    internal sealed class DemoTask(string name, int steps) : ITask
    {
        public string Name { get; } = name;

        public string Command => string.Empty;

        public string Parameter => string.Empty;

        public event EventHandler<TaskStartingEventArgs> Starting;
        public event EventHandler<TaskReportStatusEventArgs> ReportStatus;
        public event EventHandler<TaskCompletedEventArgs> Completed;

        public void Execute()
        {
            Starting?.Invoke(this, new TaskStartingEventArgs(MinashiDateTime.Now));

            Random random = new();

            for (int i = 0; i < steps; i++)
            {
                int timeUsed = random.Next(10);
                Thread.Sleep(timeUsed * 1000);

                ReportStatus?.Invoke(this, new TaskReportStatusEventArgs(MinashiDateTime.Now, $"Step {i + 1} completed."));
            }

            Completed?.Invoke(this, new TaskCompletedEventArgs(MinashiDateTime.Now));
        }

        public SchedulerTask ToSchedulerTask(TimeSpan delay = default) => new SchedulerTask(this, delay);
    }
}
