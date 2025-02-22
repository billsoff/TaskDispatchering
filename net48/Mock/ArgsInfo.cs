namespace Mock
{
    internal readonly struct ArgsInfo
    {
        public ArgsInfo(string name, string host, int steps, bool throwError)
        {
            this.Name = name;
            this.Host = host;
            this.Steps = steps;
            this.ThrowError = throwError;
        }

        public string Name { get; }

        public string Host { get; }

        public int Steps { get; }

        public bool ThrowError { get; }
    }
}
