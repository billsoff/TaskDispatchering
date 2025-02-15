using A.TaskDispatching;
using A.UI.Service;

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
