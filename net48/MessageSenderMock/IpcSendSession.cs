using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Text;
using System.Threading.Tasks;

namespace MessageSenderMock
{
    internal class IpcSendSession : IDisposable
    {
        private readonly MemoryMappedFile _mappedFile;
        private const int LENGTH = byte.MaxValue + 1;

        public IpcSendSession(string mapName)
        {
            _mappedFile = MemoryMappedFile.CreateOrOpen(mapName, LENGTH);
        }

        public void Send(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);

            using (Stream stream = _mappedFile.CreateViewStream())
            {
                stream.WriteByte((byte)bytes.Length);
                stream.Write(bytes, 0, bytes.Length);
            }
        }

        public void Dispose()
        {
            _mappedFile.Dispose();
        }
    }
}
