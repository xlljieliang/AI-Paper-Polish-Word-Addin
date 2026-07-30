using System;
using System.Windows.Forms;
using AIPolishCOMAddin.Infrastructure;
using AIPolishCOMAddin.UI;
using AIPolishCOMAddin.Utils;
using Office = Microsoft.Office.Core;

namespace AIPolishCOMAddin
{
    /// <summary>
    /// 插件主入口 — ThisAddIn
    /// 负责：启动/关闭、TaskPane 创建、右键菜单注册
    /// </summary>
    public partial class ThisAddIn
    {
        private Office.CommandBarButton _contextMenuItem;

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            try
            {
                Logger.Initialize();
                Logger.Info("====== AI论文润色助手 启动 ======");

                CreateTaskPane();
                RegisterContextMenu();

                Logger.Info("插件启动完成");
            }
            catch (Exception ex)
            {
                Logger.Error("ThisAddIn_Startup 异常", ex);
                MessageBox.Show(
                    $"插件启动失败: {ex.Message}\n\n请尝试重新安装或联系开发者。",
                    "启动错误",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            try
            {
                UnregisterContextMenu();
                Logger.Info("====== AI论文润色助手 关闭 ======");
            }
            catch (Exception ex)
            {
                Logger.Error("ThisAddIn_Shutdown 异常", ex);
            }
        }

        #region 右键菜单

        /// <summary>
        /// 注册 Word 右键菜单 — "AI润色选中段落"
        /// </summary>
        private void RegisterContextMenu()
        {
            try
            {
                var popupMenu = this.Application.CommandBars["Text"];
                if (popupMenu == null) return;

                // 防止重复注册
                foreach (Office.CommandBarControl ctrl in popupMenu.Controls)
                {
                    if (ctrl.Tag == "AIPolishAddin")
                    {
                        _contextMenuItem = ctrl as Office.CommandBarButton;
                        return;
                    }
                }

                _contextMenuItem = (Office.CommandBarButton)popupMenu.Controls.Add(
                    Type: Office.MsoControlType.msoControlButton,
                    Before: popupMenu.Controls.Count + 1,
                    Temporary: true);

                if (_contextMenuItem != null)
                {
                    _contextMenuItem.Caption = "AI润色选中段落";
                    _contextMenuItem.Tag = "AIPolishAddin";
                    _contextMenuItem.FaceId = 378;
                    _contextMenuItem.Style = Office.MsoButtonStyle.msoButtonIconAndCaption;
                    _contextMenuItem.Click += ContextMenuClick;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("RegisterContextMenu 失败", ex);
            }
        }

        private void UnregisterContextMenu()
        {
            try
            {
                if (_contextMenuItem != null)
                {
                    _contextMenuItem.Delete();
                    _contextMenuItem = null;
                }
            }
            catch (Exception ex)
            {
                Logger.Error("UnregisterContextMenu 失败", ex);
            }
        }

        /// <summary>
        /// 右键菜单点击：获取选中文本来填充主面板
        /// </summary>
        private void ContextMenuClick(Office.CommandBarButton cmdBarbutton, ref bool cancelDefault)
        {
            try
            {
                Logger.Info("右键菜单触发: AI润色选中段落");

                if (!WordHelper.HasSelection(this.Application))
                {
                    MessageBox.Show("请先在文档中选中要润色的文本段落。",
                        "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (!RegistrySettings.HasSettings())
                {
                    var result = MessageBox.Show(
                        "请先在插件设置中配置 API 参数。\n\n是否跳转到设置面板？",
                        "API 未配置", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (result == DialogResult.Yes)
                    {
                        if (_mainTaskPane?.Control is System.Windows.Forms.TabControl tc
                            && tc.TabPages.Count > 1)
                            tc.SelectedIndex = 1;
                    }
                    return;
                }

                // 触发主面板的「获取选中」逻辑
                _mainPanel?.GetType()
                    .GetMethod("btnGetSelection_Click",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.Invoke(_mainPanel, new object[] { null, EventArgs.Empty });
            }
            catch (Exception ex)
            {
                Logger.Error("ContextMenuClick 异常", ex);
            }
        }

        #endregion

        /// <summary>
        /// 获取主面板引用
        /// </summary>
        public MainPanelControl MainPanel => _mainPanel;

        /// <summary>
        /// 获取设置面板引用
        /// </summary>
        public SettingPanelControl SettingPanel => _settingPanel;

        #region VSTO generated code

        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion
    }
}
