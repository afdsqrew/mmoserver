using System;
using System.IO;
using WebSocketSharp.Server;

namespace Ymck
{
    /// <summary>
    /// 把基类 protected 的 Send / SendAsync 以 public 的 *Ex 方法转发出去，
    /// 让外部代码（例如 MsgProcessor 的回调，其 sender 正是本类型）能给客户端回消息。
    /// </summary>
    public class WebSocketBehaviorEx_001 : WebSocketBehavior
    {
        // 同步发送
        public void SendEx(string data) { Send(data); }
        public void SendEx(FileInfo file) { Send(file); }
        public void SendEx(byte[] data) { Send(data); }

        // 异步发送
        public void SendAsyncEx(FileInfo file, Action<bool> completed)
        { SendAsync(file, completed); }

        public void SendAsyncEx(Stream stream, int length, Action<bool> completed)
        { SendAsync(stream, length, completed); }

        public void SendAsyncEx(byte[] data, Action<bool> completed)
        { SendAsync(data, completed); }

        public void SendAsyncEx(string data, Action<bool> completed)
        { SendAsync(data, completed); }
    }
}
