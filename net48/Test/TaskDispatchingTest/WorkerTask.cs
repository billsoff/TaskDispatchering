﻿using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

using A.TaskDispatching;

namespace A.TaskDispatchingTest
{
    /// <summary>
    /// 工作任务，用于启动进程
    /// </summary>
    /// <param name="name"></param>
    /// <param name="command"></param>
    /// <param name="arguments"></param>
    internal class WorkerTask : TaskBase
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

        public override string Name { get; }

        public string Command { get; }

        public string Arguments { get; }

        private readonly StringBuilder _errorMessage = new StringBuilder();

        public override void Execute()
        {
            OnStarting(new TaskEventArgs(MinashiDateTime.Now));
            StartProcess();
            OnCompleted(
                    new TaskCompletedEventArgs(
                            MinashiDateTime.Now,
                            _errorMessage.Length != 0 ? _errorMessage.ToString() : null
                        )
                );
        }

        private void StartProcess()
        {
            Process process = new Process();
            ProcessStartInfo startInfo = process.StartInfo;

            startInfo.FileName = Command;
            startInfo.Arguments = $"\"{Arguments.Replace("\"", "\\\"")}\"";

            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;

            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            process.OutputDataReceived += OnProcessOutputDataReceived;
            process.ErrorDataReceived += OnProcessErrorDataReceived;

            bool success = process.Start();

            NotifyStarted();
            OnStarted(new TaskEventArgs(MinashiDateTime.Now));

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

            OnReportStatus(new TaskReportStatusEventArgs(MinashiDateTime.Now, e.Data));
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
}