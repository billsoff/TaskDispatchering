using IpcSessions;

using static System.Console;

IpcSession session = new("B");
const string connectedProcessName = "A";

session.MessageReceived += OnMessageReceived;

WriteLine("{0} is ready.", session.ProcessName);
WriteLine();

await session.CreateSessionAsync(connectedProcessName);

while (true)
{
    string text = ReadLine();

    if (string.IsNullOrWhiteSpace(text))
    {
        continue;
    }

    await session.SendMessageAsync(text, connectedProcessName);
}


static void OnMessageReceived(object sender, MessageReceivedEventArgs e)
{
    var message = Message.Load<TextMessage>(e.Data);

    WriteLine("{0}: {1} ({2:HH:mm:ss})", message.From, message.Text, message.Timestamp);
    WriteLine();
}
