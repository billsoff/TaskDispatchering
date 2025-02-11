namespace TaskDispatching
{
    public sealed class TaskDispatcher_0
    {
        public SequentialDispatcher Sequential { get; init; }

        public ParallelDispatcher Parallel { get; init; }

        public async Task<string> ExecuteAsync()
        {
            List<Task<string>> tasks = [];

            if (Sequential != null)
            {
                tasks.Add(Sequential.ExecuteAsync());
            }

            if (Parallel != null)
            {
                tasks.Add(Parallel.ExecuteAsync());
            }

            string[] results = await Task.WhenAll(tasks);

            return string.Join($"{Environment.NewLine}{Environment.NewLine}", results);
        }
    }
}
