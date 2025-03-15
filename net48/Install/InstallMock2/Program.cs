namespace InstallMock2
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
