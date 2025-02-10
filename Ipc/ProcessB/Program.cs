using IpcSessions;

using TaskDispatching;

using static System.Console;

IpcSession session = new("B");
CancellationTokenSource tokenSource = new();

string connectedProcessName = "A";

session.SessionClosed += OnSessionClosed;
session.TextMessageReceived += OnTextMessageReceived;

WriteLine("{0} is ready.", session.ProcessName);
WriteLine();

await session.CreateSessionAsync(connectedProcessName);

WriteLine("Start session with {0} (enter exit when you want to end session)...", connectedProcessName);
WriteLine();

try
{
    await StartSessionAsync(tokenSource.Token);
    WriteLine("Session with {0} closed ({1:HH:mm:ss}).", connectedProcessName, MinashiDateTime.Now);
}
catch (OperationCanceledException)
{
}


Task StartSessionAsync(CancellationToken token) =>
    Task.Run(() =>
    {
        while (true)
        {
            string text = ReadLine();

            if (string.Equals(text, "exit", StringComparison.OrdinalIgnoreCase))
            {
                session.CloseSessionAsync(connectedProcessName).Wait();

                break;
            }

            if (string.IsNullOrWhiteSpace(text))
            {
                continue;
            }

            session.SendMessageAsync(text, connectedProcessName).Wait();
        }
    }, token);

void OnSessionClosed(object sender, SessionClosedEventArgs e)
{
    tokenSource.Cancel();

    WriteLine("Session with {0} closed ({1:HH:mm:ss}).", e.Message.From, e.Message.Timestamp);
    WriteLine();
}

static void OnTextMessageReceived(object sender, TextMessageReceivedEventArgs e)
{
    var message = e.Message;

    WriteLine("{0}: {1} ({2:HH:mm:ss})", message.From, message.Text, message.Timestamp);
    WriteLine();
}
