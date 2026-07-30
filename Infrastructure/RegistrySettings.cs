using System;
using Microsoft.Win32;

namespace AIPolishCOMAddin.Infrastructure
{
    /// <summary>
    /// 设置持久化模块 — Windows 注册表读写
    /// 存储路径: HKEY_CURRENT_USER\Software\AIPaperPolishAddin
    /// </summary>
    public class RegistrySettings
    {
        private const string REGISTRY_ROOT = @"Software\AIPaperPolishAddin";

        // --- API 配置键名 ---
        public const string KEY_API_BASE_URL = "ApiBaseUrl";
        public const string KEY_API_KEY = "ApiKey";
        public const string KEY_MODEL = "Model";
        public const string KEY_TEMPERATURE = "Temperature";
        public const string KEY_MAX_TOKENS = "MaxTokens";
        public const string KEY_TIMEOUT = "TimeoutSeconds";
        public const string KEY_RETRY_COUNT = "RetryCount";

        // --- 功能开关键名 ---
        public const string KEY_ENABLE_TERM_PROTECT = "EnableTermProtect";
        public const string KEY_ENABLE_TRACK_CHANGES = "EnableTrackChanges";
        public const string KEY_ENABLE_SENTENCE_MODE = "EnableSentenceMode";

        // --- 自定义术语 ---
        public const string KEY_CUSTOM_TERMS = "CustomTerms";

        // --- 默认值 ---
        public static class Defaults
        {
            public static string ApiBaseUrl => "https://api.deepseek.com/v1";
            public static string ApiKey => "";
            public static string Model => "deepseek-chat";
            public static double Temperature => 0.1;
            public static int MaxTokens => 2048;
            public static int TimeoutSeconds => 60;
            public static int RetryCount => 2;
            public static bool EnableTermProtect => true;
            public static bool EnableTrackChanges => true;
            public static bool EnableSentenceMode => false;
            public static string CustomTerms => "";
        }

        /// <summary>
        /// 检查注册表中是否有已保存的配置
        /// </summary>
        public static bool HasSettings()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(REGISTRY_ROOT))
            {
                return key != null
                    && key.GetValue(KEY_API_BASE_URL) != null
                    && !string.IsNullOrWhiteSpace(key.GetValue(KEY_API_BASE_URL) as string);
            }
        }

        /// <summary>
        /// 读取所有设置到 SettingsModel
        /// </summary>
        public static SettingsModel LoadAll()
        {
            var model = new SettingsModel();
            using (var key = Registry.CurrentUser.OpenSubKey(REGISTRY_ROOT))
            {
                if (key == null) return model; // 返回默认值

                model.ApiBaseUrl = GetString(key, KEY_API_BASE_URL, Defaults.ApiBaseUrl);
                model.ApiKey = GetString(key, KEY_API_KEY, Defaults.ApiKey);
                model.Model = GetString(key, KEY_MODEL, Defaults.Model);
                model.Temperature = GetDouble(key, KEY_TEMPERATURE, Defaults.Temperature);
                model.MaxTokens = GetInt(key, KEY_MAX_TOKENS, Defaults.MaxTokens);
                model.TimeoutSeconds = GetInt(key, KEY_TIMEOUT, Defaults.TimeoutSeconds);
                model.RetryCount = GetInt(key, KEY_RETRY_COUNT, Defaults.RetryCount);
                model.EnableTermProtect = GetBool(key, KEY_ENABLE_TERM_PROTECT, Defaults.EnableTermProtect);
                model.EnableTrackChanges = GetBool(key, KEY_ENABLE_TRACK_CHANGES, Defaults.EnableTrackChanges);
                model.EnableSentenceMode = GetBool(key, KEY_ENABLE_SENTENCE_MODE, Defaults.EnableSentenceMode);
                model.CustomTerms = GetString(key, KEY_CUSTOM_TERMS, Defaults.CustomTerms);
            }
            return model;
        }

        /// <summary>
        /// 保存所有设置到注册表
        /// </summary>
        public static void SaveAll(SettingsModel model)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(REGISTRY_ROOT))
            {
                if (key == null)
                    throw new InvalidOperationException("无法创建注册表键，请检查权限。");

                SetString(key, KEY_API_BASE_URL, model.ApiBaseUrl ?? Defaults.ApiBaseUrl);
                SetString(key, KEY_API_KEY, model.ApiKey ?? Defaults.ApiKey);
                SetString(key, KEY_MODEL, model.Model ?? Defaults.Model);
                SetDouble(key, KEY_TEMPERATURE, model.Temperature);
                SetInt(key, KEY_MAX_TOKENS, model.MaxTokens);
                SetInt(key, KEY_TIMEOUT, model.TimeoutSeconds);
                SetInt(key, KEY_RETRY_COUNT, model.RetryCount);
                SetBool(key, KEY_ENABLE_TERM_PROTECT, model.EnableTermProtect);
                SetBool(key, KEY_ENABLE_TRACK_CHANGES, model.EnableTrackChanges);
                SetBool(key, KEY_ENABLE_SENTENCE_MODE, model.EnableSentenceMode);
                SetString(key, KEY_CUSTOM_TERMS, model.CustomTerms ?? Defaults.CustomTerms);
            }
        }

        /// <summary>
        /// 重置所有设置（删除整个注册表键，恢复默认）
        /// </summary>
        public static void ResetAll()
        {
            try
            {
                Registry.CurrentUser.DeleteSubKeyTree(REGISTRY_ROOT, false);
            }
            catch (ArgumentException)
            {
                // 键不存在，忽略
            }
        }

        // --- 内部读写辅助方法 ---

        private static string GetString(RegistryKey key, string name, string defaultValue)
        {
            var val = key.GetValue(name);
            return val != null ? val.ToString() : defaultValue;
        }

        private static int GetInt(RegistryKey key, string name, int defaultValue)
        {
            var val = key.GetValue(name);
            if (val is int intVal) return intVal;
            if (val != null && int.TryParse(val.ToString(), out int parsed)) return parsed;
            return defaultValue;
        }

        private static double GetDouble(RegistryKey key, string name, double defaultValue)
        {
            var val = key.GetValue(name);
            if (val is int intVal) return intVal / 100.0; // 存储为 int(value*100)
            if (val != null && double.TryParse(val.ToString(), out double parsed)) return parsed;
            return defaultValue;
        }

        private static bool GetBool(RegistryKey key, string name, bool defaultValue)
        {
            var val = key.GetValue(name);
            if (val is int intVal) return intVal != 0;
            return defaultValue;
        }

        private static void SetString(RegistryKey key, string name, string value)
        {
            key.SetValue(name, value ?? "");
        }

        private static void SetInt(RegistryKey key, string name, int value)
        {
            key.SetValue(name, value, RegistryValueKind.DWord);
        }

        private static void SetDouble(RegistryKey key, string name, double value)
        {
            // 存储为整数（温度*100），避免浮点数精度问题
            key.SetValue(name, (int)(value * 100), RegistryValueKind.DWord);
        }

        private static void SetBool(RegistryKey key, string name, bool value)
        {
            key.SetValue(name, value ? 1 : 0, RegistryValueKind.DWord);
        }
    }

    /// <summary>
    /// 设置数据模型
    /// </summary>
    public class SettingsModel
    {
        public string ApiBaseUrl { get; set; }
        public string ApiKey { get; set; }
        public string Model { get; set; }
        public double Temperature { get; set; }
        public int MaxTokens { get; set; }
        public int TimeoutSeconds { get; set; }
        public int RetryCount { get; set; }
        public bool EnableTermProtect { get; set; }
        public bool EnableTrackChanges { get; set; }
        public bool EnableSentenceMode { get; set; }
        public string CustomTerms { get; set; }

        public SettingsModel()
        {
            ApiBaseUrl = RegistrySettings.Defaults.ApiBaseUrl;
            ApiKey = RegistrySettings.Defaults.ApiKey;
            Model = RegistrySettings.Defaults.Model;
            Temperature = RegistrySettings.Defaults.Temperature;
            MaxTokens = RegistrySettings.Defaults.MaxTokens;
            TimeoutSeconds = RegistrySettings.Defaults.TimeoutSeconds;
            RetryCount = RegistrySettings.Defaults.RetryCount;
            EnableTermProtect = RegistrySettings.Defaults.EnableTermProtect;
            EnableTrackChanges = RegistrySettings.Defaults.EnableTrackChanges;
            EnableSentenceMode = RegistrySettings.Defaults.EnableSentenceMode;
            CustomTerms = RegistrySettings.Defaults.CustomTerms;
        }
    }
}
