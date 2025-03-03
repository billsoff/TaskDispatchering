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
        private static string _name;
        private static bool _mockError;

        static void Main(string[] args)
        {
            LogService.LoadConfiguration();

            GetMockInfo(args.Length != 0 ? args[0] : null);
            int processId = Process.GetCurrentProcess().Id;

            Random random = new Random();

            // 接收 JSON 参数
            if (args.Length == 1)
            {
                Console.WriteLine("Accept arguments:");
                Console.WriteLine(JToken.Parse(args[0]).ToString(Formatting.None));
            }

            Log.Information("[{MyName} {ProcessId}] starting...", _name, processId);

            if (_mockError)
            {
                Thread.Sleep(Math.Max(3, random.Next(8)) * 1000);

                throw new InvalidOperationException("Oop!");
            }

            Thread.Sleep(1000 * Math.Max(2, random.Next(6)));

            Log.Information("[{MyName} {ProcessId}] completed.", _name, processId);

            LogService.Close();
        }

        private static void GetMockInfo(string parameter)
        {
            try
            {
                JObject o = JObject.Parse(parameter);
                int number = Convert.ToInt32(o["IntervalTime"].ToString()) - 2;

                char me = (char)('A' + number);

                _name = me.ToString();

                const string MOCK_ERROR = "mockError";
                _mockError = o.ContainsKey(MOCK_ERROR) && 
                             bool.TryParse(o[MOCK_ERROR].ToString(), out bool mockError) && 
                             mockError;
            }
            catch
            {
                _name = "Mock2";
            }
        }
    }
}
