using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace AIPolishCOMAddin.Engine
{
    /// <summary>
    /// 中英文分句器
    /// 支持：中文句号、英文句点、问号、感叹号、分号
    /// 保护缩写点（e.g., i.e., etc., vs., al. 不切分）
    /// </summary>
    public static class SentenceSplitter
    {
        // 常见缩写模式（这些点不视为句尾）
        private static readonly HashSet<string> _abbreviations = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "e.g", "i.e", "etc", "vs", "al", "et", "cf", "fig", "eq",
            "sec", "ref", "no", "vol", "pp", "eds", "dept", "st",
            "ave", "dr", "mr", "ms", "mrs", "jr", "sr", "prof",
            "inc", "ltd", "corp", "co", "assn", "bros",
            // 英文期刊缩写常见
            "ieee", "acm", "neurips", "iclr", "icml", "cvpr", "eccv", "aaai", "arxiv"
        };

        // 缩写+点结尾的模式，用于预检查
        private static readonly Regex _abbreviationPattern = new Regex(
            @"\b([A-Za-z]+\w*)\.$",
            RegexOptions.Compiled);

        // 句子边界正则
        // 匹配：中文句号、英文句点+空格、问号、感叹号、分号后跟换行
        // 但排除缩写点
        private static readonly Regex _sentenceEndPattern = new Regex(
            @"(?:[。！？；\?!;]\s*|(?<=[a-zA-Z])\.(?=\s+[A-Z]))",
            RegexOptions.Compiled);

        // 换行符保留模式：连续换行表示段落分隔
        private static readonly Regex _paragraphBreak = new Regex(
            @"\n\s*\n",
            RegexOptions.Compiled);

        /// <summary>
        /// 将文本分割为句子列表
        /// </summary>
        /// <param name="text">待分割文本</param>
        /// <returns>句子列表（保留空行标记段落分隔）</returns>
        public static List<string> Split(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new List<string>();

            var sentences = new List<string>();

            // 第一步：按段落分割
            string[] paragraphs = _paragraphBreak.Split(text);

            for (int pIdx = 0; pIdx < paragraphs.Length; pIdx++)
            {
                string paragraph = paragraphs[pIdx].Trim();
                if (string.IsNullOrWhiteSpace(paragraph))
                    continue;

                // 对每个段落进行分句
                var paraSentences = SplitParagraph(paragraph);
                sentences.AddRange(paraSentences);

                // 段落之间增加空标记
                if (pIdx < paragraphs.Length - 1)
                {
                    sentences.Add("__PARAGRAPH_BREAK__");
                }
            }

            return sentences;
        }

        /// <summary>
        /// 分割单个段落为句子
        /// </summary>
        private static List<string> SplitParagraph(string paragraph)
        {
            var sentences = new List<string>();
            int start = 0;
            int length = paragraph.Length;

            for (int i = 0; i < length; i++)
            {
                char c = paragraph[i];

                // 检查是否是句子结束符
                if (IsSentenceEndChar(c))
                {
                    // 检查是否是缩写点（不分割）
                    if (c == '.' && i > 0 && i + 1 < length)
                    {
                        // 检查缩写模式：单词缩写字 + .
                        int wordStart = i;
                        while (wordStart > start && char.IsLetter(paragraph[wordStart - 1]))
                            wordStart--;

                        if (wordStart < i)
                        {
                            string word = paragraph.Substring(wordStart, i - wordStart);
                            if (_abbreviations.Contains(word))
                            {
                                continue; // 缩写点，不分割
                            }
                        }

                        // 检查 "et al." 模式
                        if (i >= 3 && paragraph.Substring(i - 3, 3).Equals("al.", StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        // 检查数字后的点（如 "1.5" 不是句尾）
                        if (i > start && char.IsDigit(paragraph[i - 1]) && i + 1 < length && char.IsDigit(paragraph[i + 1]))
                        {
                            continue;
                        }
                    }

                    // 计算句子结束位置（包含标点）
                    int end = i + 1;

                    // 跳过结尾空白
                    while (end < length && char.IsWhiteSpace(paragraph[end]))
                    {
                        // 如果遇到换行，也作为句子边界
                        if (paragraph[end] == '\n' || paragraph[end] == '\r')
                        {
                            end++;
                            break;
                        }
                        end++;
                    }

                    string sentence = paragraph.Substring(start, end - start).Trim();
                    if (!string.IsNullOrWhiteSpace(sentence))
                    {
                        sentences.Add(sentence);
                    }

                    start = end;
                    i = end - 1; // 外层循环会++
                }
            }

            // 处理剩余文本
            if (start < length)
            {
                string remaining = paragraph.Substring(start).Trim();
                if (!string.IsNullOrWhiteSpace(remaining))
                {
                    // 如果剩余部分看起来很完整，作为一个句子
                    if (sentences.Count > 0 || remaining.Length > 2)
                    {
                        sentences.Add(remaining);
                    }
                    else if (sentences.Count == 0)
                    {
                        sentences.Add(remaining);
                    }
                }
            }

            // 如果段落没有分出任何句子（无标点），整体作为一个句子
            if (sentences.Count == 0 && !string.IsNullOrWhiteSpace(paragraph))
            {
                sentences.Add(paragraph.Trim());
            }

            return sentences;
        }

        /// <summary>
        /// 判断字符是否为句子结束符
        /// </summary>
        private static bool IsSentenceEndChar(char c)
        {
            return c == '。' || c == '！' || c == '？' ||
                   c == '!' || c == '?' || c == '；' || c == ';' ||
                   // 句点需要额外上下文判断，这里保守处理
                   (c == '\n' && c != '.');
        }

        /// <summary>
        /// 将带段落标记的句子列表重新组合为文本
        /// </summary>
        public static string Join(List<string> sentences)
        {
            if (sentences == null || sentences.Count == 0)
                return "";

            var result = new System.Text.StringBuilder();
            for (int i = 0; i < sentences.Count; i++)
            {
                if (sentences[i] == "__PARAGRAPH_BREAK__")
                {
                    result.AppendLine();
                    result.AppendLine();
                }
                else
                {
                    if (i > 0 && sentences[i - 1] != "__PARAGRAPH_BREAK__")
                    {
                        result.Append(" ");
                    }
                    result.Append(sentences[i]);
                }
            }

            return result.ToString().Trim();
        }

        /// <summary>
        /// 统计句子数量（不含段落标记）
        /// </summary>
        public static int CountSentences(List<string> sentences)
        {
            int count = 0;
            foreach (var s in sentences)
            {
                if (s != "__PARAGRAPH_BREAK__")
                    count++;
            }
            return count;
        }
    }
}
