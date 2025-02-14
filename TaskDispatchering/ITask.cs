namespace A.TaskDispatching
{
    public interface ITask
    {
        string Name { get; }

        void Execute();

        Task ExecuteAsync() => Task.Run(Execute);

        event EventHandler<TaskStartingEventArgs> Starting;

        event EventHandler<TaskReportStatusEventArgs> ReportStatus;

        event EventHandler<TaskCompletedEventArgs> Completed;
    }
}
