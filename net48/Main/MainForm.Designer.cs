using System.Drawing;
using System.Windows.Forms;

namespace A.UI
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel1 = new Panel();
            pbTask = new ProgressBar();
            btnClose = new Button();
            btnExecute = new Button();
            splitContainer1 = new SplitContainer();
            lvTasks = new ListView();
            taskNumber = new ColumnHeader();
            taskName = new ColumnHeader();
            taskStatus = new ColumnHeader();
            taskMessage = new ColumnHeader();
            taskProgress = new ColumnHeader();
            txtLog = new TextBox();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(pbTask);
            panel1.Controls.Add(btnClose);
            panel1.Controls.Add(btnExecute);
            panel1.Dock = DockStyle.Bottom;
            panel1.Location = new Point(0, 400);
            panel1.Margin = new Padding(2, 3, 2, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(724, 42);
            panel1.TabIndex = 0;
            // 
            // pbTask
            // 
            pbTask.Enabled = false;
            pbTask.Location = new Point(189, 10);
            pbTask.Name = "pbTask";
            pbTask.Size = new Size(511, 23);
            pbTask.Style = ProgressBarStyle.Marquee;
            pbTask.TabIndex = 2;
            pbTask.Visible = false;
            // 
            // btnClose
            // 
            btnClose.Location = new Point(97, 8);
            btnClose.Margin = new Padding(2, 3, 2, 3);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(73, 25);
            btnClose.TabIndex = 1;
            btnClose.Text = "閉じる";
            btnClose.UseVisualStyleBackColor = true;
            btnClose.Click += OnCloseButtonClick;
            // 
            // btnExecute
            // 
            btnExecute.Location = new Point(9, 8);
            btnExecute.Margin = new Padding(2, 3, 2, 3);
            btnExecute.Name = "btnExecute";
            btnExecute.Size = new Size(73, 25);
            btnExecute.TabIndex = 0;
            btnExecute.Text = "実行";
            btnExecute.UseVisualStyleBackColor = true;
            btnExecute.Click += OnExecuteButtonClick;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Margin = new Padding(2, 3, 2, 3);
            splitContainer1.Name = "splitContainer1";
            splitContainer1.Orientation = Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(lvTasks);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(txtLog);
            splitContainer1.Size = new Size(724, 400);
            splitContainer1.SplitterDistance = 232;
            splitContainer1.SplitterWidth = 3;
            splitContainer1.TabIndex = 2;
            // 
            // lvTasks
            // 
            lvTasks.Activation = ItemActivation.OneClick;
            lvTasks.Columns.AddRange(new ColumnHeader[] { taskNumber, taskName, taskStatus, taskMessage, taskProgress });
            lvTasks.Dock = DockStyle.Fill;
            lvTasks.FullRowSelect = true;
            lvTasks.GridLines = true;
            lvTasks.HeaderStyle = ColumnHeaderStyle.Nonclickable;
            lvTasks.Location = new Point(0, 0);
            lvTasks.Margin = new Padding(2, 3, 2, 3);
            lvTasks.MultiSelect = false;
            lvTasks.Name = "lvTasks";
            lvTasks.Size = new Size(724, 232);
            lvTasks.TabIndex = 0;
            lvTasks.UseCompatibleStateImageBehavior = false;
            lvTasks.View = View.Details;
            lvTasks.SelectedIndexChanged += OnTasksListViewSelectedIndexChanged;
            // 
            // taskNumber
            // 
            taskNumber.Text = "番号";
            // 
            // taskName
            // 
            taskName.Text = "タスク名";
            taskName.Width = 120;
            // 
            // taskStatus
            // 
            taskStatus.Text = "ステータス";
            taskStatus.Width = 120;
            // 
            // taskMessage
            // 
            taskMessage.Text = "メッセージ";
            taskMessage.Width = 300;
            // 
            // taskProgress
            // 
            taskProgress.Text = "進捗";
            taskProgress.Width = 300;
            // 
            // txtLog
            // 
            txtLog.Dock = DockStyle.Fill;
            txtLog.Location = new Point(0, 0);
            txtLog.Margin = new Padding(2, 3, 2, 3);
            txtLog.Multiline = true;
            txtLog.Name = "txtLog";
            txtLog.ReadOnly = true;
            txtLog.ScrollBars = ScrollBars.Vertical;
            txtLog.Size = new Size(724, 165);
            txtLog.TabIndex = 0;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(724, 442);
            Controls.Add(splitContainer1);
            Controls.Add(panel1);
            Margin = new Padding(2, 3, 2, 3);
            Name = "MainForm";
            Text = "タスク実行機";
            panel1.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private SplitContainer splitContainer1;
        private ListView lvTasks;
        private ColumnHeader taskNumber;
        private TextBox txtLog;
        private Button btnExecute;
        private Button btnClose;
        private ColumnHeader taskName;
        private ColumnHeader taskStatus;
        private ColumnHeader taskMessage;
        private ColumnHeader taskProgress;
        private ProgressBar pbTask;
    }
}
