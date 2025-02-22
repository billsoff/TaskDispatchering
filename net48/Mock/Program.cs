using System;
using System.Threading.Tasks;

using static System.Console;
using static System.Math;

namespace Mock
{
    internal class Program
    {
        static async Task Main()
        {
            /*
             * -name:xxx
             * -host:xxx
             * -steps:xxx
             * -throwError
             */
            Random random = new Random();

            var cmdArgs = CommandArgs.Load();
            ArgsInfo argsInfo = new ArgsInfo(
                    name: cmdArgs["name"],
                    host: cmdArgs["host"],
                    steps: Min(9, Max(2, val2: cmdArgs.Get("steps", random.Next(6)))),
                    throwError: cmdArgs.IsOn("throwError")
                );

            if (argsInfo.ThrowError)
            {
                throw new InvalidOperationException("Opp");
            }

            for (int step = 0; step < argsInfo.Steps; step++)
            {
                int timeUsed = Max(1, random.Next(3));
                await Task.Delay(timeUsed * 1000);

                WriteLine($"Step {step + 1}/{argsInfo.Steps} completed.");
            }
        }
    }
}
