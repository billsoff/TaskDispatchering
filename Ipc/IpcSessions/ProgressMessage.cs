namespace IpcSessions
{
    public sealed class ProgressMessage(string fromProcess, string toProcess)
        : Message(fromProcess, toProcess)
    {
        public override string Action => MessageTypes.Progress;

        public string Name { get; set; }

        public int CurrentStep { get; set; }

        public int TotalSteps { get; set; }
    }
}
