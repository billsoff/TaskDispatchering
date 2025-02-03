using System.Text;

namespace TaskDispatching
{
    public sealed class SequentialDispatcher(IEnumerable<SchedulerTask> tasks)
    {
        public async Task<string> ExecuteAsync()
        {
            StringBuilder buffer = new();

            foreach (SchedulerTask task in tasks)
            {
                buffer.AppendLine(task.Name);
                buffer.AppendLine(string.Empty.PadRight(50, '-'));

                buffer.AppendLine(await task.ExecuteAsync());
            }

            return buffer.ToString();
        }
    }
}
