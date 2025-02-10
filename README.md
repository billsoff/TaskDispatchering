## IPC(进程间通信)说明
### 使用方法
  类`IpcSession`用于建立会话，收/发消息
  1. 构造
    <br />1.1 服务端构造
  1. 
  1. ```c#
        IpcSession session = new(processName: "A", canBeConnected: true);
      ```

<br />
&nbsp;&nbsp;&nbsp;&nbsp;1.2 客户端构造

      ```c#
        IpcSession session = new(processName: "B");
      ```

  2. 建立会话，由客户端发起

    ```c#
      await session.CreateSessionAsync("A");
    ```

  3. 如果会话成功建立，服务端会发生`SessionCreated`事件
    
  4. 发送消息时，调用方法`SendMessageAsync`来实现

    ```c#
      await session.SendMessageAsync(text, connectedProcessName);
    ```

  5. 接受消息通过事件`TextMessageReceived`来实现
    <br />5.1 事件订阅

      ```c#
        session.TextMessageReceived += OnTextMessageReceived;
      ```

    <br />5.2 接受消息

      ```c#
          static void OnTextMessageReceived(object sender, TextMessageReceivedEventArgs e)
          {
              var message = e.Message;

              WriteLine("{0}: {1} ({2:HH:mm:ss})", message.From, message.Text, message.Timestamp);
              WriteLine();
          }
      ```

### 实现方法
1. `IpcSession`内部通过两个信道(Channel)来发/收消息，分别是`SendSessionChannel`和`ReceiveSessionChannel`。(服务还单独有一个`ReceiveSessionChannel`用于创建连接)
  服务端和客户端刚好是对称的，即服务端的`SendSessionChannel`是客户端的`ReceiveSessionChannel`，反之亦然。

2. 信道保持`MemoryMappedFile`和`Mutex`，前者是非持久的，后者用于进程同步。都是在建立连接时由客户端创建的，服务端随后打开。

3. 消息格式
  _消息长度__消息内容_ _消息长度_ 是4个字节的整数，_消息内容_ 是UTF8编码的消息体。

4. 发消息
  <br />4.1 获取`Mutex`锁
  <br />4.2 向`SendSessionChannel`写入 _消息长度_ 和 _消息内容_
  <br />4.3 释放`Mutex`锁

5. 收消息
  <br />5.1 检查 _消息长度_ ，如为 `0`，则执行下一步，否则，转到 5.3
  <br />5.2 睡眠`100`毫秒(这个值可以在构造`IpcSession`时设置)，然后转到 5.1
  <br />5.3 获取`Mutex`锁
  <br />5.4 读取`MemoryMappedFile`中的消息，发布`MessageReceived`事件
  <br />5.5 将 _消息长度_ 置 `0`
  <br />5.6 释放`Mutex`锁
  <br />5.7 转到 5.1
