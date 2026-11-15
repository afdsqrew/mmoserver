using System;
using System.Collections.Generic;
using WebSocketSharp.Server;

namespace Ymck
{
    /// <summary>
    /// d2-04
    /// </summary>
    public static class MsgProcessor_002
    {
        // 消息类型 -> 处理委托
        private static readonly Dictionary<Type, MsgHandler> msgHandlers =
            new Dictionary<Type, MsgHandler>();

        /// <summary>
        /// 注册 T 类型消息的处理函数。
        /// 通过闭包把泛型委托适配成统一的非泛型委托，以便按 Type 存入字典。
        /// </summary>
        public static void RegHandler<T>(MsgHandler<T> handler)
        {
            msgHandlers[typeof(T)] = (sender, reqOrResp) => handler(sender, (T)reqOrResp);
        }

        /// <summary>
        /// 按消息对象的运行时类型分发给已注册的处理函数。
        /// </summary>
        private static void HandleMsg(WebSocketBehaviorEx_001 sender, object reqOrResp)
        {
            if (reqOrResp == null)
                return;

            if (msgHandlers.TryGetValue(reqOrResp.GetType(), out MsgHandler handler))
                handler(sender, reqOrResp);
        }

        /// <summary>
        /// 处理一条网络消息：有请求就处理请求，否则处理响应。
        /// 目前只分发 userReg（用户注册）一种消息。
        /// </summary>
        public static void Process(WebSocketBehaviorEx_001 sender, NetMsg netMsg)
        {
            if (netMsg.Req != null)
                HandleMsg(sender, netMsg.Req.UserReg);
            else if (netMsg.Resp != null)
                HandleMsg(sender, netMsg.Resp.UserReg);
        }

        public delegate void MsgHandler(WebSocketBehaviorEx_001 sender, object reqOrResp);

        public delegate void MsgHandler<T>(WebSocketBehaviorEx_001 sender, T reqOrResp);
    }
}
