using Mock;

using static System.Console;
using static System.Math;

/*
 * -name:xxx
 * -steps:xxx
 * -throwError
 */
var cmdArgs = CommandArgs.Load();
ArgsInfo argsInfo = new(
        Name: cmdArgs["name"],
        Steps: Max(1, cmdArgs.Get<int>("steps", 5)),
        ThrowError: cmdArgs.IsOn("throwError")
    );

if (argsInfo.ThrowError)
{
    throw new InvalidOperationException("Opp");
}

Random random = new();

for (int step = 0; step < argsInfo.Steps; step++)
{
    int timeUsed = Max(1, random.Next(3));
    await Task.Delay(timeUsed * 1000);

    WriteLine($"Step {step + 1}/{argsInfo.Steps} completed.");
}
