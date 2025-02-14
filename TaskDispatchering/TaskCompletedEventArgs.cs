namespace A.TaskDispatching
{
    public sealed class TaskCompletedEventArgs(DateTimeOffset timestamp, string errorMessage = null) : TaskEventArgs(timestamp)
    {
        public bool Success => string.IsNullOrEmpty(errorMessage);

        public string ErrorMessage => errorMessage;
    }
}
