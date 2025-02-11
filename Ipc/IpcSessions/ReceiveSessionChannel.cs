using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;

namespace IpcSessions;

internal sealed class ReceiveSessionChannel : SessionChannel
{
    private readonly int _pollingMilliseconds;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly ConfiguredTaskAwaitable _pollingTask;

    private readonly Thread _pollingThread;

    public ReceiveSessionChannel(
            MemoryMappedFile mappedFile,
            Mutex mutex,
            string fromProcess,
            string toProcess,
            int pollingMilliseconds
        ) : base(mappedFile, mutex, fromProcess, toProcess)
    {
        _pollingMilliseconds = pollingMilliseconds;
        _pollingThread = new Thread(StartPolling)
        {
            IsBackground = true
        };
        _pollingThread.Start();
    }

    public event EventHandler<MessageReceivedEventArgs> MessageReceived;

    private void StartPolling()
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
                Thread.Sleep(_pollingMilliseconds);

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

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _cancellationTokenSource.Cancel();
    }
}
