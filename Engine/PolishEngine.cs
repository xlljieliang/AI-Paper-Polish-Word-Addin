using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AIPolishCOMAddin.Infrastructure;
using AIPolishCOMAddin.Models;
using AIPolishCOMAddin.Utils;

namespace AIPolishCOMAddin.Engine
{
    /// <summary>
    /// 润色引擎核心 — 编排整段润色和逐句润色
    /// </summary>
    public class PolishEngine
    {
        private readonly LLMClient _llmClient;
        private readonly SettingsModel _settings;
        private readonly TermProtector _termProtector;

        // 进度报告回调
        public Action<int, int> OnSentenceProgress; // (current, total)
        public Action<string> OnStatusUpdate;
        public Action<string> OnLogMessage;

        public PolishEngine(SettingsModel settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _llmClient = new LLMClient(settings);
            _termProtector = new TermProtector(settings.CustomTerms);
        }

        /// <summary>
        /// 执行润色（主入口）
        /// </summary>
        public async Task<PolishResult> PolishAsync(
            string text,
            PolishMode mode,
            SectionType section,
            bool isSentenceMode,
            CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = new PolishResult
            {
                OriginalText = text,
                Mode = mode,
                Section = section,
                IsSentenceBySentence = isSentenceMode,
                TermProtectionEnabled = _settings.EnableTermProtect,
                CompletedAt = DateTime.Now
            };

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                // 1. 验证输入
                if (string.IsNullOrWhiteSpace(text))
                {
                    result.IsSuccess = false;
                    result.ErrorMessage = "输入文本为空。";
                    return result;
                }

                // 2. Token 估算
                int estimatedInputTokens = WordHelper.EstimateTokens(text);
                result.EstimatedInputTokens = estimatedInputTokens;
                OnLogMessage?.Invoke($"预估输入 token: {estimatedInputTokens:N0}");

                // 3. 长文本警告
                if (estimatedInputTokens > 4000)
                {
                    OnLogMessage?.Invoke("⚠️ 文本较长（超过4000 tokens），建议分段润色。");
                }

                if (isSentenceMode)
                {
                    // 逐句润色
                    result.SentenceDetails = await PolishSentenceBySentenceAsync(text, mode, section, cancellationToken);
                    result.PolishedText = ReconstructFromSentences(result.SentenceDetails);
                }
                else
                {
                    // 整段润色
                    result.PolishedText = await PolishWholeParagraphAsync(text, mode, section, cancellationToken);
                }

                // 4. 估算输出 token
                if (!string.IsNullOrEmpty(result.PolishedText))
                {
                    result.EstimatedOutputTokens = WordHelper.EstimateTokens(result.PolishedText);
                }

                result.IsSuccess = true;
            }
            catch (OperationCanceledException)
            {
                result.IsSuccess = false;
                result.ErrorMessage = "润色已取消。";
                OnLogMessage?.Invoke("⛔ 润色已取消");
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.ErrorMessage = $"润色失败: {ex.Message}";
                Logger.Error("PolishAsync 异常", ex);
                OnLogMessage?.Invoke($"❌ 错误: {ex.Message}");
            }
            finally
            {
                stopwatch.Stop();
                result.ElapsedMilliseconds = stopwatch.ElapsedMilliseconds;

                if (result.IsSuccess && !string.IsNullOrEmpty(result.PolishedText))
                {
                    // 记录 API 调用
                    Logger.ApiCall(
                        _settings.Model,
                        _settings.ApiBaseUrl,
                        result.EstimatedInputTokens,
                        result.EstimatedOutputTokens,
                        result.ElapsedMilliseconds,
                        true);
                }
            }

            return result;
        }

        /// <summary>
        /// 整段润色 — 一次性调用 LLM
        /// </summary>
        private async Task<string> PolishWholeParagraphAsync(
            string text, PolishMode mode, SectionType section, CancellationToken cancellationToken)
        {
            OnStatusUpdate?.Invoke("正在调用大模型进行整段润色...");
            OnLogMessage?.Invoke($"📤 发送请求 (Model: {_settings.Model})");

            // 术语保护
            string processedText = text;
            if (_settings.EnableTermProtect)
            {
                processedText = _termProtector.Protect(text);
                int termCount = CountProtectedTerms(processedText);
                if (termCount > 0)
                    OnLogMessage?.Invoke($"🛡️ 已保护 {termCount} 个专业术语");
            }

            // 构建 Prompt
            string systemPrompt = PromptLibrary.BuildSystemPrompt(mode, section, false);
            string userPrompt = PromptLibrary.BuildUserPrompt(processedText, mode);

            // 调用 LLM
            var response = await _llmClient.ChatCompletionAsync(systemPrompt, userPrompt, cancellationToken);

            if (!response.IsSuccess)
            {
                throw new LLMException(response.ErrorMessage, response.ErrorType, response.StatusCode);
            }

            OnLogMessage?.Invoke($"📥 收到回复 (输出 tokens: {response.OutputTokens})");

            // 术语还原
            string polishedText = response.Content;
            if (_settings.EnableTermProtect)
            {
                polishedText = _termProtector.Restore(polishedText);
            }

            return polishedText;
        }

        /// <summary>
        /// 逐句润色 — 逐句调用 LLM，支持进度和取消
        /// </summary>
        private async Task<List<SentencePolishDetail>> PolishSentenceBySentenceAsync(
            string text, PolishMode mode, SectionType section, CancellationToken cancellationToken)
        {
            var details = new List<SentencePolishDetail>();
            var sentences = SentenceSplitter.Split(text);
            int totalSentences = SentenceSplitter.CountSentences(sentences);
            int processedCount = 0;

            OnStatusUpdate?.Invoke($"逐句润色: 共 {totalSentences} 句");
            OnLogMessage?.Invoke($"📝 逐句润色模式（共 {totalSentences} 句，消耗更多 token）");

            // 逐句 mode 追加 suffix
            string sentenceModeSuffix = PromptLibrary.GetSentenceModeSuffix();

            for (int i = 0; i < sentences.Count; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                string sentence = sentences[i];

                // 跳过段落分隔标记
                if (sentence == "__PARAGRAPH_BREAK__")
                {
                    details.Add(new SentencePolishDetail
                    {
                        Index = i,
                        OriginalSentence = "",
                        PolishedSentence = "",
                        IsSuccess = true
                    });
                    continue;
                }

                if (string.IsNullOrWhiteSpace(sentence))
                {
                    continue;
                }

                processedCount++;
                OnSentenceProgress?.Invoke(processedCount, totalSentences);
                OnStatusUpdate?.Invoke($"正在处理第 {processedCount}/{totalSentences} 句...");
                OnLogMessage?.Invoke($"  第 {processedCount}/{totalSentences} 句: \"{sentence[..Math.Min(30, sentence.Length)]}...\"");

                // 术语保护（逐句）
                string processedSentence = sentence;
                if (_settings.EnableTermProtect)
                {
                    processedSentence = _termProtector.Protect(sentence);
                }

                // 构建 Prompt（加逐句后缀）
                string systemPrompt = PromptLibrary.BuildSystemPrompt(mode, section, true);
                systemPrompt += sentenceModeSuffix;

                string userPrompt = PromptLibrary.BuildUserPrompt(processedSentence, mode);

                // 调用 LLM
                var detail = new SentencePolishDetail
                {
                    Index = i,
                    OriginalSentence = sentence,
                    IsSuccess = false
                };

                try
                {
                    var response = await _llmClient.ChatCompletionAsync(systemPrompt, userPrompt, cancellationToken);

                    if (response.IsSuccess)
                    {
                        string polishedSentence = response.Content;
                        // 术语还原
                        if (_settings.EnableTermProtect)
                        {
                            polishedSentence = _termProtector.Restore(polishedSentence);
                        }

                        // 清理 LLM 可能添加的多余引号
                        polishedSentence = polishedSentence.Trim('"', '「', '」', '『', '』', '\'');

                        detail.PolishedSentence = polishedSentence;
                        detail.IsSuccess = true;
                        OnLogMessage?.Invoke($"  ✓ 第 {processedCount} 句完成");
                    }
                    else
                    {
                        detail.ErrorMessage = response.ErrorMessage;
                        detail.PolishedSentence = sentence; // 失败时保留原句
                        OnLogMessage?.Invoke($"  ⚠ 第 {processedCount} 句失败: {response.ErrorMessage}");
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    detail.ErrorMessage = ex.Message;
                    detail.PolishedSentence = sentence; // 失败时保留原句
                    OnLogMessage?.Invoke($"  ⚠ 第 {processedCount} 句异常: {ex.Message}");
                }

                details.Add(detail);

                // 如果不是最后一句，间隔一小段时间防止 API 限流
                if (i < sentences.Count - 1 && details.Count > 0)
                {
                    await Task.Delay(200, cancellationToken);
                }
            }

            OnStatusUpdate?.Invoke($"逐句润色完成: {processedCount}/{totalSentences} 句");
            return details;
        }

        /// <summary>
        /// 从逐句润色结果重建完整文本
        /// </summary>
        private string ReconstructFromSentences(List<SentencePolishDetail> details)
        {
            var result = new System.Text.StringBuilder();

            for (int i = 0; i < details.Count; i++)
            {
                var detail = details[i];

                // 段落分隔标记
                if (detail.OriginalSentence == "" && string.IsNullOrEmpty(detail.PolishedSentence))
                {
                    result.AppendLine();
                    result.AppendLine();
                    continue;
                }

                string text = detail.IsSuccess && !string.IsNullOrEmpty(detail.PolishedSentence)
                    ? detail.PolishedSentence
                    : detail.OriginalSentence;

                if (result.Length > 0)
                {
                    result.Append(" ");
                }
                result.Append(text);
            }

            return result.ToString().Trim();
        }

        /// <summary>
        /// 统计受保护术语数量
        /// </summary>
        private int CountProtectedTerms(string text)
        {
            int count = 0;
            int idx = 0;
            while ((idx = text.IndexOf("[[TERM_", idx, StringComparison.Ordinal)) >= 0)
            {
                count++;
                idx += 7;
            }
            return count;
        }

        public void Dispose()
        {
            _llmClient?.Dispose();
        }
    }
}
