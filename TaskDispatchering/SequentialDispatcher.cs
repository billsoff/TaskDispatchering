using System.Text;

namespace TaskDispatching
{
    public sealed class SequentialDispatcher(IList<SchedulerTask> tasks)
    {
        public IList<SchedulerTask> Tasks { get; } = tasks;

        public async Task<string> ExecuteAsync()
        {
            StringBuilder buffer = new();
            int index = -1;

            foreach (SchedulerTask task in Tasks)
            {
                index++;

                buffer.AppendLine(task.Name);
                buffer.AppendLine(string.Empty.PadRight(50, '-'));

                buffer.AppendLine(await task.ExecuteAsync());

                if (task.Status == SchedulerTaskStatus.Failed)
                {
                    PendingRemainderTasks(index + 1);

                    break;
                }
            }

            return buffer.ToString();
        }

        private void PendingRemainderTasks(int startIndex)
        {
            for (int i = startIndex; i < Tasks.Count; i++)
            {
                Tasks[i].Pending();
            }
        }
    }
}
