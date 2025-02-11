using DispatchDemo;

using TaskDispatching;

const string config = """
＜DataRow>
  <param localPath="A" No="1" Go="true" startTime="" />
  <param localPath="B" No="2" Go="false" startTime="13:00:00"/>
  <param localPath="C" No="3" Go="true" startTime=""/>
  <param localPath="D" No="4" Go="true" startTime=""/>
  <param localPath="E" No="5" Go="true" interTime="00:00:10" times=2/>
</DataRow>
""";
Console.WriteLine(config);
Console.WriteLine();

DateTime now = DateTime.Today + new TimeSpan(12, 59, 0);
MinashiDateTime.Offset = now - DateTime.Now;

TaskDispatcher_0 dispatcher = new()
{
    Sequential = new SequentialDispatcher([
            new ProcessTask("A", 2).ToSchedulerTask(),
            new ProcessTask("C", 3).ToSchedulerTask(),
            new ProcessTask("D", 4).ToSchedulerTask(),
            new ProcessTask("E", 5).ToSchedulerTask(),
            new ProcessTask("E", 5).ToSchedulerTask(TimeSpan.FromSeconds(10)),
       ]),
    Parallel = new ParallelDispatcher([
            new ProcessTask("B", 6, throwError: true).ToSchedulerTask(delay: DateTime.Today + new TimeSpan(13, 0, 0) - MinashiDateTime.Now),
        ]),
};

string output = await dispatcher.ExecuteAsync();

Console.WriteLine();
Console.WriteLine(output);

Console.WriteLine("Press any key to exit...");
Console.ReadKey(intercept: true);
