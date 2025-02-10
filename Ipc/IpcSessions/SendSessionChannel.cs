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
        Mutex.WaitOne();
        Stream.Seek(0, SeekOrigin.Begin);

        try
        {
            int byteCount = Encoding.GetByteCount(message);

            SetContentLength(byteCount);
            DoSendMessage(message);
        }
        finally
        {
            Mutex.ReleaseMutex();
        }
    }

    private void DoSendMessage(string message)
    {
        using StreamWriter writer = new(Stream, Encoding, leaveOpen: true);
        writer.Write(message);
    }
}
