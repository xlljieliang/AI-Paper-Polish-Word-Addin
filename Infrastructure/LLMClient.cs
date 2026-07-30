using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace AIPolishCOMAddin.Infrastructure
{
    /// <summary>
    /// LLM HTTP 客户端 — 兼容 OpenAI Chat-Completion API
    /// 支持 DeepSeek / OpenAI / Kimi / 智谱 / 通义千问 等
    /// 使用 .NET 内置 JavaScriptSerializer（无需 Newtonsoft.Json）
    /// </summary>
    public class LLMClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly SettingsModel _settings;
        private readonly JavaScriptSerializer _json;
        private const string CHAT_ENDPOINT = "/chat/completions";

        public LLMClient(SettingsModel settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(_settings.TimeoutSeconds > 0
                    ? _settings.TimeoutSeconds
                    : 60)
            };
            _json = new JavaScriptSerializer();
        }

        /// <summary>
        /// 发送聊天补全请求
        /// </summary>
        public async Task<LLMResponse> ChatCompletionAsync(
            string systemPrompt,
            string userPrompt,
            CancellationToken cancellationToken = default)
        {
            string baseUrl = _settings.ApiBaseUrl?.TrimEnd('/') ?? "";
            string url = baseUrl + CHAT_ENDPOINT;
            string apiKey = _settings.ApiKey ?? "";
            string model = _settings.Model ?? "deepseek-chat";
            double temperature = _settings.Temperature;
            int maxTokens = _settings.MaxTokens > 0 ? _settings.MaxTokens : 2048;

            // 手动构建 JSON（避免外部依赖）
            string jsonContent = BuildChatCompletionJson(systemPrompt, userPrompt, model, temperature, maxTokens);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            }

            int retryCount = Math.Max(0, _settings.RetryCount);
            int attempt = 0;
            Exception lastException = null;

            while (attempt <= retryCount)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var response = await _httpClient.PostAsync(url, httpContent, cancellationToken)
                        .ConfigureAwait(false);

                    string responseBody = await response.Content
                        .ReadAsStringAsync()
                        .ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        return ParseSuccessResponse(responseBody);
                    }

                    var errorInfo = ParseErrorResponse(responseBody, (int)response.StatusCode);
                    lastException = new LLMException(errorInfo.ErrorMessage, errorInfo.ErrorType, (int)response.StatusCode);

                    // 401/403 不重试
                    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                        response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                    {
                        break;
                    }

                    // 429 等待后重试
                    if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests && attempt < retryCount)
                    {
                        await Task.Delay(2000 * (attempt + 1), cancellationToken)
                            .ConfigureAwait(false);
                    }
                }
                catch (OperationCanceledException) { throw; }
                catch (TaskCanceledException)
                {
                    lastException = new LLMException("请求超时，请检查网络连接或增大超时时间。", "timeout", 0);
                }
                catch (HttpRequestException ex)
                {
                    lastException = new LLMException($"网络连接失败: {ex.Message}", "network_error", 0);
                }
                catch (Exception ex)
                {
                    lastException = new LLMException($"未知错误: {ex.Message}", "unknown", 0);
                }

                attempt++;
                if (attempt <= retryCount)
                {
                    await Task.Delay(1000 * (int)Math.Pow(2, attempt - 1), cancellationToken)
                        .ConfigureAwait(false);
                }
            }

            return new LLMResponse
            {
                IsSuccess = false,
                ErrorMessage = lastException?.Message ?? "未知错误",
                ErrorType = (lastException as LLMException)?.ErrorType ?? "unknown",
                StatusCode = (lastException as LLMException)?.StatusCode ?? 0
            };
        }

        /// <summary>
        /// 测试 API 连通性
        /// </summary>
        public async Task<LLMResponse> TestConnectionAsync()
        {
            try
            {
                string baseUrl = _settings.ApiBaseUrl?.TrimEnd('/') ?? "";
                string url = baseUrl + CHAT_ENDPOINT;
                string apiKey = _settings.ApiKey ?? "";
                string model = _settings.Model ?? "deepseek-chat";

                string jsonContent = $@"{{""model"":""{EscapeJson(model)}"",""messages"":[{{""role"":""user"",""content"":""Hi""}}],""max_tokens"":5,""stream"":false}}";
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                _httpClient.DefaultRequestHeaders.Remove("Authorization");
                if (!string.IsNullOrWhiteSpace(apiKey))
                {
                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
                }

                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15)))
                {
                    var response = await _httpClient.PostAsync(url, httpContent, cts.Token)
                        .ConfigureAwait(false);

                    string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        return new LLMResponse { IsSuccess = true, Content = "✅ 连接成功！API 配置正确。" };
                    }

                    var error = ParseErrorResponse(responseBody, (int)response.StatusCode);
                    return new LLMResponse
                    {
                        IsSuccess = false,
                        ErrorMessage = error.ErrorMessage,
                        ErrorType = error.ErrorType,
                        StatusCode = (int)response.StatusCode
                    };
                }
            }
            catch (OperationCanceledException)
            {
                return new LLMResponse { IsSuccess = false, ErrorMessage = "连接超时，请检查 API Base URL 是否正确。", ErrorType = "timeout" };
            }
            catch (HttpRequestException ex)
            {
                return new LLMResponse { IsSuccess = false, ErrorMessage = $"网络错误: {ex.Message}", ErrorType = "network_error" };
            }
            catch (Exception ex)
            {
                return new LLMResponse { IsSuccess = false, ErrorMessage = $"连接测试失败: {ex.Message}", ErrorType = "unknown" };
            }
        }

        /// <summary>
        /// 手动构建 Chat Completion JSON（无外部依赖）
        /// </summary>
        private string BuildChatCompletionJson(string systemPrompt, string userPrompt,
            string model, double temperature, int maxTokens)
        {
            var sb = new StringBuilder();
            sb.Append("{");
            sb.Append($"\"model\":\"{EscapeJson(model)}\",");
            sb.Append("\"messages\":[");
            sb.Append($"{{\"role\":\"system\",\"content\":\"{EscapeJson(systemPrompt)}\"}},");
            sb.Append($"{{\"role\":\"user\",\"content\":\"{EscapeJson(userPrompt)}\"}}");
            sb.Append("],");
            sb.Append($"\"temperature\":{temperature:F2},");
            sb.Append($"\"max_tokens\":{maxTokens},");
            sb.Append("\"stream\":false");
            sb.Append("}");
            return sb.ToString();
        }

        /// <summary>
        /// JSON 字符串转义
        /// </summary>
        private static string EscapeJson(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return text
                .Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t")
                .Replace("\b", "\\b")
                .Replace("\f", "\\f");
        }

        /// <summary>
        /// 解析成功响应
        /// </summary>
        private LLMResponse ParseSuccessResponse(string json)
        {
            try
            {
                var dict = _json.Deserialize<Dictionary<string, object>>(json);

                var choices = dict.GetValueOrDefault("choices") as object[];
                string content = "";
                int inputTokens = 0, outputTokens = 0;

                if (choices != null && choices.Length > 0)
                {
                    var firstChoice = choices[0] as Dictionary<string, object>;
                    var message = firstChoice?.GetValueOrDefault("message") as Dictionary<string, object>;
                    content = message?.GetValueOrDefault("content") as string ?? "";
                }

                var usage = dict.GetValueOrDefault("usage") as Dictionary<string, object>;
                if (usage != null)
                {
                    inputTokens = Convert.ToInt32(usage.GetValueOrDefault("prompt_tokens") ?? 0);
                    outputTokens = Convert.ToInt32(usage.GetValueOrDefault("completion_tokens") ?? 0);
                }

                return new LLMResponse
                {
                    IsSuccess = true,
                    Content = content.Trim(),
                    InputTokens = inputTokens,
                    OutputTokens = outputTokens
                };
            }
            catch (Exception ex)
            {
                return new LLMResponse
                {
                    IsSuccess = false,
                    ErrorMessage = $"解析响应失败: {ex.Message}",
                    ErrorType = "parse_error"
                };
            }
        }

        /// <summary>
        /// 解析错误响应
        /// </summary>
        private (string ErrorMessage, string ErrorType) ParseErrorResponse(string json, int statusCode)
        {
            try
            {
                var dict = _json.Deserialize<Dictionary<string, object>>(json);
                var error = dict.GetValueOrDefault("error") as Dictionary<string, object>;

                string message = error?.GetValueOrDefault("message") as string ?? json;
                string type = error?.GetValueOrDefault("type") as string ?? "api_error";

                if (message.Length > 200)
                    message = message.Substring(0, 200) + "...";

                return (message, type);
            }
            catch
            {
                return statusCode switch
                {
                    400 => ("请求格式错误，请检查 API 配置。", "bad_request"),
                    401 => ("API Key 无效或未提供，请检查 API Key。", "auth_error"),
                    403 => ("无权限访问，请检查 API Key 权限。", "forbidden"),
                    404 => ("API 端点不存在，请检查 Base URL。", "not_found"),
                    429 => ("请求太频繁，已被限流，请稍后重试。", "rate_limit"),
                    500 => ("模型服务端错误，请稍后重试。", "server_error"),
                    _ => ($"HTTP {statusCode}: 请求失败", "http_error")
                };
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }

    /// <summary>
    /// LLM 响应数据模型
    /// </summary>
    public class LLMResponse
    {
        public bool IsSuccess { get; set; }
        public string Content { get; set; }
        public int InputTokens { get; set; }
        public int OutputTokens { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorType { get; set; }
        public int StatusCode { get; set; }
    }

    /// <summary>
    /// LLM 异常类
    /// </summary>
    public class LLMException : Exception
    {
        public string ErrorType { get; }
        public int StatusCode { get; }

        public LLMException(string message, string errorType, int statusCode) : base(message)
        {
            ErrorType = errorType;
            StatusCode = statusCode;
        }
    }

    /// <summary>
    /// Dictionary 扩展方法（简化取值）
    /// </summary>
    internal static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TValue>(this Dictionary<string, object> dict, string key)
        {
            if (dict != null && dict.TryGetValue(key, out var value))
            {
                return (TValue)value;
            }
            return default;
        }
    }
}
