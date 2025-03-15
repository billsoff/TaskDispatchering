using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;

using Serilog;

namespace A.Install.Engine
{
    public abstract class InstallerBase : IDisposable
    {
        public void Install()
        {
            InitializeLog();

            string[] args = Environment.GetCommandLineArgs();

            if (args != null)
            {
                Log.Information("Command args: {Args}", string.Join(" ", args));
            }

            try
            {
                Log.Information("Start install...");
                DoInstall();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Install failed");

                Console.WriteLine();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey(intercept: true);
            }
        }

        private void DoInstall()
        {
            Type type = GetType();
            InstallInfo info = new InstallInfo(type);

            string target = Path.Combine(info.RootDirectory, info.Name);
            Log.Information("Prepare deploy \"{Name}\" to \"{Location}\"", info.Name, target);

            using (Stream stream = info.Read())
            {
                if (Directory.Exists(target))
                {
                    Log.Information("Folder \"{Location}\" exists. Prepare to delete...", target);
                    Directory.Delete(target, recursive: true);
                    Log.Information("Folder \"{Location}\" was deleted", target);
                }

                Log.Information("Prepare extract \"{Name}.zip\" to \"{Location}\"", info.Name, target);

                ZipArchive archive = new ZipArchive(stream, ZipArchiveMode.Read);
                archive.ExtractToDirectory(info.RootDirectory);

                Log.Information("\"{Name}.zip\" was extracted to \"{Location}\"", info.Name, target);
            }

            if (info.EnableAutoStartup)
            {
                Log.Information("Try to startup \"{StartupPath}\"", info.StartupPath);
                info.Startup();
                Log.Information("Startup \"{StartupPath}\" succeeded", info.StartupPath);
            }

            Log.Information("Install succeeded at \"{Location}\"", target);
            Thread.Sleep(3000);
        }

        private void InitializeLog()
        {
            Log.Logger = new LoggerConfiguration()
                         .MinimumLevel.Verbose()
                         .WriteTo.Console()
                         .WriteTo.File($"logs/install-.txt", rollingInterval: RollingInterval.Day)
                         .CreateLogger();
        }

        public void Dispose()
        {
            Log.CloseAndFlush();
            GC.SuppressFinalize(this);
        }
    }

    internal sealed class InstallInfo
    {
        private readonly Type _installerType;

        public InstallInfo(Type installerType)
        {
            _installerType = installerType;

            InstallAttribute installAttr = (InstallAttribute)Attribute.GetCustomAttribute(installerType, typeof(InstallAttribute))
                                           ?? throw new InvalidOperationException($"Type {installerType} should flag attribute {nameof(InstallAttribute)}");

            RootDirectory = installAttr.RootDirectory;
            Name = installAttr.Name;
            AutoStartup = installAttr.AutoStartup;
        }

        public string RootDirectory { get; private set; }

        public string Name { get; private set; }

        public string AutoStartup { get; }

        public Stream Read()
        {
            Stream stream = _installerType.Assembly.GetManifestResourceStream(_installerType, $"{Name}.zip")
                            ?? throw new InvalidOperationException($"Deploy {Name} not found");

            return stream;
        }

        public bool EnableAutoStartup => !string.IsNullOrEmpty(AutoStartup);

        public string StartupPath => Path.Combine(RootDirectory, Name, AutoStartup);

        public void Startup()
        {
            if (!EnableAutoStartup)
            {
                return;
            }

            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = StartupPath,
            };

            Process.Start(startInfo);
        }
    }
}
