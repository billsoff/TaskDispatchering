using System.Text;

namespace A.TaskDispatching
{
    public sealed class ParallelDispatcher(IEnumerable<SchedulerTask> tasks)
    {
        public async Task<string> ExecuteAsync()
        {
            throw new NotImplementedException();
            //StringBuilder buffer = new();
            //Task<string>[] all = new Task<string>[tasks.Count()];
            //int index = -1;

            //foreach (SchedulerTask task in tasks)
            //{
            //    index++;
            //    all[index] = task.ExecuteAsync();
            //}

            //string[] results = await Task.WhenAll(all);

            //index = -1;

            //foreach (SchedulerTask task in tasks)
            //{
            //    index++;
            //    buffer.AppendLine(task.Name);
            //    buffer.AppendLine(string.Empty.PadRight(50, '-'));

            //    buffer.AppendLine(results[index]);
            //}

            //return buffer.ToString();
        }
    }
}
