using System.IO.MemoryMappedFiles;
using System.Text;

namespace IpcSessions;

internal abstract class SessionChannel(
        MemoryMappedFile mappedFile,
        Mutex mutex,
        string fromProcess,
        string toProcess,
        int sessionSize
    )
{
    protected static readonly Encoding Encoding = new UTF8Encoding(
            encoderShouldEmitUTF8Identifier: false,
            throwOnInvalidBytes: false
        );

    protected MemoryMappedViewStream Stream { get; } = mappedFile.CreateViewStream();

    protected Mutex Mutex { get; } = mutex;

    protected string FromProcess { get; } = fromProcess;

    protected string ToProcess { get; } = toProcess;

    protected byte[] SessionBuffer { get; } = new byte[sessionSize];

    protected void ResetContentLength() => SetContentLength(0);

    protected void SetContentLength(int byteCount)
    {
        Stream.Seek(0, SeekOrigin.Begin);

        using BinaryWriter writer = new(Stream, Encoding, leaveOpen: true);
        writer.Write(byteCount);
    }

    protected int GetContentLength()
    {
        if (Stream.Length == 0)
        {
            return 0;
        }

        Stream.Seek(0, SeekOrigin.Begin);

        using BinaryReader reader = new(Stream, Encoding, leaveOpen: true);

        return reader.ReadInt32();
    }

    protected bool HasContent()
    {
        int contentLength = GetContentLength();

        return contentLength > 0;
    }

    protected async Task<string> ReadContentAsync()
    {
        Stream.Seek(0, SeekOrigin.Begin);
        int contentLength = GetContentLength();

        if (contentLength <= 0)
        {
            return null;
        }

        await Stream.ReadAsync(SessionBuffer, 0, contentLength);
        using StreamReader reader = new(
                new MemoryStream(SessionBuffer, 0, contentLength, writable: false),
                Encoding,
                leaveOpen: true
            );

        return await reader.ReadToEndAsync();
    }
}
