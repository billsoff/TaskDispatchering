using System.IO.MemoryMappedFiles;

namespace IpcSessions;

internal sealed class SendSessionChannel(
        MemoryMappedFile mappedFile,
        Mutex mutex,
        string fromProcess,
        string toProcess
    ) : SessionChannel(mappedFile, mutex, fromProcess, toProcess)
{
    public void SendMessage(string message)
    {
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        while (HasContent())
        {
            Thread.Sleep(100);
        }

        Mutex.WaitOne();

        if (HasContent())
        {
            Mutex.ReleaseMutex();

            SendMessage(message);

            return;
        }

        Stream.Seek(0, SeekOrigin.Begin);

        try
        {
            int byteCount = DoSendMessage(message);
            SetContentLength(byteCount);
        }
        finally
        {
            Mutex.ReleaseMutex();
        }
    }

    private int DoSendMessage(string message)
    {
        SetContentLength(0);
        long startPosition = Stream.Position;

        using StreamWriter writer = new(Stream, Encoding, leaveOpen: true);

        writer.Write(message);
        writer.Close();

        return (int)(Stream.Position - startPosition);
    }
}
