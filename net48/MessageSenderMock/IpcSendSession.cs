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

        public IpcSendSession(string filePath, string mapName)
        {
            _mappedFile = MemoryMappedFile.CreateFromFile(filePath, FileMode.Open, mapName);
        }

        public async Task SendAsync(string data)
        {
            int length = Encoding.UTF8.GetByteCount(data);
            ClearStream(length);

            Stream stream = _mappedFile.CreateViewStream();

            using (StreamWriter writer = new StreamWriter(stream))
            {
                await writer.WriteAsync(data);
            }
        }

        public void Dispose()
        {
            ClearStream();
            _mappedFile.Dispose();
        }

        private void ClearStream(int dataLength = 252)
        {
            Stream stream = _mappedFile.CreateViewStream();
            byte ZERO = 0;
            int byteCount = dataLength + 4;

            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                for (int i = 0; i < byteCount; i++)
                {
                    writer.Write(ZERO);
                }
            }
        }
    }
}
