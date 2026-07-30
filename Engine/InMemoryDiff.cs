using System;
using System.Collections.Generic;
using System.Text;

namespace AIPolishCOMAddin.Engine
{
    /// <summary>
    /// 文本差异比对模块
    /// 基于最长公共子序列（LCS）算法实现词级 diff
    /// 输出操作序列：保留 / 删除 / 插入
    /// </summary>
    public static class InMemoryDiff
    {
        /// <summary>
        /// Diff 操作类型
        /// </summary>
        public enum DiffOperation
        {
            Equal,      // 保留
            Delete,     // 删除（原文中有，润色后没有）
            Insert      // 插入（原文中没有，润色后有）
        }

        /// <summary>
        /// Diff 操作单元
        /// </summary>
        public class DiffUnit
        {
            public DiffOperation Operation { get; set; }
            public string Text { get; set; }
            public int OriginalIndex { get; set; }  // 在原文中的起始索引
            public int Length { get; set; }          // 文本长度

            public override string ToString()
            {
                string prefix = Operation switch
                {
                    DiffOperation.Equal => "  ",
                    DiffOperation.Delete => "- ",
                    DiffOperation.Insert => "+ ",
                    _ => "? "
                };
                return $"{prefix}\"{Text}\"";
            }
        }

        /// <summary>
        /// 计算两个文本的词级 Diff
        /// </summary>
        /// <param name="original">原始文本</param>
        /// <param name="modified">修改后文本</param>
        /// <returns>Diff 操作序列</returns>
        public static List<DiffUnit> ComputeDiff(string original, string modified)
        {
            if (original == null) original = "";
            if (modified == null) modified = "";

            // 分词
            string[] originalWords = Tokenize(original);
            string[] modifiedWords = Tokenize(modified);

            // 计算 LCS
            var lcs = ComputeLCS(originalWords, modifiedWords);

            // 根据 LCS 生成 diff 序列
            return GenerateDiffSequence(originalWords, modifiedWords, lcs);
        }

        /// <summary>
        /// 将文本分词（按空白和标点，保留分隔符用于重建）
        /// </summary>
        private static string[] Tokenize(string text)
        {
            if (string.IsNullOrEmpty(text))
                return Array.Empty<string>();

            var tokens = new List<string>();
            var current = new StringBuilder();

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];

                // 空白字符作为分隔
                if (char.IsWhiteSpace(c))
                {
                    if (current.Length > 0)
                    {
                        tokens.Add(current.ToString());
                        current.Clear();
                    }
                    tokens.Add(c.ToString());
                }
                // 标点符号（中文和英文标点）
                else if (char.IsPunctuation(c) || c > 0x4E00 && c < 0x9FFF)
                {
                    // 中文汉字作为独立 token（方便逐字对比）
                    if (c >= 0x4E00 && c <= 0x9FFF)
                    {
                        if (current.Length > 0)
                        {
                            tokens.Add(current.ToString());
                            current.Clear();
                        }
                        tokens.Add(c.ToString());
                    }
                    else
                    {
                        current.Append(c);
                    }
                }
                else
                {
                    current.Append(c);
                }
            }

            if (current.Length > 0)
            {
                tokens.Add(current.ToString());
            }

            return tokens.ToArray();
        }

        /// <summary>
        /// 计算最长公共子序列（LCS）
        /// 使用标准 DP 算法
        /// </summary>
        private static int[,] ComputeLCS(string[] left, string[] right)
        {
            int m = left.Length;
            int n = right.Length;
            var dp = new int[m + 1, n + 1];

            for (int i = 1; i <= m; i++)
            {
                for (int j = 1; j <= n; j++)
                {
                    if (string.Equals(left[i - 1], right[j - 1], StringComparison.OrdinalIgnoreCase))
                    {
                        dp[i, j] = dp[i - 1, j - 1] + 1;
                    }
                    else
                    {
                        dp[i, j] = Math.Max(dp[i - 1, j], dp[i, j - 1]);
                    }
                }
            }

            return dp;
        }

        /// <summary>
        /// 根据 LCS DP 表生成 diff 操作序列
        /// </summary>
        private static List<DiffUnit> GenerateDiffSequence(
            string[] originalWords,
            string[] modifiedWords,
            int[,] lcs)
        {
            var units = new List<DiffUnit>();
            int i = originalWords.Length;
            int j = modifiedWords.Length;

            // 反向追踪 LCS
            var reverseOps = new List<DiffUnit>();

            while (i > 0 || j > 0)
            {
                if (i > 0 && j > 0 && string.Equals(originalWords[i - 1], modifiedWords[j - 1], StringComparison.OrdinalIgnoreCase))
                {
                    reverseOps.Add(new DiffUnit
                    {
                        Operation = DiffOperation.Equal,
                        Text = originalWords[i - 1],
                        OriginalIndex = i - 1,
                        Length = originalWords[i - 1].Length
                    });
                    i--;
                    j--;
                }
                else if (j > 0 && (i == 0 || lcs[i, j - 1] >= lcs[i - 1, j]))
                {
                    reverseOps.Add(new DiffUnit
                    {
                        Operation = DiffOperation.Insert,
                        Text = modifiedWords[j - 1],
                        OriginalIndex = -1,
                        Length = modifiedWords[j - 1].Length
                    });
                    j--;
                }
                else if (i > 0)
                {
                    reverseOps.Add(new DiffUnit
                    {
                        Operation = DiffOperation.Delete,
                        Text = originalWords[i - 1],
                        OriginalIndex = i - 1,
                        Length = originalWords[i - 1].Length
                    });
                    i--;
                }
            }

            // 反转得到正向序列
            reverseOps.Reverse();

            // 合并相邻的同类型操作
            return MergeAdjacentEqualOps(reverseOps);
        }

        /// <summary>
        /// 合并相邻的相同类型操作
        /// 例如相邻的多个 Equal 合并为一个、相邻的多个 Delete 合并为一个
        /// </summary>
        private static List<DiffUnit> MergeAdjacentEqualOps(List<DiffUnit> ops)
        {
            var merged = new List<DiffUnit>();

            foreach (var op in ops)
            {
                if (merged.Count > 0)
                {
                    var last = merged[merged.Count - 1];
                    if (last.Operation == op.Operation)
                    {
                        last.Text += op.Text;
                        last.Length += op.Length;
                        continue;
                    }
                }
                merged.Add(new DiffUnit
                {
                    Operation = op.Operation,
                    Text = op.Text,
                    OriginalIndex = op.OriginalIndex,
                    Length = op.Length
                });
            }

            return merged;
        }

        /// <summary>
        /// 生成带高亮标记的 diff 文本（用于 UI 预览）
        /// </summary>
        public static string GetDiffPreview(string original, string modified)
        {
            var ops = ComputeDiff(original, modified);

            var sb = new StringBuilder();
            // 原始文本（带删除标记）
            sb.AppendLine("【原始文本】");
            foreach (var op in ops)
            {
                if (op.Operation == DiffOperation.Delete)
                {
                    sb.Append($"[--{op.Text}--]");
                }
                else if (op.Operation == DiffOperation.Equal)
                {
                    sb.Append(op.Text);
                }
            }

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("【润色文本】");
            foreach (var op in ops)
            {
                if (op.Operation == DiffOperation.Insert)
                {
                    sb.Append($"[++{op.Text}++]");
                }
                else if (op.Operation == DiffOperation.Equal)
                {
                    sb.Append(op.Text);
                }
            }

            return sb.ToString();
        }
    }
}
