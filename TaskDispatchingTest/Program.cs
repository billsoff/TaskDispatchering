using A.TaskDispatching;
using A.TaskDispatchingTest;

using static System.Console;

WriteLine("Test starting...");
WriteLine();

(TaskDispatcher Dispatcher, string FileName, string Config)[] dispatcherInfoItems = LoadTaskDispatchers();
TaskDispatcher[] allDispatchers = dispatcherInfoItems.Select(item => item.Dispatcher).ToArray();

Task[] tasks = dispatcherInfoItems
               .Select(item => item.Dispatcher.ExecuteAsync())
               .ToArray();

foreach (var (Dispatcher, _, _) in dispatcherInfoItems)
{
    Dispatcher.Completed += OnDispatcherCompleted;
}

await Task.WhenAll(tasks);

WriteLine();
WriteLine("All completed.");
WriteLine();

Write("Generate test results... ");
GenerateTestResults(dispatcherInfoItems);

WriteLine("OK.");
WriteLine();

WriteLine("Test completed.");
WriteLine();


void OnDispatcherCompleted(object sender, EventArgs e)
{
    int index = Array.IndexOf(allDispatchers, (TaskDispatcher)sender);
    WriteLine("{0} completed.", Path.GetFileName(dispatcherInfoItems[index].FileName));
}

static (TaskDispatcher Dispatcher, string FileName, string Config)[] LoadTaskDispatchers()
{
    string[] fileNames = GetAllTestFiles();
    var dispatchers = new (TaskDispatcher Dispatcher, string FileName, string Config)[fileNames.Length];

    for (int i = 0; i < fileNames.Length; i++)
    {
        string fileName = fileNames[i];
        string config = ReadConfig(fileName);
        TaskDispatcher taskDispatcher = TaskConfig
                                        .Load(new StringReader(config))
                                        .BuildTaskDispatcher();

        dispatchers[i] = (taskDispatcher, fileName, config);
    }

    return dispatchers;
}

static string[] GetAllTestFiles() =>
    Directory.GetFiles(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestConfig"),
            "*.xml"
        );

static string ReadConfig(string fileName)
{
    using StreamReader reader = new(fileName);

    return reader.ReadToEnd();
}

static void GenerateTestResults((TaskDispatcher Dispatcher, string FileName, string Config)[] dispatchers)
{
    using StreamWriter writer = new(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestResults.txt"),
            append: false
        );

    string ruler = string.Empty.PadRight(80, '-');

    foreach (var (Dispatcher, FileName, Config) in dispatchers)
    {
        writer.WriteLine(Path.GetFileName(FileName));
        writer.WriteLine(ruler);

        writer.WriteLine(Config);
        writer.WriteLine();

        OutputSchedulerTasks(Dispatcher, writer);

        writer.WriteLine(ruler);
        writer.WriteLine();
    }
}

static void OutputSchedulerTasks(TaskDispatcher dispatcher, StreamWriter writer)
{
    OutputSummary(dispatcher, writer);
    OutputDetail(dispatcher, writer);
}

static void OutputSummary(TaskDispatcher dispatcher, StreamWriter writer)
{
    IList<string> log;
    string ruler = string.Empty.PadRight(140, '-');

    writer.WriteLine(ruler);

    foreach (CompositeSchedulerTask schedulerTask in dispatcher.TaskQueue.Cast<CompositeSchedulerTask>())
    {
        for (int i = 0; i < schedulerTask.PrimitiveSchedulerTasks.Count; i++)
        {
            if (i != 0)
            {
                writer.Write("  ");
            }

            PrimitiveSchedulerTask primitive = schedulerTask.PrimitiveSchedulerTasks[i];
            log = primitive.Log;

            writer.Write(
                    "{0}. {1} {2:HH:mm:ss} created. {3}", 
                    primitive.Number,
                    primitive.Name, 
                    primitive.CreationTime,
                    primitive.Status.GetDisplayName()
                );

            if (log.Count == 0)
            {
                continue;
            }

            if (log.Count >= 1)
            {
                writer.Write(" ({0} - ", primitive.Log[1][0..8]);
            }

            if (log.Count >= 2)
            {
                writer.Write("{0})", primitive.Log[^1][0..8]);
            }
            else if (log.Count >= 1)
            {
                writer.Write(")");
            }
        }

        writer.WriteLine();
    }

    writer.WriteLine(ruler);
    writer.WriteLine();
}

static void OutputDetail(TaskDispatcher dispatcher, StreamWriter writer)
{
    string ruler = string.Empty.PadRight(30, '-');

    foreach (PrimitiveSchedulerTask schedulerTask in dispatcher.PrimitiveTasks)
    {
        writer.WriteLine(
                "{0}. {1} {2}",
                schedulerTask.Number,
                schedulerTask.Name,
                schedulerTask.Status.GetDisplayName()
            );
        writer.WriteLine(ruler);

        foreach (string log in schedulerTask.Log)
        {
            writer.WriteLine(log);
        }

        if (schedulerTask.Log.Count != 0)
        {
            writer.WriteLine(ruler);
        }

        writer.WriteLine();
    }
}