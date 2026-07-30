using System;
using System.Collections.Generic;

namespace AIPolishCOMAddin.Models
{
    /// <summary>
    /// 润色结果数据模型
    /// </summary>
    public class PolishResult
    {
        /// <summary>原始文本</summary>
        public string OriginalText { get; set; }

        /// <summary>润色后文本</summary>
        public string PolishedText { get; set; }

        /// <summary>使用的润色模式</summary>
        public PolishMode Mode { get; set; }

        /// <summary>使用的章节类型</summary>
        public SectionType Section { get; set; }

        /// <summary>是否逐句润色</summary>
        public bool IsSentenceBySentence { get; set; }

        /// <summary>是否启用了术语保护</summary>
        public bool TermProtectionEnabled { get; set; }

        /// <summary>各句润色明细（逐句模式时）</summary>
        public List<SentencePolishDetail> SentenceDetails { get; set; }

        /// <summary>预估输入Token数</summary>
        public int EstimatedInputTokens { get; set; }

        /// <summary>预估输出Token数</summary>
        public int EstimatedOutputTokens { get; set; }

        /// <summary>耗时（毫秒）</summary>
        public long ElapsedMilliseconds { get; set; }

        /// <summary>是否成功</summary>
        public bool IsSuccess { get; set; }

        /// <summary>错误信息（失败时）</summary>
        public string ErrorMessage { get; set; }

        /// <summary>完成时间</summary>
        public DateTime CompletedAt { get; set; }

        public PolishResult()
        {
            SentenceDetails = new List<SentencePolishDetail>();
            CompletedAt = DateTime.Now;
        }
    }

    /// <summary>
    /// 逐句润色的单句明细
    /// </summary>
    public class SentencePolishDetail
    {
        /// <summary>句子序号</summary>
        public int Index { get; set; }

        /// <summary>原始句子</summary>
        public string OriginalSentence { get; set; }

        /// <summary>润色后句子</summary>
        public string PolishedSentence { get; set; }

        /// <summary>是否成功</summary>
        public bool IsSuccess { get; set; }

        /// <summary>错误信息</summary>
        public string ErrorMessage { get; set; }
    }
}
