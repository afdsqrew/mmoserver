using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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
        /// protobuf 生成类专用深拷贝：基于 LitJson 的 JSON 往返，返回指定类型。
        /// </summary>
        /// <remarks>
        /// 为什么不能直接用 <see cref="DeepCopy{T}(object)"/>：protoc 生成的类上有两个
        /// public static 属性 Parser(MessageParser&lt;T&gt;) 与 Descriptor(MessageDescriptor)，
        /// LitJson 的 JsonMapper.WriteValue 会连静态属性一起反射，递归进 MessageDescriptor
        /// 的反射元数据，触发 RuntimeType.DeclaringMethod 抛 InvalidOperationException，
        /// 经反射包装后成为 TargetInvocationException。
        ///
        /// 本方法的处理方式：
        ///   · 只取「公共实例属性」构造 JsonData，天然跳过静态元数据属性；
        ///   · JSON 的编码/解码仍全部交给 LitJson（ToJson(JsonData) / ToObject(json, Type)）；
        ///   · RepeatedField&lt;T&gt; 这类只读集合属性，LitJson 反序列化时会静默忽略，
        ///     因此额外以源对象为准把集合元素逐个重建并 Add 回去（递归处理嵌套消息）；
        ///   · 浮点写成 JSON 数字而不是字符串，不依赖 Utils.Init() 注册的 float 钩子。
        ///
        /// 适用范围：同类型深拷贝，或字段名一致且可写的目标类型（跨类型大小写不一致的
        /// 字段不会被映射，例如 protobuf 的 Name 对应不上 Model.User.name）。
        /// 已知限制：MapField&lt;K,V&gt;（实现 IDictionary 而非 IList）与 bytes/ByteString
        /// 未做还原；本工程的 .proto 目前只用到了标量、嵌套消息和 repeated 字段。
        /// </remarks>
        public static T DeepCopyProtobuf<T>(this object o)
            => (T)CopyProtobufImpl(o, typeof(T));

        /// <summary>同上，保留运行时类型（对标 <see cref="DeepCopy(object)"/>）。</summary>
        public static object DeepCopyProtobuf(this object o)
            => o == null ? null : CopyProtobufImpl(o, o.GetType());

        private static object CopyProtobufImpl(object o, Type targetType)
        {
            if (o == null) return null;
            if (targetType == null) throw new ArgumentNullException(nameof(targetType));

            // ① 只序列数据属性 → JsonData → JSON 字符串
            string json = JsonMapper.ToJson(ToJsonData(o));
            // ② 交给 LitJson 还原标量、枚举、可写属性与嵌套消息
            object copy = JsonMapper.ToObject(json, targetType);
            // ③ 补回 LitJson 无法写入的只读集合（RepeatedField<T> 等）
            RestoreCollections(o, copy, targetType);
            return copy;
        }

        /// <summary>把任意对象转成只包含数据成员的 JsonData，null 用 C# null 表示（JSON null）。</summary>
        private static JsonData ToJsonData(object value)
        {
            if (value == null) return null;

            Type t = value.GetType();

            // 标量必须放在集合判断之前：string 本身也实现 IEnumerable。
            if (t.IsEnum) return new JsonData(Convert.ToInt64(value));
            if (value is bool b) return new JsonData(b);
            if (value is float f) return new JsonData((double)f);
            if (value is double d) return new JsonData(d);
            if (value is decimal m) return new JsonData((double)m);
            if (value is string s) return new JsonData(s);
            if (value is char ch) return new JsonData(ch.ToString());
            if (value is byte || value is sbyte || value is short || value is ushort
                || value is int || value is uint || value is long)
                return new JsonData(Convert.ToInt64(value));
            if (value is ulong ul) return new JsonData((double)ul);
            if (value is DateTime dt) return new JsonData(dt.ToString("o"));

            // ByteString 没有硬引用（本公共库不引用 Google.Protobuf），按类型名兜底。
            if (t.FullName == "Google.Protobuf.ByteString")
            {
                MethodInfo toBase64 = t.GetMethod("ToBase64", Type.EmptyTypes);
                if (toBase64 != null)
                    return new JsonData((string)toBase64.Invoke(value, null));
            }

            // 数组 / RepeatedField<T> / List<T> 等
            if (value is IEnumerable seq)
            {
                JsonData arr = new JsonData();
                arr.SetJsonType(JsonType.Array);
                foreach (object item in seq)
                    arr.Add(ToJsonData(item));
                return arr;
            }

            // 普通对象：只反射公共实例属性，静态的 Parser / Descriptor 不会出现在这里。
            JsonData obj = new JsonData();
            obj.SetJsonType(JsonType.Object);
            foreach (PropertyInfo p in GetDataProperties(t))
                obj[p.Name] = ToJsonData(p.GetValue(value));
            return obj;
        }

        /// <summary>可读取、非索引器、非反射元数据类型的公共实例属性。</summary>
        private static IEnumerable<PropertyInfo> GetDataProperties(Type t)
        {
            foreach (PropertyInfo p in t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (!p.CanRead || p.GetIndexParameters().Length != 0) continue;
                if (IsMetadataType(p.PropertyType)) continue;
                yield return p;
            }
        }

        private static bool IsMetadataType(Type t)
        {
            string ns = t.Namespace ?? string.Empty;
            return typeof(Type).IsAssignableFrom(t)
                || ns.StartsWith("System.Reflection", StringComparison.Ordinal)
                || ns.StartsWith("Google.Protobuf.Reflection", StringComparison.Ordinal)
                || t.Name.StartsWith("MessageParser", StringComparison.Ordinal);
        }

        /// <summary>
        /// 以源对象为准，重建目标对象上的集合属性。
        /// LitJson 不会给「只有 getter」的 RepeatedField&lt;T&gt; 赋值，只能取到实例后手工 Add。
        /// </summary>
        private static void RestoreCollections(object src, object dst, Type dstType)
        {
            if (src == null || dst == null) return;

            foreach (PropertyInfo p in GetDataProperties(src.GetType()))
            {
                PropertyInfo dp = dstType.GetProperty(p.Name, BindingFlags.Public | BindingFlags.Instance);
                if (dp == null || !dp.CanRead) continue;

                if (IsCollectionType(p.PropertyType))
                {
                    IList srcList = p.GetValue(src) as IList;
                    IList dstList = dp.GetValue(dst) as IList;
                    if (srcList == null || dstList == null) continue;

                    dstList.Clear();
                    Type elemType = GetElementType(p.PropertyType);
                    foreach (object item in srcList)
                    {
                        if (item == null) { dstList.Add(null); continue; }
                        if (IsScalarType(elemType)) { dstList.Add(item); continue; }

                        // 元素同样走 JsonData 路径，避免直接 ToJson 又碰上静态元数据属性。
                        string elemJson = JsonMapper.ToJson(ToJsonData(item));
                        object elemCopy = JsonMapper.ToObject(elemJson, elemType);
                        RestoreCollections(item, elemCopy, elemType);
                        dstList.Add(elemCopy);
                    }
                }
                else if (!IsScalarType(p.PropertyType))
                {
                    object child = p.GetValue(src);
                    if (child == null) continue;
                    object dstChild = dp.GetValue(dst);
                    if (dstChild != null)
                        RestoreCollections(child, dstChild, dp.PropertyType);
                }
            }
        }

        private static bool IsScalarType(Type t)
        {
            t = Nullable.GetUnderlyingType(t) ?? t;
            return t.IsPrimitive || t.IsEnum || t == typeof(string)
                || t == typeof(decimal) || t == typeof(DateTime) || t == typeof(Guid);
        }

        private static bool IsCollectionType(Type t)
            => t != typeof(string) && typeof(IEnumerable).IsAssignableFrom(t);

        private static Type GetElementType(Type t)
        {
            if (t.IsArray) return t.GetElementType();
            if (t.IsGenericType)
            {
                Type[] args = t.GetGenericArguments();
                if (args.Length == 1) return args[0];
            }
            return typeof(object);
        }

        /// <summary>
        /// 反转义。实现用的是 Regex.Unescape（正则转义反转），不是 JSON / HTML / URL 解码；
        /// 遇到它不认识的转义序列或未闭合的 \uXXXX 会抛异常。
        /// 原版只在发送日志这一处用到，且没有 try/catch —— 日志异常会连带中断发送。
        /// </summary>
        public static string Unescape(this string s) => Regex.Unescape(s);
    }
}
