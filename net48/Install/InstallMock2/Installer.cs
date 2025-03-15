using A.Install.Engine;

namespace InstallMock2
{
    [Install(rootDirectory: @"C:\Lisa", name: "Mock2")]
    internal sealed class Installer : InstallerBase
    {
    }
}
