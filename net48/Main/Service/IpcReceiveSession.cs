using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Threading.Tasks;

namespace A.UI.Service
{
    public sealed class IpcReceiveSession : IDisposable
    {
        private readonly string _mapName;
        private readonly int _pollingMilliseconds;

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private MemoryMappedFile _mappedFile;

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

            while (true)
            {
                bool success = OpenSession();

                if (success)
                {
                    Console.WriteLine("OK. Session {0} opened.", _mapName);

                    return true;
                }

                if (token.IsCancellationRequested)
                {
                    break;
                }

                await Task.Delay(_pollingMilliseconds);
            }

            Console.WriteLine("Canceled.");

            return false;
        }

        public async Task ReceiveAsync()
        {
            if (_mappedFile == null)
            {
                throw new InvalidOperationException("Please open session first.");
            }

            CancellationToken token = _cancellationTokenSource.Token;
            string oldData = null;

            Console.WriteLine("Start receiving data...");

            while (true)
            {
                oldData = Receive(oldData);

                if (token.IsCancellationRequested)
                {
                    break;
                }

                await Task.Delay(_pollingMilliseconds);
            }

            Console.WriteLine("End receiving data.");
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

        private string Receive(string oldData)
        {
            using (StreamReader reader = new StreamReader(_mappedFile.CreateViewStream()))
            {
                string newData = reader.ReadToEnd()?.TrimEnd('\0');

                if (!string.IsNullOrWhiteSpace(newData) && newData != oldData)
                {
                    DataReceived?.Invoke(this, new SessionDataReceivedEventArgs(newData));
                }

                return newData;
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            Thread.Sleep(_pollingMilliseconds * 5);

            _mappedFile?.Dispose();
            _mappedFile = null;

            GC.SuppressFinalize(this);
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
