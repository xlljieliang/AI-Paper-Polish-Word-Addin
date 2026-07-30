using System;
using System.IO;
using System.Text;

namespace AIPolishCOMAddin.Infrastructure
{
    /// <summary>
    /// 日志模块 — 在插件目录下写入本地日志文件
    /// 日志文件：AIPolish_YYYYMMDD.log
    /// </summary>
    public static class Logger
    {
        private static string _logDirectory;
        private static readonly object _lock = new object();

        /// <summary>
        /// 初始化日志模块
        /// </summary>
        /// <param name="logDir">日志目录，默认使用插件所在目录</param>
        public static void Initialize(string logDir = null)
        {
            _logDirectory = logDir;
        }

        /// <summary>
        /// 写入信息日志
        /// </summary>
        public static void Info(string message)
        {
            WriteLog("INFO", message);
        }

        /// <summary>
        /// 写入警告日志
        /// </summary>
        public static void Warn(string message)
        {
            WriteLog("WARN", message);
        }

        /// <summary>
        /// 写入错误日志
        /// </summary>
        public static void Error(string message, Exception ex = null)
        {
            var sb = new StringBuilder(message);
            if (ex != null)
            {
                sb.AppendLine();
                sb.Append($"  异常: {ex.GetType().Name}: {ex.Message}");
                if (ex.InnerException != null)
                {
                    sb.AppendLine();
                    sb.Append($"  内部异常: {ex.InnerException.Message}");
                }
            }
            WriteLog("ERROR", sb.ToString());
        }

        /// <summary>
        /// 写入调试日志
        /// </summary>
        public static void Debug(string message)
        {
            WriteLog("DEBUG", message);
        }

        /// <summary>
        /// 写入 API 调用日志
        /// </summary>
        public static void ApiCall(string model, string endpoint, int inputTokens, int outputTokens, long elapsedMs, bool isSuccess)
        {
            string status = isSuccess ? "SUCCESS" : "FAILED";
            WriteLog("API", $"[{status}] model={model} endpoint={endpoint} " +
                             $"in_tokens={inputTokens} out_tokens={outputTokens} elapsed={elapsedMs}ms");
        }

        private static void WriteLog(string level, string message)
        {
            try
            {
                string logDir = _logDirectory ?? GetDefaultLogDir();
                if (!Directory.Exists(logDir))
                    Directory.CreateDirectory(logDir);

                string logFile = Path.Combine(logDir, $"AIPolish_{DateTime.Now:yyyyMMdd}.log");
                string logLine = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";

                lock (_lock)
                {
                    File.AppendAllText(logFile, logLine + Environment.NewLine, Encoding.UTF8);
                }

                // 清理超过30天的日志文件
                if (DateTime.Now.Hour == 0 && DateTime.Now.Minute == 0)
                {
                    CleanupOldLogs(logDir);
                }
            }
            catch
            {
                // 日志写入失败不应影响主程序
            }
        }

        private static string GetDefaultLogDir()
        {
            try
            {
                string assemblyDir = Path.GetDirectoryName(
                    System.Reflection.Assembly.GetExecutingAssembly().Location);
                return Path.Combine(assemblyDir ?? ".", "logs");
            }
            catch
            {
                return Path.Combine(Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData), "AIPolishAddin", "logs");
            }
        }

        /// <summary>
        /// 删除超过30天的旧日志
        /// </summary>
        private static void CleanupOldLogs(string logDir)
        {
            try
            {
                if (!Directory.Exists(logDir)) return;
                var cutoff = DateTime.Now.AddDays(-30);
                foreach (var file in Directory.GetFiles(logDir, "AIPolish_*.log"))
                {
                    if (File.GetCreationTime(file) < cutoff)
                    {
                        File.Delete(file);
                    }
                }
            }
            catch
            {
                // 清理失败不重要
            }
        }
    }
}
