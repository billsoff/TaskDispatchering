using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Serilog;

namespace A.UI.Service
{
    public sealed class IpcReceiveSession : IDisposable
    {
        private readonly string _mapName;
        private readonly int _pollingMilliseconds;

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private MemoryMappedFile _mappedFile;

        private DataReader _dataReader;

        public IpcReceiveSession(string mapName, int pollingMilliseconds = 100)
        {
            _mapName = mapName;
            _pollingMilliseconds = pollingMilliseconds;
        }

        public event EventHandler<SessionDataReceivedEventArgs> DataReceived;

        public async Task<bool> OpenSessionReceiveAsync()
        {
            bool opened = await OpenSessionAsync();

            if (!opened)
            {
                return false;
            }

            await ReceiveAsync();

            return true;
        }

        public async Task<bool> OpenSessionAsync()
        {
            CancellationToken token = _cancellationTokenSource.Token;

            Console.Write("Try open session {0}...  ", _mapName);
            Log.Information("[IPC] Try open session \"{SessionName}\"...", _mapName);

            while (true)
            {
                bool success;

                try
                {
                    success = OpenSession();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "[IPC] Open session \"{SessionName}\" failed", _mapName);

                    throw;
                }

                if (success)
                {
                    Console.WriteLine("OK. Session {0} opened.", _mapName);
                    Log.Information("[IPC] Open session \"{SessionName}\" succeeded", _mapName);

                    return true;
                }

                if (token.IsCancellationRequested)
                {
                    break;
                }

                await Task.Delay(_pollingMilliseconds);
            }

            Console.WriteLine("Canceled.");
            Log.Information("[IPC] Open session \"{SessionName}\" canceled", _mapName);

            return false;
        }

        public async Task ReceiveAsync()
        {
            if (_mappedFile == null)
            {
                Log.Error("[IPC] Cannot receiving data because session \"{SessionName}\" is not opened", _mapName);

                throw new InvalidOperationException("Please open session first.");
            }

            _dataReader?.Dispose();
            _dataReader = new DataReader(_mappedFile);
            string newData;

            CancellationToken token = _cancellationTokenSource.Token;

            Console.WriteLine("Start receiving data...");
            Log.Information("[IPC] Start receiving data from session \"{SessionName}\"...", _mapName);

            while (true)
            {
                try
                {
                    newData = _dataReader.Read();

                    if (newData != null)
                    {
                        Log.Information("[IPC] Received data 「{Data}」 from session \"{SessionName}\"", newData, _mapName);

                        DataReceived?.Invoke(this, new SessionDataReceivedEventArgs(newData));
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "[IPC] Session \"{SessionName}\" is terminated because receiving data failed", _mapName);

                    throw;
                }

                if (token.IsCancellationRequested)
                {
                    break;
                }

                await Task.Delay(_pollingMilliseconds);
            }

            Console.WriteLine("End receiving data.");

            Log.Information("[IPC] Session \"{SessionName}\" is closed", _mapName);
            Log.Information("[IPC] End receiving data from session \"{SessionName}\"", _mapName);
        }

        private bool OpenSession()
        {
            try
            {
                _mappedFile = MemoryMappedFile.OpenExisting(_mapName);
            }
            catch (FileNotFoundException)
            {
            }

            return _mappedFile != null;
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            Thread.Sleep(_pollingMilliseconds * 5);

            _dataReader?.Dispose();
            _dataReader = null;

            _mappedFile?.Dispose();
            _mappedFile = null;

            GC.SuppressFinalize(this);

            Log.Information("[IPC] Session \"{SessionName}\" is disposed", _mapName);
        }


        private sealed class DataReader : IDisposable
        {
            private const int LENGTH = byte.MaxValue + 1; // 1 byte to store data length

            private readonly byte[] _oldData = new byte[LENGTH];
            private readonly byte[] _newData = new byte[LENGTH];

            private readonly Stream _stream;

            public DataReader(MemoryMappedFile mappedFile)
            {
                _stream = mappedFile.CreateViewStream();
            }

            public string Read()
            {
                _stream.Position = 0;
                _stream.Read(_newData, 0, LENGTH);

                if (!IsNew())
                {
                    return null;
                }

                Backup();

                return Encoding.UTF8.GetString(_newData, 1, _newData[0]);
            }

            private bool IsNew() =>
                _newData[0] != _oldData[0] ||
                !_newData.Take(_newData[0] + 1).SequenceEqual(
                 _oldData.Take(_oldData[0] + 1));

            private void Backup()
            {
                for (int i = 0; i < _oldData.Length; i++)
                {
                    _oldData[i] = 0;
                }

                _newData.CopyTo(_oldData, 0);
            }

            public void Dispose()
            {
                _stream.Dispose();
                GC.SuppressFinalize(this);
            }
        }
    }

    public sealed class SessionDataReceivedEventArgs : EventArgs
    {
        public SessionDataReceivedEventArgs(string data)
        {
            Data = data;
        }

        public string Data { get; }
    }
}
