using Newtonsoft.Json;

using System;
using System.IO;
using System.Threading.Tasks;

namespace MessageSenderMock
{
    internal class Program
    {
        static async Task Main()
        {
            Console.WriteLine("SWM started.");

            const int TIMES = 14;
            Random random = new Random();
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "session.txt");

            IpcSendSession session = new IpcSendSession(filePath, "hello");

            for (int i = 0; i < TIMES; i++)
            {
                await Task.Delay(Math.Max(2, random.Next(6)) * 1000);

                Message message = new Message() { Step = i + 1 };
                string data = JsonConvert.SerializeObject(message);

                await session.SendAsync(data);

                Console.WriteLine("Send data: {0}", data);
            }

            await session.SendAsync(string.Empty);
            session.Dispose();
        }
    }

    internal sealed class Message
    {
        public int Step { get; set; }
    }
}
