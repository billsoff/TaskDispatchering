using System.IO.MemoryMappedFiles;
using System.Text;

namespace IpcSessions;

internal abstract class SessionChannel(
        MemoryMappedFile mappedFile,
        Mutex mutex,
        string fromProcess,
        string toProcess
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

    protected string ReadContent()
    {
        Stream.Seek(0, SeekOrigin.Begin);
        int contentLength = GetContentLength();

        if (contentLength <= 0)
        {
            return null;
        }

        using StreamReader reader = new(
                new ContentStream(Stream, contentLength),
                Encoding,
                leaveOpen: true
            );

        return reader.ReadToEnd();
    }


    private sealed class ContentStream(Stream innerStream, int length) : Stream
    {
        private readonly long _startPosition = innerStream.Position;

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => length;

        public override long Position { get => innerStream.Position - _startPosition; set => throw new NotSupportedException(); }

        public override int Read(byte[] buffer, int offset, int count) =>
            innerStream.Read(buffer, offset, Math.Min(count, (int)(Length - Position)));

        public override int ReadByte() =>
            Position < Length ? innerStream.ReadByte() : -1;

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
