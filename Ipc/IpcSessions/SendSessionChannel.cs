using System.IO.MemoryMappedFiles;

namespace IpcSessions;

internal sealed class SendSessionChannel(
        MemoryMappedFile mappedFile,
        Mutex mutex,
        string fromProcess,
        string toProcess,
        int sessionSize
    ) : SessionChannel(mappedFile, mutex, fromProcess, toProcess, sessionSize)
{
    public async Task SendMessage(string message)
    {
        Mutex.WaitOne();
        Stream.Seek(0, SeekOrigin.Begin);

        try
        {
            int byteCount = Encoding.GetByteCount(message);

            SetContentLength(byteCount);
            await DoSendMessage(message);
        }
        finally
        {
            Mutex.ReleaseMutex();
        }
    }

    private async Task DoSendMessage(string message)
    {
        using StreamWriter writer = new(Stream, Encoding, leaveOpen: true);
        await writer.WriteAsync(message);
    }
}
