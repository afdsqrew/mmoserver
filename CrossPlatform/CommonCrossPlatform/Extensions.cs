using System;
using System.IO;
using System.Text.RegularExpressions;
using LitJson;
using WebSocketSharp;

#nullable disable

namespace Ymck
{
    /// <summary>
    /// 通用扩展方法集合。
    /// 还原自 CommonNetFx.dll!Ymck.Extensions。
    /// </summary>
    /// <remarks>
    /// 本类型只有控制流混淆（switch/goto + if(false) __methodref）和字符串加密，
    /// 没有标识符重命名，逻辑是完整的。
    ///
    /// 原版两处日志写作：
    ///   Logger.Debug(DomainLayout.UnregisterClient(ConditionType.UnregisterClient(1),
    ///                                            JsonMapper.ToJson(o).Unescape()));
    /// 其中 ConditionType.UnregisterClient(1) 是字符串解密表取出的格式串（明文未知），
    /// DomainLayout.UnregisterClient(fmt, arg) 等价于 string.Format。见 SendLogFormat。
    /// </remarks>
    public static class Extensions
    {
        /// <summary>
        /// 发送日志的格式串占位。原程序集里是 ConditionType.UnregisterClient(1) 解密出的明文，
        /// 需要从解密表 dump 出来后替换，才能与原版日志逐字一致。
        /// </summary>
        private const string SendLogFormat = "[SendPBO] {0}";

        /// <summary>
        /// 把对象用 protobuf-net 序列化后，通过原生 WebSocket 发送。
        /// PBO = Protocol Buffers Object。
        /// </summary>
        /// 
        /*Custom
        public static void SendPBO(this WebSocket ws, object o)
        {
            if (ws == null) throw new ArgumentNullException(nameof(ws));
            SendProtobuf(o, ws.Send);
        }
        */

        /// <summary>
        /// 把对象用 protobuf-net 序列化后，通过 WebSocketBehaviorEx 会话发送。
        /// 对应上一轮还原的 WebSocketBehaviorEx&lt;TSession&gt;.SendEx(byte[])。
        /// </summary>
        public static void SendPBO<TSession>(this WebSocketBehaviorEx<TSession> ws, byte[] o)
            where TSession : new()
        {
            if (ws == null) throw new ArgumentNullException(nameof(ws));
            SendProtobuf(o, ws.SendEx);
        }


        private static void SendProtobuf(byte[] o, Action<byte[]> send)
        {
            byte[] payload;
            payload = o;
            // 仅用于日志可读性：把 JSON 里的转义序列还原成人能读的文本。
            // Regex.Unescape 不是 JSON 解码器，见 Unescape() 的说明。
            Logger.Debug(string.Format(SendLogFormat, JsonMapper.ToJson(o).Unescape()));

            send(payload);
        }
        /*
        private static void SendProtobuf(object o, Action<byte[]> send )
        {
            byte[] payload;
            using (var buffer = new MemoryStream())
            {
                // 注意：T 传的是 object，protobuf-net 会按 o 的运行时类型去找序列化器，
                // 因此 o 必须带 [ProtoContract]（或已在 RuntimeTypeModel 中注册），否则会抛异常。
                Serializer.Serialize<object>(buffer, o);
                payload = buffer.ToArray();
            }

            // 仅用于日志可读性：把 JSON 里的转义序列还原成人能读的文本。
            // Regex.Unescape 不是 JSON 解码器，见 Unescape() 的说明。
            Logger.Debug(string.Format(SendLogFormat, JsonMapper.ToJson(o).Unescape()));

            send(payload);
        }
        */


        /// <summary>
        /// 两个 SendPBO 的公共实现：序列化 → 取字节 → 记日志 → 发送。
        /// 原版是两份完全重复的方法体 + try/finally 手动 Dispose，这里合并并用 using。
        /// </summary>
        /// 
        /*@@custom
        private static void SendProtobuf(object o, Action<byte[]> send)
        {
            byte[] payload;
            using (var buffer = new MemoryStream())
            {
                // 注意：T 传的是 object，protobuf-net 会按 o 的运行时类型去找序列化器，
                // 因此 o 必须带 [ProtoContract]（或已在 RuntimeTypeModel 中注册），否则会抛异常。
                Serializer.Serialize<object>(buffer, o);
                payload = buffer.ToArray();
            }

            // 仅用于日志可读性：把 JSON 里的转义序列还原成人能读的文本。
            // Regex.Unescape 不是 JSON 解码器，见 Unescape() 的说明。
            Logger.Debug(string.Format(SendLogFormat, JsonMapper.ToJson(o).Unescape()));

            send(payload);
        }
        */

        /// <summary>JSON 往返深拷贝，结果写入 out 参数。</summary>
        public static void DeepCopyTo<T>(this object o, out T o2)
            => o2 = JsonMapper.ToObject<T>(JsonMapper.ToJson(o));

        /// <summary>JSON 往返深拷贝，返回指定类型。</summary>
        public static T DeepCopy<T>(this object o)
            => JsonMapper.ToObject<T>(JsonMapper.ToJson(o));

        /// <summary>
        /// JSON 往返深拷贝，保留运行时类型。MsgProcessor 入队时调用的就是这个重载。
        /// </summary>
        /// <remarks>
        /// 局限（原版即如此，使用时心里要有数）：
        ///   · o 为 null 时 o.GetType() 会抛 NullReferenceException；
        ///   · 只复制可序列化的公共成员，私有字段、只读字段会丢；
        ///   · 目标类型必须有可用的无参构造，且循环引用会导致无限递归；
        ///   · 丢失引用同一性，数值类型可能变化（LitJson 对 int/long/double 的映射较宽松），
        ///     DateTime、byte[] 等格式也不保证往返一致；
        ///   · 每次调用产生两个完整 JSON 字符串，高频消息队列里开销不小。
        /// 另外这是 this object 扩展，会挂到所有类型上；对 WebSocket / TSocket 这类
        /// 大对象图调用会直接失败，别误用。
        /// </remarks>
        public static object DeepCopy(this object o)
            => JsonMapper.ToObject(JsonMapper.ToJson(o), o.GetType());

        /// <summary>
        /// 反转义。实现用的是 Regex.Unescape（正则转义反转），不是 JSON / HTML / URL 解码；
        /// 遇到它不认识的转义序列或未闭合的 \uXXXX 会抛异常。
        /// 原版只在发送日志这一处用到，且没有 try/catch —— 日志异常会连带中断发送。
        /// </summary>
        public static string Unescape(this string s) => Regex.Unescape(s);
    }
}
