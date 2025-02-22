using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace A.UI
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            _ = Task.Run(StartProcess);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        private static void StartProcess()
        {
            Process process = new Process();
            ProcessStartInfo startInfo = process.StartInfo;

            startInfo.FileName = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    @"..\..\..\MessageSenderMock\bin\Debug\MessageSenderMock.exe"
                );

            process.Start();
            process.WaitForExit();
        }
    }
}