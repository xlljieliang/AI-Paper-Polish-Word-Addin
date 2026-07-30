using System;
using System.Text.RegularExpressions;
using Microsoft.Office.Interop.Word;

namespace AIPolishCOMAddin.Utils
{
    /// <summary>
    /// Word 文档交互辅助工具
    /// 负责获取选中文本、保护特殊内容、撤销管理等
    /// </summary>
    public static class WordHelper
    {
        /// <summary>
        /// 获取当前选中的文本内容
        /// </summary>
        public static string GetSelectedText(Application wordApp)
        {
            if (wordApp == null || wordApp.Selection == null)
                return null;

            try
            {
                if (wordApp.Selection.Type == WdSelectionType.wdSelectionNormal)
                {
                    return wordApp.Selection.Range.Text.TrimEnd('\r', '\n');
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 检查是否有选中文本
        /// </summary>
        public static bool HasSelection(Application wordApp)
        {
            if (wordApp == null || wordApp.Selection == null)
                return false;

            try
            {
                return wordApp.Selection.Type == WdSelectionType.wdSelectionNormal
                       && !string.IsNullOrWhiteSpace(wordApp.Selection.Range.Text);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 检查文档是否受保护（限制编辑）
        /// </summary>
        public static bool IsDocumentProtected(Application wordApp)
        {
            try
            {
                if (wordApp.ActiveDocument == null) return false;
                return wordApp.ActiveDocument.ProtectionType != WdProtectionType.wdNoProtection;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 检查选中内容是否包含复杂对象（公式域等）
        /// 返回 true 表示包含可能出问题的内容
        /// </summary>
        public static bool ContainsComplexContent(Range range)
        {
            try
            {
                // 检查是否包含 OMath（公式）
                if (range.OMaths != null && range.OMaths.Count > 0)
                    return true;

                // 检查是否包含域（交叉引用等）
                if (range.Fields != null && range.Fields.Count > 0)
                    return true;

                // 检查是否包含脚注/尾注
                if ((range.Footnotes != null && range.Footnotes.Count > 0) ||
                    (range.Endnotes != null && range.Endnotes.Count > 0))
                    return true;

                // 检查是否包含批注
                if (range.Comments != null && range.Comments.Count > 0)
                    return true;

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取选中文本长度（字符数）
        /// </summary>
        public static int GetSelectionLength(Application wordApp)
        {
            try
            {
                return wordApp.Selection.Range.Text.TrimEnd('\r', '\n').Length;
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// 简化文本用于 Token 估算
        /// 中文约 1 token ≈ 1.5 字，英文约 1 token ≈ 4 字符
        /// </summary>
        public static int EstimateTokens(string text)
        {
            if (string.IsNullOrEmpty(text)) return 0;

            int chineseChars = Regex.Matches(text, @"[一-鿿]").Count;
            int otherChars = text.Length - chineseChars;

            // 中文约 1.5 字/token，英文约 4 字符/token
            return (int)(chineseChars / 1.5 + otherChars / 3.5);
        }

        /// <summary>
        /// 检查文本是否过长，超过建议限制
        /// </summary>
        public static bool IsTextTooLong(string text, int maxTokens = 4000)
        {
            return EstimateTokens(text) > maxTokens;
        }

        /// <summary>
        /// 执行 Word 撤销操作
        /// </summary>
        public static void Undo(Application wordApp)
        {
            try
            {
                if (wordApp.ActiveDocument != null)
                {
                    wordApp.ActiveDocument.Undo();
                }
            }
            catch (Exception)
            {
                // 静默处理撤销异常
            }
        }

        /// <summary>
        /// 在 Word 状态栏显示消息
        /// </summary>
        public static void SetStatusBar(Application wordApp, string message)
        {
            try
            {
                wordApp.StatusBar = message;
            }
            catch (Exception)
            {
                // 静默处理
            }
        }
    }
}
