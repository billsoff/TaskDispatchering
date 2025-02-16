using System.Diagnostics;
using System.Text;

using A.TaskDispatching;

namespace A.TaskDispatchingTest;

/// <summary>
/// 工作任务，用于启动进程
/// </summary>
/// <param name="name"></param>
/// <param name="command"></param>
/// <param name="arguments"></param>
internal class WorkerTask : ITask
{
    public WorkerTask(string name, string command, string arguments, int delaySeconds = 2)
    {
        Name = name;
        Command = command;
        Arguments = arguments;

        if (delaySeconds > 0)
        {
            Thread.Sleep(delaySeconds * 1000);
        }

        CreationTime = MinashiDateTime.Now;
    }

    public string Name { get; }

    public string Command { get; }

    public string Arguments { get; }

    public DateTimeOffset CreationTime { get; }

    public event EventHandler<TaskStartingEventArgs> Starting;
    public event EventHandler<TaskReportStatusEventArgs> ReportStatus;
    public event EventHandler<TaskCompletedEventArgs> Completed;

    private readonly StringBuilder _errorMessage = new();

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

        startInfo.FileName = Command;
        startInfo.Arguments = Arguments;

        startInfo.CreateNoWindow = true;
        startInfo.UseShellExecute = false;

        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = true;

        process.OutputDataReceived += OnProcessOutputDataReceived;
        process.ErrorDataReceived += OnProcessErrorDataReceived;

        bool success = process.Start();

        if (!success)
        {
            throw new InvalidOperationException($"Start process 「{Name}」 failed.");
        }

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        process.WaitForExit();
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
}
