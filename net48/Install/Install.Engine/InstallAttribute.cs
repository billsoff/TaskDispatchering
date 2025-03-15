using System;

namespace A.Install.Engine
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class InstallAttribute : Attribute
    {
        public InstallAttribute(string rootDirectory, string name)
        {
            RootDirectory = rootDirectory;
            Name = name;
        }

        public string RootDirectory { get; }

        public string Name { get; }

        public string AutoStartup { get; set; }
    }
}
