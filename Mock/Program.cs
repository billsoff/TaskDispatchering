using IpcSessions;

using Mock;

using static System.Console;
using static System.Math;

/*
 * -name:xxx
 * -host:xxx
 * -steps:xxx
 * -throwError
 */
var cmdArgs = CommandArgs.Load();
ArgsInfo argsInfo = new(
        Name: cmdArgs["name"],
        Host: cmdArgs["host"],
        Steps: Min(9, Max(1, cmdArgs.Get<int>("steps", 5))),
        ThrowError: cmdArgs.IsOn("throwError")
    );

if (argsInfo.ThrowError)
{
    throw new InvalidOperationException("Opp");
}

WriteLine(argsInfo);

IpcSession session = null;

if (!string.IsNullOrEmpty(argsInfo.Name) && !string.IsNullOrEmpty(argsInfo.Host))
{
    try
    {
        session = new IpcSession(argsInfo.Name);

        session.CreateSession(argsInfo.Host);
        WriteLine($"Session to {argsInfo.Host} (from {session.ProcessName})");
    }
    catch (Exception ex)
    {
        WriteLine(ex);
    }
}

Dictionary<int, string> ops = new()
{
    {1, "環境検査" },
    {2, "ダウンロードリクエスト" },
    {3, "ダウンロード中" },
    {4, "ダウンロード完了" },
    {5, "インストールP" },
    {6, "インストールQ" },
    {7, "インストールR" },
    {8, "インストールS" },
    {9, "インストールT" },
};

Random random = new();

for (int step = 0; step < argsInfo.Steps; step++)
{
    int timeUsed = Max(1, random.Next(3));
    await Task.Delay(timeUsed * 1000);

    WriteLine($"Step {step + 1}/{argsInfo.Steps} completed.");
    SendProgress(step + 1);
}

await session?.CloseSessionAsync(argsInfo.Host);

void SendProgress(int step)
{
    if (session == null)
    {
        return;
    }

    session.SendMessage(
            new ProgressMessage(argsInfo.Name, argsInfo.Host)
            {
                Name = ops[step],
                CurrentStep = step,
                TotalSteps = argsInfo.Steps,
            }.ToJson(),
            argsInfo.Host
        );
}
