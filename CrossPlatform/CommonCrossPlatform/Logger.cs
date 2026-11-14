using System;
using System.IO;
using log4net;
using log4net.Config;

namespace Ymck
{
    /// <summary>
    /// log4net 的静态门面。
    /// 还原自 CommonNetFx.dll!Ymck.Logger。
    /// </summary>
    /// <remarks>
    /// 必须在进程启动时调用一次 Init()，否则 _log 为 null，下面五个方法全部会抛 NullReferenceException。
    /// 这一点直接影响 Extensions.SendPBO 与 MsgProcessor.Enqueue —— 它们的日志调用没有 try/catch。
    /// </remarks>
    public static class Logger
    {
        // TODO(还原)：以下三个字符串原本分别是
        //   ConditionType.UnregisterClient(14)  / logger 名称
        //   ConditionType.UnregisterClient(35)  / log4net 配置文件路径
        //   ConditionType.UnregisterClient(58)  / 初始化失败提示
        // 明文在解密表里，dump 出来后逐字替换。这里用的是路径合法、语义合理的占位值。
        private const string LoggerName = "Ymck";
        private const string ConfigFilePath = "log4net.xml";
        private const string InitFailedMessage = "log4net 配置未生效，Logger 初始化失败。";

        /// <summary>原反编译里的字段名也是 valuesCache（混淆器统一重命名）。</summary>
        private static ILog _log;

        /// <summary>
        /// 初始化：取 ILog、加载并监视 log4net 配置文件，配置未生效则抛异常。
        /// </summary>
        public static void Init()
        {
            _log = LogManager.GetLogger(LoggerName);
            XmlConfigurator.ConfigureAndWatch(new FileInfo(ConfigFilePath));

            // 原版：
            //   if ((LogManager.GetRepository().Configured ? 1 : 0)
            //       != FunctionSettings.UnregisterClient(16)) return;
            //   throw new System.Exception(ConditionType.UnregisterClient(58));
            // 常量表第 16 项按语义推定是 0，于是条件等价于「Configured 为 true 就返回」，
            // 即只有配置失败才抛异常。若 dump 出来的值不是 0，需要按原值反向调整这里。
            if (LogManager.GetRepository().Configured) return;
            throw new Exception(InitFailedMessage);
        }

        public static void Debug(string s) => _log.Debug((object)s);

        public static void Info(string s) => _log.Info((object)s);

        public static void Warning(string s) => _log.Warn((object)s);

        public static void Error(string s) => _log.Error((object)s);

        /// <summary>
        /// 注意：这个方法写的是 <b>Fatal</b> 级别，不是 Error。
        /// MsgProcessor.HandleAllMsgs / Enqueue 和 Extensions 里的 catch 全都调它，
        /// 意味着业务异常一律被记成致命错误，容易污染告警。原版即如此，此处忠实保留。
        /// </summary>
        public static void Exception(string s) => _log.Fatal((object)s);
    }
}
