using System;
using System.IO;
using WebSocketSharp;
using WebSocketSharp.Server;

#nullable disable

namespace Ymck
{
    /// <summary>
    /// WebSocketSharp 会话行为扩展基类：为每个 WebSocket 连接绑定一个 TSession 会话状态对象。
    /// </summary>
    /// <remarks>
    /// 还原自 CommonNetFx.dll!Ymck.WebSocketBehaviorEx`1。本类型本身未混淆，只需格式化与补注释。
    ///
    /// 它同时是 MsgProcessor&lt;TSession, TSocket&gt; 中 TSocket 的约束类型：
    /// 消息分发器把收到的消息排入队列，再由逻辑线程统一回调，正是为了避开
    /// WebSocketSharp 的 OnMessage 所在接收线程，并保证回调期间 session 仍然有效。
    /// </remarks>
    /// <typeparam name="TSession">会话状态类型，必须具有公共无参构造函数。</typeparam>
    public class WebSocketBehaviorEx<TSession> : WebSocketBehavior
        where TSession : new()
    {
        /// <summary>
        /// 当前连接的会话状态。OnOpen 时创建，OnClose 后置为 default(TSession)。
        /// 对引用类型而言关闭后即为 null；异步发送回调或排队中的消息若在此之后读取，需要自行判空。
        /// </summary>
        public TSession session;

        // ---- Send / SendAsync 的别名。原版没有附加任何逻辑，只是把基类方法以 *Ex 名称重新暴露，
        //      作用应是给派生类留出统一的拦截点，并避免与业务方法重名。

        public void SendEx(string data) => Send(data);

        public void SendEx(FileInfo file) => Send(file);

        public void SendEx(byte[] data) => Send(data);

        public void SendAsyncEx(FileInfo file, Action<bool> completed)
            => SendAsync(file, completed);

        public void SendAsyncEx(Stream stream, int length, Action<bool> completed)
            => SendAsync(stream, length, completed);

        public void SendAsyncEx(byte[] data, Action<bool> completed)
            => SendAsync(data, completed);

        public void SendAsyncEx(string data, Action<bool> completed)
            => SendAsync(data, completed);

        // ---- 会话生命周期

        protected override void OnOpen()
        {
            base.OnOpen();
            session = new TSession();
        }

        protected override void OnClose(CloseEventArgs e)
        {
            session = default(TSession);
            base.OnClose(e);
        }
    }


}
