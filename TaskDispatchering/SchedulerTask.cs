using System.Text;

namespace TaskDispatching
{
    public sealed class SchedulerTask(ITask task, TimeSpan delay = default)
    {
        public TimeSpan Delay { get; } = delay;

        public ITask Worker { get; } = task;

        public string Name => Worker.Name;

        public async Task<string> ExecuteAsync()
        {
            if (Delay > TimeSpan.Zero)
            {
                await Task.Delay(Delay);
            }

            StringBuilder buffer = new();

            try
            {
                Worker.Starting += OnWorkerStarting;
                Worker.ReportStatus += OnWorkerReportStatus;
                Worker.Completed += OnWorkerCompleted;

                await Worker.ExecuteAsync();
            }
            finally
            {
                Worker.Starting -= OnWorkerStarting;
                Worker.ReportStatus -= OnWorkerReportStatus;
                Worker.Completed -= OnWorkerCompleted;
            }

            return buffer.ToString();


            void OnWorkerStarting(object sender, TaskStartingEventArgs e)
            {
                buffer.AppendLine(BuildLog(e.Timestamp, "Starting..."));
            }

            void OnWorkerReportStatus(object sender, TaskReportStatusEventArgs e)
            {
                buffer.AppendLine(BuildLog(e.Timestamp, e.Message));
            }

            void OnWorkerCompleted(object sender, TaskCompletedEventArgs e)
            {
                buffer.AppendLine(
                        BuildLog(
                                e.Timestamp,
                                e.Success ? "Succeeded" : $"Failed: {e.ErrorMessage}"
                            )
                    );

                Console.WriteLine($"{((ITask)sender).Name} completed.");
            }
        }

        private static string BuildLog(DateTimeOffset timestamp, string message) =>
            $"{(timestamp.LocalDateTime):HH:mm:ss} {message}";
    }
}
