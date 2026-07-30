using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AIPolishCOMAddin.Engine;
using AIPolishCOMAddin.Infrastructure;
using AIPolishCOMAddin.Models;
using AIPolishCOMAddin.Utils;
using Microsoft.Office.Interop.Word;

namespace AIPolishCOMAddin.UI
{
    /// <summary>
    /// 润色工作台面板 — CustomTaskPane 主界面
    /// </summary>
    public partial class MainPanelControl : UserControl
    {
        private Microsoft.Office.Interop.Word.Application _wordApp;
        private PolishEngine _engine;
        private PolishResult _currentResult;
        private CancellationTokenSource _cts;
        private bool _isRunning;

        // 设置引用（由外部注入）
        private SettingsModel _settings;

        public MainPanelControl()
        {
            InitializeComponent();
            SetupUI();
        }

        /// <summary>
        /// 注入 Word Application 引用
        /// </summary>
        public void SetWordApplication(Microsoft.Office.Interop.Word.Application wordApp)
        {
            _wordApp = wordApp;
        }

        /// <summary>
        /// 加载设置
        /// </summary>
        public void LoadSettings(SettingsModel settings)
        {
            _settings = settings;
            ApplySettingsToUI();
        }

        /// <summary>
        /// 刷新设置（从注册表重新读取）
        /// </summary>
        public void ReloadSettings()
        {
            _settings = RegistrySettings.LoadAll();
            ApplySettingsToUI();
        }

        private void SetupUI()
        {
            // 润色模式下拉
            cmbPolishMode.Items.Clear();
            cmbPolishMode.Items.AddRange(PolishModeHelper.GetAllModeDescriptions());
            cmbPolishMode.SelectedIndex = 0;

            // 章节类型下拉
            cmbSectionType.Items.Clear();
            cmbSectionType.Items.Add("通用");
            cmbSectionType.Items.Add("Abstract 摘要");
            cmbSectionType.Items.Add("Introduction 引言");
            cmbSectionType.Items.Add("Related Work 相关工作");
            cmbSectionType.Items.Add("Method 方法");
            cmbSectionType.Items.Add("Experiment 实验");
            cmbSectionType.Items.Add("Discussion 讨论");
            cmbSectionType.Items.Add("Conclusion 结论");
            cmbSectionType.SelectedIndex = 0;

            // 日志字体
            txtLog.Font = new Font("Consolas", 9F);
            txtOriginal.Font = new Font("Microsoft YaHei UI", 9F);
            txtPolished.Font = new Font("Microsoft YaHei UI", 9F);
        }

        private void ApplySettingsToUI()
        {
            if (_settings == null) return;

            chkSentenceMode.Checked = _settings.EnableSentenceMode;
            chkTermProtect.Checked = _settings.EnableTermProtect;
            chkTrackChanges.Checked = _settings.EnableTrackChanges;
        }

        private void MainPanelControl_Load(object sender, EventArgs e)
        {
            // 加载时不做特殊处理，等待外部注入
        }

        /// <summary>
        /// 获取选中文本
        /// </summary>
        private void btnGetSelection_Click(object sender, EventArgs e)
        {
            try
            {
                if (!WordHelper.HasSelection(_wordApp))
                {
                    MessageBox.Show("请先在 Word 文档中选中要润色的论文文本。",
                        "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string selectedText = WordHelper.GetSelectedText(_wordApp);
                txtOriginal.Text = selectedText;
                txtPolished.Clear();
                _currentResult = null;
                UpdateButtonsAfterResult(false);

                int tokens = WordHelper.EstimateTokens(selectedText);
                AddLog($"📄 已获取选中文本 ({selectedText.Length} 字符, 约 {tokens} token)");

                if (tokens > 4000)
                {
                    AddLog("⚠️ 文本较长，建议拆分为段落分别润色。");
                }
            }
            catch (Exception ex)
            {
                AddLog($"❌ 获取失败: {ex.Message}");
                Logger.Error("btnGetSelection_Click 异常", ex);
            }
        }

        /// <summary>
        /// 执行润色
        /// </summary>
        private async void btnPolish_Click(object sender, EventArgs e)
        {
            if (_isRunning) return;

            // 检查 API 配置
            if (_settings == null || string.IsNullOrWhiteSpace(_settings.ApiBaseUrl))
            {
                MessageBox.Show("请先在「模型设置」Tab 中配置 API 参数。\n\n" +
                    "需要填写：API Base URL 和 API Key",
                    "API 未配置", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 检查选中文本
            string text = txtOriginal.Text;
            if (string.IsNullOrWhiteSpace(text))
            {
                MessageBox.Show("请先点击「获取选中」或直接在原文框中粘贴要润色的文本。",
                    "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 逐句模式警告
            if (chkSentenceMode.Checked)
            {
                int estTokens = WordHelper.EstimateTokens(text);
                int estSentences = Math.Max(1, text.Length / 80);
                if (estSentences > 10)
                {
                    var result = MessageBox.Show(
                        $"逐句润色将把文本分为约 {estSentences} 句分别调用 API。\n" +
                        $"这将消耗更多 token（约 {estTokens * estSentences / 2:N0} tokens）。\n\n" +
                        "继续吗？",
                        "逐句润色提示",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result != DialogResult.Yes) return;
                }
            }

            // 启动润色
            _isRunning = true;
            SetRunningState(true);

            try
            {
                _cts = new CancellationTokenSource();

                // 创建引擎
                _engine = new PolishEngine(_settings);

                // 绑定进度回调
                _engine.OnSentenceProgress = (current, total) =>
                {
                    if (!IsDisposed && !Disposing)
                    {
                        this.Invoke((Action)(() =>
                        {
                            if (total > 0)
                            {
                                progressBar.Value = Math.Min(100, current * 100 / total);
                            }
                            lblStatus.Text = $"正在处理第 {current}/{total} 句...";
                        }));
                    }
                };

                _engine.OnStatusUpdate = (status) =>
                {
                    if (!IsDisposed && !Disposing)
                    {
                        this.Invoke((Action)(() =>
                        {
                            lblStatus.Text = status;
                        }));
                    }
                };

                _engine.OnLogMessage = (msg) =>
                {
                    if (!IsDisposed && !Disposing)
                    {
                        this.Invoke((Action)(() => AddLog(msg)));
                    }
                };

                // 确定模式和章节
                var mode = PolishModeHelper.GetModeByIndex(cmbPolishMode.SelectedIndex);
                var section = (SectionType)cmbSectionType.SelectedIndex;

                // 执行润色
                _currentResult = await Task.Run(() =>
                    _engine.PolishAsync(text, mode, section, chkSentenceMode.Checked, _cts.Token));

                // 显示结果
                if (_currentResult.IsSuccess && !string.IsNullOrEmpty(_currentResult.PolishedText))
                {
                    txtPolished.Text = _currentResult.PolishedText;
                    txtPolished.ForeColor = Color.DarkGreen;
                    UpdateButtonsAfterResult(true);
                    AddLog($"✅ 润色完成！耗时 {_currentResult.ElapsedMilliseconds / 1000.0:F1}s");
                    AddLog($"  预估输入: {_currentResult.EstimatedInputTokens} token | 输出: {_currentResult.EstimatedOutputTokens} token");
                }
                else
                {
                    UpdateButtonsAfterResult(false);
                    AddLog($"❌ {_currentResult.ErrorMessage}");
                }
            }
            catch (Exception ex)
            {
                AddLog($"❌ 润色异常: {ex.Message}");
                Logger.Error("btnPolish_Click 异常", ex);
            }
            finally
            {
                _isRunning = false;
                SetRunningState(false);
            }
        }

        /// <summary>
        /// 停止润色
        /// </summary>
        private void btnStop_Click(object sender, EventArgs e)
        {
            if (_cts != null && !_cts.IsCancellationRequested)
            {
                _cts.Cancel();
                AddLog("⛔ 正在停止...");
            }
        }

        /// <summary>
        /// 预览差异
        /// </summary>
        private void btnPreviewDiff_Click(object sender, EventArgs e)
        {
            if (_currentResult == null || string.IsNullOrEmpty(_currentResult.PolishedText))
                return;

            string diffPreview = InMemoryDiff.GetDiffPreview(
                _currentResult.OriginalText,
                _currentResult.PolishedText);

            txtPolished.Text = diffPreview;
            txtPolished.ForeColor = Color.Black;
        }

        /// <summary>
        /// 应用到文档
        /// </summary>
        private void btnApply_Click(object sender, EventArgs e)
        {
            if (_currentResult == null || string.IsNullOrEmpty(_currentResult.PolishedText))
            {
                MessageBox.Show("请先执行润色，再应用到文档。",
                    "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;

                var injector = new TrackChangesInjector(_wordApp);
                bool success = injector.ApplyPolishResult(
                    _currentResult.OriginalText,
                    _currentResult.PolishedText,
                    chkTrackChanges.Checked);

                if (success)
                {
                    AddLog($"📝 已应用到文档" +
                        (chkTrackChanges.Checked ? "（修订模式）" : "（直接替换）"));
                    btnUndo.Enabled = true;
                }
                else
                {
                    AddLog("⚠️ 应用到文档失败，请检查选区和文档状态。");
                }
            }
            catch (Exception ex)
            {
                AddLog($"❌ 应用失败: {ex.Message}");
                Logger.Error("btnApply_Click 异常", ex);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        /// <summary>
        /// 复制结果到剪贴板
        /// </summary>
        private void btnCopyResult_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPolished.Text))
            {
                try
                {
                    Clipboard.SetText(txtPolished.Text);
                    AddLog("📋 已复制到剪贴板");
                }
                catch (Exception ex)
                {
                    AddLog($"❌ 复制失败: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// 撤销
        /// </summary>
        private void btnUndo_Click(object sender, EventArgs e)
        {
            WordHelper.Undo(_wordApp);
            AddLog("↩ 已执行撤销");
        }

        /// <summary>
        /// 清空日志
        /// </summary>
        private void btnClearLog_Click(object sender, EventArgs e)
        {
            txtLog.Clear();
        }

        /// <summary>
        /// 设置运行/空闲状态
        /// </summary>
        private void SetRunningState(bool running)
        {
            btnPolish.Enabled = !running;
            btnGetSelection.Enabled = !running;
            cmbPolishMode.Enabled = !running;
            cmbSectionType.Enabled = !running;
            chkSentenceMode.Enabled = !running;
            chkTermProtect.Enabled = !running;
            chkTrackChanges.Enabled = !running;

            btnStop.Visible = running;

            if (running)
            {
                progressBar.Visible = true;
                progressBar.Style = ProgressBarStyle.Marquee;
                lblStatus.Text = "正在调用大模型...";
            }
            else
            {
                progressBar.Style = ProgressBarStyle.Continuous;
                progressBar.Value = 0;
                progressBar.Visible = false;
            }
        }

        /// <summary>
        /// 根据结果更新按钮状态
        /// </summary>
        private void UpdateButtonsAfterResult(bool hasResult)
        {
            btnPreviewDiff.Enabled = hasResult;
            btnApply.Enabled = hasResult;
            btnCopyResult.Enabled = hasResult;
        }

        /// <summary>
        /// 添加日志
        /// </summary>
        public void AddLog(string message)
        {
            if (IsDisposed || Disposing) return;

            try
            {
                if (txtLog.InvokeRequired)
                {
                    txtLog.Invoke((Action)(() => AppendLog(message)));
                }
                else
                {
                    AppendLog(message);
                }
            }
            catch
            {
                // 忽略跨线程访问异常
            }

            // 同时也写入本地日志文件
            if (message.StartsWith("❌") || message.StartsWith("⚠️") ||
                message.StartsWith("✅") || message.StartsWith("📄"))
            {
                Logger.Info(message);
            }
        }

        private void AppendLog(string message)
        {
            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            txtLog.AppendText($"[{timestamp}] {message}\r\n");
            // 自动滚动到底部
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
        }
    }
}
