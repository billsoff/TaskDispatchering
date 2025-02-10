namespace IpcSessions;

public sealed class TextMessage(string fromProcess, string toProcess) : Message(fromProcess, toProcess)
{
    public override string Action => MessageTypes.Text;

    public string Text { get; set; }
}
