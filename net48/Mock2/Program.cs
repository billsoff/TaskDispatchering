using A.UI.Service;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Serilog;

using System;
using System.Diagnostics;
using System.Threading;

namespace Mock2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            LogService.LoadConfiguration();

            string myName = GetMyName(args.Length != 0 ? args[0] : null);
            int processId = Process.GetCurrentProcess().Id;

            Random random = new Random();

            // 接收 JSON 参数
            if (args.Length == 1)
            {
                Console.WriteLine("Accept arguments:");
                Console.WriteLine(JToken.Parse(args[0]).ToString(Formatting.None));
            }

            Log.Information("[{MyName} {ProcessId}] starting...", myName, processId);

            if (random.Next(5) == 0)
            {
                throw new InvalidOperationException("Oop!");
            }

            Thread.Sleep(1000 * Math.Max(2, random.Next(6)));

            Log.Information("[{MyName} {ProcessId}] completed.", myName, processId);

            LogService.Close();
        }

        private static string GetMyName(string parameter)
        {
            try
            {
                JObject o = JObject.Parse(parameter);
                int number = Convert.ToInt32(o["IntervalTime"].ToString()) - 2;

                char me = (char)('A' + number);

                return me.ToString();
            }
            catch
            {
                return "Mock2";
            }
        }
    }
}
