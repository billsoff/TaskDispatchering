using A.Install.Engine;

namespace A.InstallMain
{
    [Install(rootDirectory: @"C:\Lisa", name: "Main", AutoStartup = "Main.exe")]
    internal sealed class Installer : InstallerBase
    {
    }
}
