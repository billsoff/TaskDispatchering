using System.IO.MemoryMappedFiles;

namespace IpcSessions;

internal sealed class ReceiveSessionChannel(
        MemoryMappedFile mappedFile,
        Mutex mutex,
        string fromProcess,
        string toProcess,
        int sessionSize,
        int pollingMilliseconds
    ) : SessionChannel(mappedFile, mutex, fromProcess, toProcess, sessionSize)
{
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public event EventHandler<MessageReceivedEventArgs> MessageReceived;

    public async Task StartPollingAsync()
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
                await Task.Delay(pollingMilliseconds);

                continue;
            }

            Mutex.WaitOne();

            try
            {
                string content = await ReadContentAsync();
                ResetContentLength();

                MessageReceived?.Invoke(this, new MessageReceivedEventArgs(content));
            }
            finally
            {
                Mutex.ReleaseMutex();
            }
        }
    }

    public void EndPolling()
    {
        _cancellationTokenSource.Cancel();
    }
}
