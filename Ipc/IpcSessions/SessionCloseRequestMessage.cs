namespace IpcSessions;

public sealed class SessionCloseRequestMessage(string fromProcess, string toProcess) : Message(fromProcess, toProcess)
{
    public override string Action => MessageTypes.SessionCloseRequest;
}
