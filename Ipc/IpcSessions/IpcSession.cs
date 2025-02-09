using System.IO.MemoryMappedFiles;

namespace IpcSessions;

public sealed class IpcSession
{
    private const int SESSION_SIZE = 1024 * 4;
    private const int RECEIVE_POLLING_MILLISECONDS = 100;

    private const string UNIQUE_ID = "9A752043";

    private readonly ReceiveSessionChannel _sessionCreateRequest;

    private readonly Dictionary<string, SendSessionChannel> _sendSessions = [];
    private readonly Dictionary<string, ReceiveSessionChannel> _receiveSessions = [];

    public IpcSession(
            string processName,
            bool canBeConnected = false,
            int sessionSize = SESSION_SIZE,
            int receivePollingMilliseconds = RECEIVE_POLLING_MILLISECONDS
        )
    {
        ProcessName = processName;

        SessionSize = sessionSize;
        ReceivePollingMilliseconds = receivePollingMilliseconds;

        if (canBeConnected)
        {
            MemoryMappedFile _sessionCreatingMappedFile = MemoryMappedFile.CreateNew(
                    CreateMappedFileName(processName, SessionChannelType.SessionCreateRequest),
                    SessionSize
                );
            Mutex _sessionCreatingMutex = new(initiallyOwned: false, CreateMutexName(processName, SessionChannelType.SessionCreateRequest));

            _sessionCreateRequest = new ReceiveSessionChannel(
                    _sessionCreatingMappedFile,
                    _sessionCreatingMutex,
                    fromProcess: null,
                    toProcess: processName,
                    SessionSize,
                    RECEIVE_POLLING_MILLISECONDS
                );
            _sessionCreateRequest.MessageReceived += OnSessionCreateRequestMessageReceived;
            _ = _sessionCreateRequest.StartPollingAsync();
        }
    }

    public string ProcessName { get; }

    public int SessionSize { get; }

    public int ReceivePollingMilliseconds { get; }

    public event EventHandler<SessionCreatedEventArgs> SessionCreated;

    public event EventHandler<MessageReceivedEventArgs> MessageReceived;

    public async Task CreateSessionAsync(string connectedProcessName)
    {
        if (_sendSessions.ContainsKey(connectedProcessName))
        {
            return;
        }

        string sendMappedFileName = CreateMappedFileName(
                ProcessName,
                SessionChannelType.SendMessage,
                connectedProcessName
            );
        string sendMutexName =
                CreateMutexName(
                ProcessName,
                SessionChannelType.SendMessage,
                connectedProcessName
            );
        string receiveMappedFileName = CreateMappedFileName(
                ProcessName,
                SessionChannelType.ReceiveMessage,
                connectedProcessName
            );
        string receiveMutexName = CreateMutexName(
                ProcessName,
                SessionChannelType.ReceiveMessage,
                connectedProcessName
            );

        MemoryMappedFile sendMappedFile = MemoryMappedFile.CreateNew(sendMappedFileName, SessionSize);
        Mutex sendMutex = new(initiallyOwned: false, sendMutexName);
        SendSessionChannel sendSession = new(
                sendMappedFile,
                sendMutex,
                ProcessName,
                connectedProcessName,
                SessionSize
            );
        _sendSessions.Add(connectedProcessName, sendSession);

        MemoryMappedFile receiveMappedFile = MemoryMappedFile.CreateNew(receiveMappedFileName, SessionSize);
        Mutex receiveMutex = new(initiallyOwned: false, receiveMutexName);
        ReceiveSessionChannel receiveSession = new(
                receiveMappedFile,
                receiveMutex,
                fromProcess: connectedProcessName,
                toProcess: ProcessName,
                SessionSize,
                ReceivePollingMilliseconds
            );
        _receiveSessions.Add(connectedProcessName, receiveSession);

        receiveSession.MessageReceived += OnReceiveSessionMessageReceived;
        _ = receiveSession.StartPollingAsync();

        SessionCreateRequestMessage message = new(ProcessName, connectedProcessName)
        {
            SendMappedFileName = receiveMappedFileName,
            SendMutexName = receiveMutexName,
            ReceiveMappedFileName = sendMappedFileName,
            ReceiveMutexName = sendMutexName,
        };
        SendSessionChannel sessionCreateRequest = new(
                MemoryMappedFile.OpenExisting(CreateMappedFileName(connectedProcessName, SessionChannelType.SessionCreateRequest)),
                Mutex.OpenExisting(CreateMutexName(connectedProcessName, channelType: SessionChannelType.SessionCreateRequest)),
                ProcessName,
                connectedProcessName,
                SessionSize
            );

        await sessionCreateRequest.SendMessage(message.ToJson());
    }

    public Task SendMessageAsync(string text, string connectedProcessName)
    {
        SendSessionChannel sendSession = _sendSessions[connectedProcessName];
        TextMessage message = new(ProcessName, connectedProcessName)
        {
            Text = text,
        };

        return sendSession.SendMessage(message.ToJson());
    }

    private void OnSessionCreateRequestMessageReceived(object sender, MessageReceivedEventArgs e)
    {
        var message = Message.Load<SessionCreateRequestMessage>(e.Data);

        MemoryMappedFile sendMappedFile = MemoryMappedFile.OpenExisting(message.SendMappedFileName);
        Mutex sendMutex = Mutex.OpenExisting(message.SendMutexName);

        MemoryMappedFile receiveMappedFile = MemoryMappedFile.OpenExisting(message.ReceiveMappedFileName);
        Mutex receiveMutex = Mutex.OpenExisting(message.ReceiveMutexName);

        SendSessionChannel sendSession = new(
                sendMappedFile,
                sendMutex,
                ProcessName,
                message.From,
                SessionSize
            );
        ReceiveSessionChannel receiveSession = new(
                receiveMappedFile,
                receiveMutex,
                fromProcess: message.From,
                toProcess: ProcessName,
                SessionSize,
                ReceivePollingMilliseconds
            );

        _sendSessions.Add(message.From, sendSession);
        _receiveSessions.Add(message.From, receiveSession);

        _ = receiveSession.StartPollingAsync();
        receiveSession.MessageReceived += OnReceiveSessionMessageReceived;

        SessionCreated?.Invoke(this, new SessionCreatedEventArgs(message));
    }

    private void OnReceiveSessionMessageReceived(object sender, MessageReceivedEventArgs e) =>
        MessageReceived?.Invoke(this, e);

    private static string CreateMappedFileName(string ownerProcessName, SessionChannelType channelType, string connectedProcessName = null) =>
        connectedProcessName != null
            ? $"{ownerProcessName}_{connectedProcessName}_mapped_file_{GetChannelTypeName(channelType)}_{UNIQUE_ID}"
            : $"{ownerProcessName}_mapped_file_{GetChannelTypeName(channelType)}_{UNIQUE_ID}";

    private static string CreateMutexName(string selfProcessName, SessionChannelType channelType, string connectedProcessName = null) =>
        connectedProcessName != null
            ? $"{selfProcessName}_{connectedProcessName}_mutex_{GetChannelTypeName(channelType)}_{UNIQUE_ID}"
            : $"{selfProcessName}_mutex_{GetChannelTypeName(channelType)}_{UNIQUE_ID}";

    private static string GetChannelTypeName(SessionChannelType channelType) => channelType switch
    {
        SessionChannelType.SendMessage => "send",
        SessionChannelType.ReceiveMessage => "receive",
        _ => "session_create",
    };
}
