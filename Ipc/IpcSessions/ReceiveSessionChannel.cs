using System.IO.MemoryMappedFiles;

namespace IpcSessions;

internal sealed class ReceiveSessionChannel : SessionChannel
{
    private readonly int _pollingMilliseconds;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly Task _pollingTask;

    public ReceiveSessionChannel(
            MemoryMappedFile mappedFile,
            Mutex mutex,
            string fromProcess,
            string toProcess,
            int pollingMilliseconds
        ) : base(mappedFile, mutex, fromProcess, toProcess)
    {
        _pollingMilliseconds = pollingMilliseconds;
        _pollingTask = StartPollingAsync();
    }

    public event EventHandler<MessageReceivedEventArgs> MessageReceived;

    private async Task StartPollingAsync()
    {
        CancellationToken token = _cancellationTokenSource.Token;

        while (true)
        {
            if (token.IsCancellationRequested)
            {
                break;
            }

            if (!HasContent())
            {
                await Task.Delay(_pollingMilliseconds);

                continue;
            }

            Mutex.WaitOne();

            try
            {
                string content = ReadContent();
                ResetContentLength();

                MessageReceived?.Invoke(this, new MessageReceivedEventArgs(content));
            }
            finally
            {
                Mutex.ReleaseMutex();
            }
        }
    }

    public async Task CloseAsync()
    {
        _cancellationTokenSource.Cancel();
        await _pollingTask;
    }
}
