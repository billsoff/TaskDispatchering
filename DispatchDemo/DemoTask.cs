using TaskDispatching;

namespace DispatchDemo
{
    internal sealed class DemoTask(string name, int steps) : ITask
    {
        public string Name { get; } = name;

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

        public SchedulerTask ToSchedulerTask(TimeSpan delay = default) => new(this, delay);

        public static TaskDispatcher_0 Create() =>
            /*
            ＜DataRow>
              <param localPath="A" No="1" Go="true" startTime="" />
              <param localPath="B" No="2" Go="false" startTime="13:00:00"/>
              <param localPath="C" No="3" Go="true" startTime=""/>
              <param localPath="D" No="4" Go="true" startTime=""/>
              <param localPath="E" No="5" Go="true" interTime="00:00:10" times=2/>
            </DataRow>
            */
            new()
            {
                Sequential = new SequentialDispatcher([
                        new DemoTask("A", 3).ToSchedulerTask(),
                        new DemoTask("C", 5).ToSchedulerTask(),
                        new DemoTask("D", 7).ToSchedulerTask(),
                        new DemoTask("E", 9).ToSchedulerTask(),
                        new DemoTask("E", 9).ToSchedulerTask(TimeSpan.FromSeconds(10)),
                   ]),
                Parallel = new ParallelDispatcher([
                        new DemoTask("B", 8).ToSchedulerTask(delay: DateTime.Today + new TimeSpan(13, 0, 0) - MinashiDateTime.Now),
                    ]),
            };
    }
}
