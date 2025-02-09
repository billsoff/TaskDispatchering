using IpcSessions;

using static System.Console;

IpcSession session = new("A", canBeConnected: true);
string connectedProcessName = null;

session.SessionCreated += OnSessionCreated;
session.MessageReceived += OnMessageReceived;

WriteLine("{0} is ready.", session.ProcessName);
WriteLine();

while (true)
{
    string text = ReadLine();

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

    connectedProcessName = e.Message.From;
}

static void OnMessageReceived(object sender, MessageReceivedEventArgs e)
{
    var message = Message.Load<TextMessage>(e.Data);

    WriteLine("{0}: {1} ({2:HH:mm:ss})", message.From, message.Text, message.Timestamp);
    WriteLine();
}
