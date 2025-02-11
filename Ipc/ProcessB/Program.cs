using IpcSessions;

using static System.Console;

IpcSession session = new("B");

string connectedProcessName = "A";
bool sessionClosed = false;

session.SessionClosed += OnSessionClosed;
session.TextMessageReceived += OnTextMessageReceived;

WriteLine("{0} is ready.", session.ProcessName);
WriteLine();

session.CreateSession(connectedProcessName);

WriteLine("Start session with {0} (enter bye when you want to end session)...", connectedProcessName);
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
        session.CloseSession(connectedProcessName);

        break;
    }

    if (string.IsNullOrWhiteSpace(text))
    {
        continue;
    }

    session.SendMessage(text, connectedProcessName);
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
