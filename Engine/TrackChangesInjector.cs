using System;
using System.Collections.Generic;
using AIPolishCOMAddin.Engine;
using AIPolishCOMAddin.Utils;
using Microsoft.Office.Interop.Word;

namespace AIPolishCOMAddin.Engine
{
    /// <summary>
    /// Word 原生 TrackChanges 修订注入模块
    /// 核心功能：将 diff 结果写入 Word，以修订标记显示红删绿增
    /// 对标小绿鲸的修订体验
    /// </summary>
    public class TrackChangesInjector
    {
        private readonly Application _wordApp;

        public TrackChangesInjector(Application wordApp)
        {
            _wordApp = wordApp ?? throw new ArgumentNullException(nameof(wordApp));
        }

        /// <summary>
        /// 执行修订注入
        /// </summary>
        /// <param name="original">原始文本</param>
        /// <param name="polished">润色后文本</param>
        /// <param name="useTrackChanges">是否使用 Word 修订模式</param>
        /// <returns>是否成功</returns>
        public bool ApplyChanges(string original, string polished, bool useTrackChanges)
        {
            try
            {
                if (_wordApp.ActiveDocument == null || _wordApp.Selection == null)
                    return false;

                if (useTrackChanges)
                {
                    return ApplyWithTrackChanges(original, polished);
                }
                else
                {
                    return ApplyDirectReplace(polished);
                }
            }
            catch (Exception ex)
            {
                Infrastructure.Logger.Error("TrackChangesInjector.ApplyChanges 失败", ex);
                return false;
            }
        }

        /// <summary>
        /// 使用 Word 修订模式逐 diff 操作写入
        /// </summary>
        private bool ApplyWithTrackChanges(string original, string polished)
        {
            try
            {
                Range selectionRange = _wordApp.Selection.Range;
                string selectedText = selectionRange.Text.TrimEnd('\r', '\n');

                if (string.IsNullOrEmpty(selectedText))
                    return false;

                // 计算 diff
                var diffUnits = InMemoryDiff.ComputeDiff(original, polished);
                if (diffUnits == null || diffUnits.Count == 0)
                    return false;

                // 保存当前修订设置
                bool originalTrackChanges = _wordApp.ActiveDocument.TrackRevisions;

                try
                {
                    // 确保开启修订
                    _wordApp.ActiveDocument.TrackRevisions = true;

                    // 获取选区的起始位置
                    int startPos = selectionRange.Start;

                    // 逐操作处理
                    int currentOffset = 0;

                    foreach (var unit in diffUnits)
                    {
                        if (string.IsNullOrEmpty(unit.Text))
                            continue;

                        // 计算当前操作对应的 Range
                        int rangeStart = startPos + currentOffset;
                        int textLength = unit.Text.Length;

                        Range opRange;
                        try
                        {
                            opRange = _wordApp.ActiveDocument.Range(rangeStart, rangeStart + textLength);
                        }
                        catch
                        {
                            // 如果 Range 超出文档范围，跳过
                            break;
                        }

                        switch (unit.Operation)
                        {
                            case InMemoryDiff.DiffOperation.Equal:
                                // 保留：跳过（不修改）
                                currentOffset += textLength;
                                break;

                            case InMemoryDiff.DiffOperation.Delete:
                                // 删除：替换为空（修订标记删除）
                                try
                                {
                                    if (opRange.Text.Length > 0)
                                    {
                                        // 先选中该范围再删除，确保修订标记生效
                                        opRange.Select();
                                        _wordApp.Selection.Delete(Unit: WdUnits.wdCharacter, Count: 1);
                                        // 删除后选区位置不变，后续插入在当前Offset位置
                                    }
                                }
                                catch
                                {
                                    // 如果某个删除失败，用字符逐个删除
                                    for (int k = 0; k < textLength; k++)
                                    {
                                        try
                                        {
                                            var charRange = _wordApp.ActiveDocument.Range(
                                                startPos + currentOffset,
                                                startPos + currentOffset + 1);
                                            charRange.Select();
                                            _wordApp.Selection.Delete(Unit: WdUnits.wdCharacter, Count: 1);
                                        }
                                        catch { break; }
                                    }
                                }
                                break;

                            case InMemoryDiff.DiffOperation.Insert:
                                // 插入：在当前位置插入文本（修订标记插入）
                                try
                                {
                                    var insertPoint = _wordApp.ActiveDocument.Range(
                                        startPos + currentOffset,
                                        startPos + currentOffset);
                                    insertPoint.Select();
                                    _wordApp.Selection.TypeText(unit.Text);
                                    currentOffset += textLength;
                                }
                                catch
                                {
                                    // 插入失败，尝试逐字符插入
                                    try
                                    {
                                        foreach (char ch in unit.Text)
                                        {
                                            var pt = _wordApp.ActiveDocument.Range(
                                                startPos + currentOffset,
                                                startPos + currentOffset);
                                            pt.Select();
                                            _wordApp.Selection.TypeText(ch.ToString());
                                            currentOffset++;
                                        }
                                    }
                                    catch { break; }
                                }
                                break;
                        }
                    }

                    return true;
                }
                finally
                {
                    // 恢复原始修订设置
                    try
                    {
                        _wordApp.ActiveDocument.TrackRevisions = originalTrackChanges;
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                Infrastructure.Logger.Error("ApplyWithTrackChanges 失败", ex);
                return false;
            }
        }

        /// <summary>
        /// 直接替换（不开启修订）
        /// </summary>
        private bool ApplyDirectReplace(string polishedText)
        {
            try
            {
                _wordApp.Selection.Range.Text = polishedText;
                return true;
            }
            catch (Exception ex)
            {
                Infrastructure.Logger.Error("ApplyDirectReplace 失败", ex);
                return false;
            }
        }

        /// <summary>
        /// 将选中文本替换为润色结果（通用入口）
        /// </summary>
        public bool ApplyPolishResult(string original, string polished, bool useTrackChanges)
        {
            // 如果选中内容为空，先尝试获取选中文本
            if (string.IsNullOrEmpty(original))
            {
                original = WordHelper.GetSelectedText(_wordApp);
            }

            if (string.IsNullOrEmpty(original))
            {
                System.Windows.Forms.MessageBox.Show(
                    "请先在 Word 中选中要润色的论文文本。",
                    "提示",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Information);
                return false;
            }

            // 检查文档保护
            if (useTrackChanges && WordHelper.IsDocumentProtected(_wordApp))
            {
                System.Windows.Forms.MessageBox.Show(
                    "文档已启用限制编辑，无法使用修订模式。\n请先解除保护（审阅 → 限制编辑 → 停止保护）。",
                    "文档受保护",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Warning);
                return false;
            }

            return ApplyChanges(original, polished, useTrackChanges);
        }
    }
}
