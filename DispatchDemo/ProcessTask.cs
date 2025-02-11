using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;

using TaskDispatching;

namespace DispatchDemo
{
    internal class ProcessTask(string name, int steps, bool throwError = false) : ITask
    {
        public string Name { get; } = name;

        public int Steps { get; } = steps;

        public bool ThrowError { get; } = throwError;

        private readonly StringBuilder _errorMessage = new();

        public event EventHandler<TaskStartingEventArgs> Starting;
        public event EventHandler<TaskReportStatusEventArgs> ReportStatus;
        public event EventHandler<TaskCompletedEventArgs> Completed;

        public void Execute()
        {
            Starting?.Invoke(this, new TaskStartingEventArgs(MinashiDateTime.Now));
            StartProcess();
            Completed?.Invoke(
                    this,
                    new TaskCompletedEventArgs(
                            MinashiDateTime.Now,
                            _errorMessage.Length != 0 ? _errorMessage.ToString() : null
                        )
                );
        }

        private void StartProcess()
        {
            Process process = new();
            ProcessStartInfo startInfo = process.StartInfo;

            startInfo.FileName = GetMockFileName();

            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            LoadArguments(startInfo.ArgumentList);

            process.OutputDataReceived += OnProcessOutputDataReceived;
            process.ErrorDataReceived += OnProcessErrorDataReceived;

            bool success = process.Start();

            if (!success)
            {
                throw new InvalidOperationException("Start process failed.");
            }

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();


            static string GetMockFileName() =>
                Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        @"..\..\..\..\Mock\bin\Debug\net8.0\Mock.exe"
                    );
        }

        private void LoadArguments(Collection<string> arguments)
        {
            arguments.Add($"-name:{Name}");
            arguments.Add($"-steps:{Steps}");

            if (ThrowError)
            {
                arguments.Add("-throwError");
            }
        }

        private void OnProcessOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data))
            {
                return;
            }

            ReportStatus?.Invoke(this, new TaskReportStatusEventArgs(MinashiDateTime.Now, e.Data));
        }

        private void OnProcessErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Data))
            {
                return;
            }

            _errorMessage.AppendLine(e.Data);
        }

        public SchedulerTask ToSchedulerTask(TimeSpan delay = default) => new(this, delay);
    }
}
