namespace IpcSessions;

public sealed class SessionCreateRequestMessage(string fromProcess, string toProcess) : Message(fromProcess, toProcess)
{
    public override string Action => "SessionCreateRequest";

    public string SendMappedFileName { get; set; }

    public string SendMutexName { get; set; }

    public string ReceiveMappedFileName { get; set; }

    public string ReceiveMutexName { get; set; }
}
