using IpcSessions;

using static System.Console;

IpcSession session = new("A", canBeConnected: true);

string connectedProcessName = null;
bool sessionClosed = false;

session.SessionCreated += OnSessionCreated;
session.SessionClosed += OnSessionClosed;

session.TextMessageReceived += OnTextMessageReceived;

WriteLine("{0} is ready.", session.ProcessName);
WriteLine();

while (true)
{
    string text = ReadLine();

    if (sessionClosed)
    {
        break;
    }

    if (string.Equals(text, "bye", StringComparison.OrdinalIgnoreCase))
    {
        await session.CloseSessionAsync(connectedProcessName);

        break;
    }

    if (string.IsNullOrWhiteSpace(text))
    {
        continue;
    }

    await session.SendMessageAsync(text, connectedProcessName);
}


void OnSessionCreated(object sender, SessionCreatedEventArgs e)
{
    WriteLine("{0} connected ({1:HH:mm:ss}).", e.Message.From, e.Message.Timestamp);
    WriteLine();

    WriteLine("Start session with {0} (enter bye when you want to end session)...", e.Message.From);

    connectedProcessName = e.Message.From;
}

void OnSessionClosed(object sender, SessionClosedEventArgs e)
{
    sessionClosed = true;

    WriteLine("Session with {0} closed ({1:HH:mm:ss}).", e.Message.From, e.Message.Timestamp);
    WriteLine();
}

static void OnTextMessageReceived(object sender, TextMessageReceivedEventArgs e)
{
    var message = e.Message;

    WriteLine("{0}: {1} ({2:HH:mm:ss})", message.From, message.Text, message.Timestamp);
    WriteLine();
}
