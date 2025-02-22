using A.TaskDispatching;
using A.UI.Service;

using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace A.UI
{
    public partial class MainForm : Form
    {
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
            DateTime now = DateTime.Today + new TimeSpan(12, 59, 40);
            MinashiDateTime.Offset = now - DateTime.Now;

            TaskConfig taskConfig = LoadTaskConfig();
            TaskDispatcher dispatcher = taskConfig.BuildTaskDispatcher();

            dispatcher.TaskStatusChanged += OnDispatcherTaskStatusChanged;
            dispatcher.TaskProgressReported += OnDispatcherTaskProgressReported;
            dispatcher.Completed += OnDispatcherCompleted;

            LoadTasks(dispatcher);

            //using IpcSession session = new(taskConfig.SchedulerProcessName, canBeConnected: true);
            //session.ProgressMessageReceived += OnSessionProgressMessageReceived;

            btnExecute.Enabled = false;

            await dispatcher.ExecuteAsync();
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

        //private void OnSessionProgressMessageReceived(object sender, ProgressMessageReceivedEventArgs e)
        //{
        //    BeginInvoke(() => UpdateProgress(e.Message));
        //}

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

        //private void UpdateProgress(ProgressMessage message)
        //{
        //    ListViewItem item = lvTasks.Items
        //                        .Cast<ListViewItem>()
        //                        .Where(i => ((SchedulerTask)i.Tag).Name == message.From &&
        //                                    ((SchedulerTask)i.Tag).Status != SchedulerTaskStatus.Waiting &&
        //                                    ((SchedulerTask)i.Tag).Status != SchedulerTaskStatus.Succeeded &&
        //                                    ((SchedulerTask)i.Tag).Status != SchedulerTaskStatus.Failed)
        //                        .FirstOrDefault();

        //    if (item == null)
        //    {
        //        return;
        //    }

        //    item.SubItems[4].Text = $"{message.Timestamp:HH:mm:ss} {message.CurrentStep}/{message.TotalSteps}: {message.Name}";
        //}

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
                ListViewItem item = new ListViewItem(
                        new string[]
                        {
                            primitiveTask.Number.ToString(),
                            primitiveTask.Name,
                            primitiveTask.Status.GetDisplayName(),
                            string.Empty,
                            string.Empty,
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
    }
}
