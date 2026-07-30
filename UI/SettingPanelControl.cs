using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using AIPolishCOMAddin.Infrastructure;

namespace AIPolishCOMAddin.UI
{
    /// <summary>
    /// 模型设置面板 — API 配置、预设、测试连通性
    /// </summary>
    public partial class SettingPanelControl : UserControl
    {
        // 设置变更事件（通知主面板刷新）
        public event Action<SettingsModel> SettingsSaved;
        public event Action SettingsReset;

        private SettingsModel _settings;

        public SettingPanelControl()
        {
            InitializeComponent();
            SetupUI();
        }

        private void SetupUI()
        {
            // 温度滑块的标签绑定
            UpdateTemperatureLabel();

            // 提示文字
            txtApiKey.PasswordChar = '●';
        }

        private void SettingPanelControl_Load(object sender, EventArgs e)
        {
            // 加载时从注册表读取设置
            LoadFromRegistry();
        }

        /// <summary>
        /// 从注册表加载设置并填充 UI
        /// </summary>
        public void LoadFromRegistry()
        {
            _settings = RegistrySettings.LoadAll();
            ApplySettingsToUI();
        }

        /// <summary>
        /// 将设置模型应用到 UI 控件
        /// </summary>
        private void ApplySettingsToUI()
        {
            if (_settings == null) return;

            txtApiBaseUrl.Text = _settings.ApiBaseUrl ?? "";
            txtApiKey.Text = _settings.ApiKey ?? "";
            txtModel.Text = _settings.Model ?? "";
            trackTemperature.Value = (int)(_settings.Temperature * 100);
            nudMaxTokens.Value = Math.Max(128, Math.Min(32768, _settings.MaxTokens));
            nudTimeout.Value = Math.Max(10, Math.Min(300, _settings.TimeoutSeconds));
            nudRetry.Value = Math.Max(0, Math.Min(5, _settings.RetryCount));
            txtCustomTerms.Text = _settings.CustomTerms ?? "";

            UpdateTemperatureLabel();
        }

        /// <summary>
        /// 从 UI 控件读取设置到模型
        /// </summary>
        private SettingsModel ReadFromUI()
        {
            return new SettingsModel
            {
                ApiBaseUrl = txtApiBaseUrl.Text.Trim(),
                ApiKey = txtApiKey.Text.Trim(),
                Model = txtModel.Text.Trim(),
                Temperature = trackTemperature.Value / 100.0,
                MaxTokens = (int)nudMaxTokens.Value,
                TimeoutSeconds = (int)nudTimeout.Value,
                RetryCount = (int)nudRetry.Value,
                CustomTerms = txtCustomTerms.Text.Trim(),
                // 选项同步到本地设置（实际选项在 MainPanel）
                EnableTermProtect = true,
                EnableTrackChanges = true,
                EnableSentenceMode = false
            };
        }

        /// <summary>
        /// 更新温度显示标签
        /// </summary>
        private void UpdateTemperatureLabel()
        {
            double value = trackTemperature.Value / 100.0;
            lblTempValue.Text = value.ToString("F2");
        }

        /// <summary>
        /// 温度滑块滚动
        /// </summary>
        private void trackTemperature_Scroll(object sender, EventArgs e)
        {
            UpdateTemperatureLabel();
        }

        /// <summary>
        /// 保存设置到注册表
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // 基础校验
                if (string.IsNullOrWhiteSpace(txtApiBaseUrl.Text))
                {
                    MessageBox.Show("API Base URL 不能为空。",
                        "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtApiKey.Text))
                {
                    var result = MessageBox.Show("API Key 为空，确定保存吗？（润色时将无法调用 API）",
                        "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result != DialogResult.Yes) return;
                }

                _settings = ReadFromUI();
                RegistrySettings.SaveAll(_settings);

                SetStatus("✅ 设置已保存", Color.DarkGreen);
                Logger.Info("设置已保存到注册表");

                // 触发事件通知主面板
                SettingsSaved?.Invoke(_settings);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存失败: {ex.Message}",
                    "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatus($"❌ 保存失败", Color.Red);
            }
        }

        /// <summary>
        /// 测试 API 连通性
        /// </summary>
        private async void btnTest_Click(object sender, EventArgs e)
        {
            try
            {
                btnTest.Enabled = false;
                btnTest.Text = "测试中...";
                SetStatus("⏳ 正在测试连接...", Color.Gray);

                var settings = ReadFromUI();

                if (string.IsNullOrWhiteSpace(settings.ApiBaseUrl) ||
                    string.IsNullOrWhiteSpace(settings.ApiKey))
                {
                    MessageBox.Show("请先填写 API Base URL 和 API Key。",
                        "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (var client = new LLMClient(settings))
                {
                    var response = await Task.Run(() => client.TestConnectionAsync());

                    if (response.IsSuccess)
                    {
                        MessageBox.Show(
                            $"✅ 连接成功！\n\nAPI 配置正确，可以正常调用。",
                            "测试结果", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        SetStatus("✅ 连接成功", Color.DarkGreen);
                    }
                    else
                    {
                        // 分类显示友好错误
                        string errorTitle = response.ErrorType switch
                        {
                            "auth_error" => "认证失败",
                            "timeout" => "连接超时",
                            "network_error" => "网络错误",
                            "rate_limit" => "请求限流",
                            "not_found" => "地址错误",
                            _ => "请求失败"
                        };

                        MessageBox.Show(
                            $"❌ {errorTitle}\n\n{response.ErrorMessage}\n\n" +
                            $"状态码: {response.StatusCode}",
                            "测试失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        SetStatus($"❌ {errorTitle}", Color.Red);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"测试异常: {ex.Message}",
                    "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetStatus("❌ 测试异常", Color.Red);
                Logger.Error("btnTest_Click 异常", ex);
            }
            finally
            {
                btnTest.Enabled = true;
                btnTest.Text = "🔌 测试连通性";
            }
        }

        /// <summary>
        /// 重置全部设置
        /// </summary>
        private void btnReset_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "确定要重置全部设置吗？\n\n" +
                "这将清除所有已保存的 API 配置，恢复出厂默认值。",
                "确认重置",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes) return;

            RegistrySettings.ResetAll();
            _settings = new SettingsModel();
            ApplySettingsToUI();

            SetStatus("🔄 已重置为默认设置", Color.Gray);
            Logger.Info("设置已重置");

            SettingsReset?.Invoke();
        }

        /// <summary>
        /// 预设按钮点击
        /// </summary>
        private void btnPreset_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            string presetName = btn.Text;

            switch (presetName)
            {
                case "DeepSeek":
                    txtApiBaseUrl.Text = "https://api.deepseek.com/v1";
                    txtModel.Text = "deepseek-chat";
                    trackTemperature.Value = 10;
                    break;
                case "GPT-4o":
                    txtApiBaseUrl.Text = "https://api.openai.com/v1";
                    txtModel.Text = "gpt-4o-mini";
                    trackTemperature.Value = 20;
                    break;
                case "Kimi":
                    txtApiBaseUrl.Text = "https://api.moonshot.cn/v1";
                    txtModel.Text = "moonshot-v1-8k";
                    trackTemperature.Value = 15;
                    break;
                case "GLM-4":
                    txtApiBaseUrl.Text = "https://open.bigmodel.cn/api/paas/v4";
                    txtModel.Text = "glm-4-flash";
                    trackTemperature.Value = 10;
                    break;
                case "通义千问":
                    txtApiBaseUrl.Text = "https://dashscope.aliyuncs.com/compatible-mode/v1";
                    txtModel.Text = "qwen-turbo";
                    trackTemperature.Value = 10;
                    break;
            }

            UpdateTemperatureLabel();
            SetStatus($"已加载 {presetName} 预设", Color.DodgerBlue);
        }

        /// <summary>
        /// 温度预设按钮点击
        /// </summary>
        private void btnTempPreset_Click(object sender, EventArgs e)
        {
            var btn = sender as Button;
            if (btn == null) return;

            string text = btn.Text;

            if (text.Contains("0.05"))
            {
                trackTemperature.Value = 5;
                SetStatus("已设为严谨校对模式 (0.05)", Color.Gray);
            }
            else if (text.Contains("0.20"))
            {
                trackTemperature.Value = 20;
                SetStatus("已设为平衡润色模式 (0.20)", Color.Gray);
            }
            else if (text.Contains("0.60"))
            {
                trackTemperature.Value = 60;
                SetStatus("已设为自由改写模式 (0.60)", Color.Gray);
            }

            UpdateTemperatureLabel();
        }

        /// <summary>
        /// 设置状态文本
        /// </summary>
        private void SetStatus(string text, Color color)
        {
            lblStatus.Text = text;
            lblStatus.ForeColor = color;
        }

        /// <summary>
        /// 外部调用：显示提示（首次启动引导）
        /// </summary>
        public void ShowFirstTimePrompt()
        {
            SetStatus("⚠️ 请先配置 API 参数", Color.OrangeRed);
        }
    }
}
