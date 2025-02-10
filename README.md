## IPC(���̼�ͨ��)˵��
### ʹ�÷���
  ��`IpcSession`���ڽ����Ự����/����Ϣ
  1. ����
    <br />1.1 ����˹���
  1. 
  1. ```c#
        IpcSession session = new(processName: "A", canBeConnected: true);
      ```

<br />
&nbsp;&nbsp;&nbsp;&nbsp;1.2 �ͻ��˹���

      ```c#
        IpcSession session = new(processName: "B");
      ```

  2. �����Ự���ɿͻ��˷���

    ```c#
      await session.CreateSessionAsync("A");
    ```

  3. ����Ự�ɹ�����������˻ᷢ��`SessionCreated`�¼�
    
  4. ������Ϣʱ�����÷���`SendMessageAsync`��ʵ��

    ```c#
      await session.SendMessageAsync(text, connectedProcessName);
    ```

  5. ������Ϣͨ���¼�`TextMessageReceived`��ʵ��
    <br />5.1 �¼�����

      ```c#
        session.TextMessageReceived += OnTextMessageReceived;
      ```

    <br />5.2 ������Ϣ

      ```c#
          static void OnTextMessageReceived(object sender, TextMessageReceivedEventArgs e)
          {
              var message = e.Message;

              WriteLine("{0}: {1} ({2:HH:mm:ss})", message.From, message.Text, message.Timestamp);
              WriteLine();
          }
      ```

### ʵ�ַ���
1. `IpcSession`�ڲ�ͨ�������ŵ�(Channel)����/����Ϣ���ֱ���`SendSessionChannel`��`ReceiveSessionChannel`��(���񻹵�����һ��`ReceiveSessionChannel`���ڴ�������)
  ����˺Ϳͻ��˸պ��ǶԳƵģ�������˵�`SendSessionChannel`�ǿͻ��˵�`ReceiveSessionChannel`����֮��Ȼ��

2. �ŵ�����`MemoryMappedFile`��`Mutex`��ǰ���Ƿǳ־õģ��������ڽ���ͬ���������ڽ�������ʱ�ɿͻ��˴����ģ���������򿪡�

3. ��Ϣ��ʽ
  _��Ϣ����__��Ϣ����_ _��Ϣ����_ ��4���ֽڵ�������_��Ϣ����_ ��UTF8�������Ϣ�塣

4. ����Ϣ
  <br />4.1 ��ȡ`Mutex`��
  <br />4.2 ��`SendSessionChannel`д�� _��Ϣ����_ �� _��Ϣ����_
  <br />4.3 �ͷ�`Mutex`��

5. ����Ϣ
  <br />5.1 ��� _��Ϣ����_ ����Ϊ `0`����ִ����һ��������ת�� 5.3
  <br />5.2 ˯��`100`����(���ֵ�����ڹ���`IpcSession`ʱ����)��Ȼ��ת�� 5.1
  <br />5.3 ��ȡ`Mutex`��
  <br />5.4 ��ȡ`MemoryMappedFile`�е���Ϣ������`MessageReceived`�¼�
  <br />5.5 �� _��Ϣ����_ �� `0`
  <br />5.6 �ͷ�`Mutex`��
  <br />5.7 ת�� 5.1
