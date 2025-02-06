using System.Text.RegularExpressions;

namespace Mock
{
    public sealed partial class CommandArgs
    {
        public static CommandArgs Load()
        {
            CommandArgs cmdArgs = new();
            Dictionary<string, string> args = cmdArgs._args;

            foreach (string arg in Environment.GetCommandLineArgs()[1..])
            {
                KeyValuePair<string, string> kvp = ParseArg(arg);

                if (kvp.Equals(default(KeyValuePair<string, string>)))
                {
                    continue;
                }

                args.Add(kvp.Key, kvp.Value);
            }

            return cmdArgs;


            static KeyValuePair<string, string> ParseArg(string arg)
            {
                if (string.IsNullOrEmpty(arg))
                {
                    return default;
                }

                Regex pattern = GetArgPattern();
                Match match = pattern.Match(arg);

                if (!match.Success)
                {
                    return default;
                }

                string name = match.Groups["Name"].Value;
                string value = match.Groups.ContainsKey("Value")
                               ? match.Groups["Value"].Value
                               : null;

                return new KeyValuePair<string, string>(name, value);
            }
        }

        [GeneratedRegex(@"^[-/]?(?<Name>\w+)(:(?<Value>.+))?$", RegexOptions.ExplicitCapture | RegexOptions.Singleline)]
        private static partial Regex GetArgPattern();

        private readonly Dictionary<string, string> _args;

        public CommandArgs()
        {
            _args = [];
        }

        public string this[string name]
        {
            get => _args.TryGetValue(name, out string value) ? value : null;
            set => _args[name] = value;
        }

        public T Get<T>(string name, T defaultValue = default)
        {
            if (string.IsNullOrEmpty(this[name]))
            {
                return defaultValue;
            }

            try
            {
                return (T)Convert.ChangeType(this[name], typeof(T));
            }
            catch
            {
                return defaultValue;
            }
        }

        public bool IsOn(string name) => _args.ContainsKey(name);
    }
}
