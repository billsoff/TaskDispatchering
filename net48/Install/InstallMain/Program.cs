using A.InstallMain;

namespace InstallMain
{
    internal class Program
    {
        static void Main()
        {
            using (Installer installer = new Installer())
            {
                installer.Install();
            }
        }
    }
}
