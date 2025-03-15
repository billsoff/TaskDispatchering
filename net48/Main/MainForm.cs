using A.TaskDispatching;
using A.UI.Service;

using Serilog;

using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace A.UI
{
    public partial class MainForm : Form
    {
        private IpcReceiveSession _session;

        public MainForm()
        {
            InitializeComponent();
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            await ExecuteAsync();
        }

        private async void OnExecuteButtonClick(object sender, EventArgs e)
        {
            await ExecuteAsync();
        }

        private async Task ExecuteAsync()
        {
            int processId = Process.GetCurrentProcess().Id;
            Log.Information("[Main {ProcessId}] Tasks started...", processId);

            TaskConfig taskConfig = LoadTaskConfig();
            TaskDispatcher dispatcher = taskConfig.BuildTaskDispatcher();

            dispatcher.TaskStatusChanged += OnDispatcherTaskStatusChanged;
            dispatcher.TaskProgressReported += OnDispatcherTaskProgressReported;
            dispatcher.Completed += OnDispatcherCompleted;

            LoadTasks(dispatcher);

            _session = new IpcReceiveSession(taskConfig.ShopRow.MemoryMappedFileName);
            _session.DataReceived += OnSessionDataReceived;
            _ = _session.OpenSessionReceiveAsync();

            btnExecute.Enabled = false;

            Task schedulerTask = dispatcher.ExecuteAsync();
            await Task.Delay(8);
            Task svmTask = Task.Run(StartProcess);

            await Task.WhenAll(schedulerTask, svmTask);

            Log.Information("[Main {ProcessId}] Tasks completed.", processId);
            Log.Information(
                    "[Main {ProcessId}] Task execution report:{Report}",
                    processId,
                    ReportBuilder.Build(dispatcher, ReadTaskConfig())
                );
        }

        private void OnSessionDataReceived(object sender, SessionDataReceivedEventArgs e)
        {
            Console.WriteLine("Received data: {0}", e.Data);

            if (e.Data.Contains("11"))
            {
                IpcReceiveSession session = (IpcReceiveSession)sender;
                session.Dispose();

                Console.WriteLine("End session.");

                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "INSTALL_WAIT.txt");

                using (StreamWriter writer = new StreamWriter(path, append: false))
                {
                    writer.Write(e.Data);
                }

                Console.WriteLine("Write to INSTALL_WAIT.txt success!");
            }
        }

        private void OnDispatcherTaskStatusChanged(object sender, SchedulerTaskStatusChangedEventArgs e)
        {
            BeginInvoke((Action)(() => UpdateStatus(e.Task, e.NewStatus)));
        }

        private void OnDispatcherTaskProgressReported(object sender, SchedulerTaskProgressReportedEventArgs e)
        {
            BeginInvoke((Action)(() => UpdateOutputMessage(e.Task, e.Message)));
        }

        private void OnDispatcherCompleted(object sender, EventArgs e)
        {
            BeginInvoke(
                    (Action)(() =>
                    {
                        btnExecute.Enabled = true;
                        pbTask.Visible = false;
                    })
                );
        }

        private void UpdateStatus(SchedulerTask task, SchedulerTaskStatus newStatus)
        {
            ListViewItem item = FindItemByTask(task);
            item.SubItems[2].Text = newStatus.GetDisplayName();
        }

        private void UpdateOutputMessage(SchedulerTask task, string message)
        {
            ListViewItem item = FindItemByTask(task);
            item.SubItems[3].Text = message;

            UpdateLog(item);
        }

        private ListViewItem FindItemByTask(SchedulerTask task) => lvTasks.Items
                   .Cast<ListViewItem>()
                   .Where(item => item.Tag == task)
                   .Single();

        private void UpdateLog(ListViewItem item = null)
        {
            if (item != null && !item.Selected)
            {
                return;
            }

            item = lvTasks.SelectedItems.Cast<ListViewItem>().FirstOrDefault();
            PrimitiveSchedulerTask task = (PrimitiveSchedulerTask)item?.Tag;

            if (task != null)
            {
                txtLog.Text = string.Join(Environment.NewLine, task.Log);
            }
        }

        private void OnCloseButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        private void OnTasksListViewSelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateLog();
        }

        private void LoadTasks(TaskDispatcher dispatcher)
        {
            lvTasks.Items.Clear();

            foreach (var primitiveTask in dispatcher.PrimitiveTasks)
            {
                TaskItem configuration = primitiveTask.Configuration;
                ListViewItem item = new ListViewItem(
                        new string[]
                        {
                            primitiveTask.Number.ToString(),
                            primitiveTask.Name,
                            primitiveTask.Status.GetDisplayName(),
                            string.Empty,
                            $"Specified time: {configuration.SpecifiedTime} Interval time: {configuration.IntervalTime}",
                        }
                    )
                {
                    Tag = primitiveTask
                };

                lvTasks.Items.Add(item);
                lvTasks.Items[0].Selected = true;

                lvTasks.Focus();
                pbTask.Visible = true;
            }
        }

        private static TaskConfig LoadTaskConfig()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Service\\parma.xml");
            StreamReader reader = new StreamReader(path);

            return TaskConfig.Load(reader);
        }

        private static string ReadTaskConfig()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Service\\parma.xml");
            StreamReader reader = new StreamReader(path);

            return reader.ReadToEnd();
        }

        private static void StartProcess()
        {
            Process process = new Process();
            ProcessStartInfo startInfo = process.StartInfo;

            startInfo.FileName = FindSendSessionMockPath();

            process.Start();
            process.WaitForExit();
        }

        private static string FindSendSessionMockPath()
        {
            string[] paths =
            {
                @"SendSessionMock\MessageSenderMock.exe",
                @"..\..\..\MessageSenderMock\bin\Debug\MessageSenderMock.exe",
            };

            foreach (string path in paths)
            {
                string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);

                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }

            throw new InvalidOperationException("Send session mock program cannot be found");
        }
    }
}
