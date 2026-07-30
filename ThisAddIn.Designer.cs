namespace AIPolishCOMAddin
{
    partial class ThisAddIn
    {
        private System.ComponentModel.IContainer components = null;
        // Use dynamic to avoid compile-time dependency on VSTO CustomTaskPane type.
        // The actual VSTO type is resolved at runtime when loaded in Word.
        private dynamic _mainTaskPane;
        private UI.MainPanelControl _mainPanel;
        private UI.SettingPanelControl _settingPanel;

        // Dispose is NOT an override. Instead, we hook the VSTO Shutdown event
        // in ThisAddIn.cs and call Cleanup() from there, avoiding the need for
        // the VSTO-generated base class at compile time.
        private void Cleanup(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
        }

        #region VSTO Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
        }

        #endregion

        /// <summary>
        /// 创建右侧 CustomTaskPane
        /// </summary>
        private void CreateTaskPane()
        {
            try
            {
                _mainPanel = new UI.MainPanelControl();
                _settingPanel = new UI.SettingPanelControl();

                _mainPanel.SetWordApplication(((dynamic)this).Application);

                // 创建 TabControl 作为两个面板的容器
                var tabControl = new System.Windows.Forms.TabControl();
                tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
                tabControl.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);

                // Tab1: 润色工作台
                var tabMain = new System.Windows.Forms.TabPage("润色工作台");
                _mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
                tabMain.Controls.Add(_mainPanel);

                // Tab2: 模型设置
                var tabSetting = new System.Windows.Forms.TabPage("模型设置");
                _settingPanel.Dock = System.Windows.Forms.DockStyle.Fill;
                tabSetting.Controls.Add(_settingPanel);

                tabControl.TabPages.Add(tabMain);
                tabControl.TabPages.Add(tabSetting);

                // 通过 VSTO CustomTaskPanes 集合添加面板 (dynamic to avoid compile-time dep)
                _mainTaskPane = ((dynamic)this).CustomTaskPanes.Add(tabControl, "AI论文润色助手");
                _mainTaskPane.Visible = true;

                // 绑定事件
                _settingPanel.SettingsSaved += OnSettingsSaved;
                _settingPanel.SettingsReset += OnSettingsReset;

                LoadSettingsAndCheckFirstRun();
            }
            catch (System.Exception ex)
            {
                Infrastructure.Logger.Error("CreateTaskPane 失败", ex);
                System.Windows.Forms.MessageBox.Show(
                    $"插件初始化失败: {ex.Message}",
                    "错误",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void LoadSettingsAndCheckFirstRun()
        {
            var settings = Infrastructure.RegistrySettings.LoadAll();

            if (Infrastructure.RegistrySettings.HasSettings())
            {
                _mainPanel.LoadSettings(settings);
            }
            else
            {
                _settingPanel.ShowFirstTimePrompt();
                try
                {
                    if (_mainTaskPane.Control is System.Windows.Forms.TabControl tc && tc.TabPages.Count > 1)
                        tc.SelectedIndex = 1;
                }
                catch { }

                System.Windows.Forms.MessageBox.Show(
                    "欢迎使用 AI论文润色助手！🎉\n\n" +
                    "首次使用请先配置大模型 API 参数：\n" +
                    "1. 在「模型设置」Tab 中填入 API Base URL 和 API Key\n" +
                    "2. 点击「测试连通性」确认可用\n" +
                    "3. 点击「保存设置」\n\n" +
                    "之后切回「润色工作台」即可开始润色。",
                    "首次使用引导",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Information);
            }
        }

        private void OnSettingsSaved(Infrastructure.SettingsModel settings)
        {
            _mainPanel.LoadSettings(settings);
        }

        private void OnSettingsReset()
        {
            _mainPanel.ReloadSettings();
        }
    }
}
