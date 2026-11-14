using System;
using LitJson;

#nullable disable

namespace Ymck
{
    /// <summary>
    /// LitJson 的全局注册入口。
    /// 还原自 CommonNetFx.dll!Ymck.Utils。
    /// </summary>
    /// <remarks>
    /// 必须在进程启动时、任何 JsonMapper.ToJson / ToObject 调用之前执行一次。
    /// 尤其是 Extensions.DeepCopy 与 SendPBO 的日志都走 LitJson，漏调会使含 float 字段的消息
    /// 序列化/反序列化失败（详见下面对导出/导入配对的说明）。
    ///
    /// 反编译里的 Utils.&lt;&gt;c.valuesCache 是 C# 编译器为「无捕获 lambda」生成的缓存字段，
    /// 混淆器把两个缓存字段都重命名成了 valuesCache（名字折叠，跟 MsgProcessor 里同一手法）。
    /// if (x == null) { x = ...; } 是标准的懒加载缓存模式；这里直接内联 lambda，
    /// 编译器会自动生成等价的 &lt;&gt;c 缓存类（元数据里也确实存在 Ymck.Utils+&lt;&gt;c）。
    /// </remarks>
    public static class Utils
    {
        /// <summary>
        /// 注册 float 的自定义序列化/反序列化钩子。
        /// </summary>
        public static void Init()
        {
            // 导出：把 float 写成 JSON 字符串（JsonWriter.Write(string) 会加引号），
            // 而不是默认的 JSON 数字。目的是规避 LitJson 对 float 的默认处理/精度问题。
            JsonMapper.RegisterExporter<float>(
                (value, writer) => writer.Write(value.ToString()));

            // 导入：与上面的字符串化配对。没有这个 importer，LitJson 遇到
            // 「JSON 字符串 → float 字段」会直接抛 JsonException。
            JsonMapper.RegisterImporter<string, float>(
                text => Convert.ToSingle(text));
        }
    }
}
