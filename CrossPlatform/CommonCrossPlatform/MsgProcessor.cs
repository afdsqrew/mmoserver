using System;
using System.Collections.Concurrent;

namespace Ymck
{
    /// <summary>
    /// 消息分发器（单例）。
    /// 网络线程：Process() 查处理器 → 深拷贝 → 入队（线程安全）
    /// 逻辑线程：HandleAllMsgs() 排空队列并按注册类型分发
    /// </summary>
    public class MsgProcessor<TSession, TSocket> : Singleton<MsgProcessor<TSession, TSocket>>
        where TSession : new()
        where TSocket : WebSocketBehaviorEx<TSession>
    {
        public delegate void MsgHandler(TSocket sender, object reqOrResp);
        public delegate void MsgHandler<T>(TSocket sender, T reqOrResp);

        private struct PendingMessage
        {
            public MsgHandler Handler;
            public TSocket Sender;
            public object Message;
        }

        /// <summary>把强类型处理器适配成非泛型委托的闭包。</summary>
        private sealed class TypedInvoker<T>
        {
            private readonly MsgHandler<T> _handler;
            public TypedInvoker(MsgHandler<T> handler) => _handler = handler;
            public void Invoke(TSocket sender, object reqOrResp)
                => _handler(sender, (T)reqOrResp);
        }

        private readonly ConcurrentDictionary<Type, MsgHandler> _handlers
            = new ConcurrentDictionary<Type, MsgHandler>();
        private readonly ConcurrentQueue<PendingMessage> _queue
            = new ConcurrentQueue<PendingMessage>();

        public MsgProcessor() { }

        /// <summary>注册某种消息类型的处理器（重复注册会覆盖）。</summary>
        public void RegHandler<T>(MsgHandler<T> handler)
        {
            if (handler == null) throw new ArgumentNullException(nameof(handler));
            _handlers[typeof(T)] = new TypedInvoker<T>(handler).Invoke;
        }

        public bool RemoveHandler<T>()
            => _handlers.TryRemove(typeof(T), out _);

        /// <summary>收到消息时调用，可在任意线程调用。</summary>
        public void Process<TNetMsg>(TSocket sender, TNetMsg netMsg)
        {
            Enqueue(sender, (object)netMsg);
        }

        private void Enqueue(TSocket sender, object msg)
        {
            if (msg == null)
            {
                Logger.Debug("@1");
                return;
            }
            if (!_handlers.TryGetValue(msg.GetType(), out var handler))
            {
                Logger.Debug("@2");
                Logger.Debug($"@2 msg.GetType()={msg.GetType()}");
                return;   // 未注册的类型直接丢弃
            }

            Logger.Debug($"[Msg] 入队 {msg.GetType().Name}");
            try
            {
                _queue.Enqueue(new PendingMessage
                {
                    Handler = handler,
                    Sender = sender,
                    Message = msg.DeepCopy(),   // 入队时快照，防止网络线程复用缓冲区
                });
            }
            catch (Exception ex)
            {
                Logger.Exception(ex.Message);
            }
        }

        /// <summary>在逻辑线程调用；单个处理器抛异常不影响后续消息。</summary>
        public void HandleAllMsgs()
        {
            Logger.Debug($"HandleAllMsgs = {_queue.Count}");
            while (_queue.TryDequeue(out var item))
            {
                try { item.Handler(item.Sender, item.Message); }
                catch (Exception ex) { Logger.Exception(ex.Message); }
            }
        }

        // 占位：原 Process() 开头那段被混淆的环境/授权校验，字符串解不出来
        private static bool ValidateEnvironment() => true;
    }
}
