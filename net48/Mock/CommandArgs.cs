using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Mock
{
    public sealed class CommandArgs
    {
        public static CommandArgs Load()
        {
            CommandArgs cmdArgs = new CommandArgs();
            Dictionary<string, string> args = cmdArgs._args;

            foreach (string arg in Environment.GetCommandLineArgs().Where((_, index) => index > 0))
            {
                KeyValuePair<string, string> kvp = ParseArg(arg);

                if (kvp.Equals(default(KeyValuePair<string, string>)))
                {
                    continue;
                }

                args.Add(kvp.Key, kvp.Value);
            }

            return cmdArgs;
        }

        private static KeyValuePair<string, string> ParseArg(string arg)
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
            string value = match.Groups.Cast<Group>().Any(g => g.Name == "Value")
                           ? match.Groups["Value"].Value
                           : null;

            return new KeyValuePair<string, string>(name, value);
        }

        private static Regex GetArgPattern()
        {
            return new Regex(
                    @"^[-/]?(?<Name>\w+)(:(?<Value>.+))?$",
                    RegexOptions.ExplicitCapture | RegexOptions.Singleline | RegexOptions.Compiled
                );
        }

        private readonly Dictionary<string, string> _args;

        public CommandArgs()
        {
            _args = new Dictionary<string, string>();
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
