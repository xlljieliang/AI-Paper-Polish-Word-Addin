namespace AIPolishCOMAddin.UI
{
    partial class MainPanelControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.lblMode = new System.Windows.Forms.Label();
            this.cmbPolishMode = new System.Windows.Forms.ComboBox();
            this.lblSection = new System.Windows.Forms.Label();
            this.cmbSectionType = new System.Windows.Forms.ComboBox();
            this.grpOptions = new System.Windows.Forms.GroupBox();
            this.chkSentenceMode = new System.Windows.Forms.CheckBox();
            this.chkTermProtect = new System.Windows.Forms.CheckBox();
            this.chkTrackChanges = new System.Windows.Forms.CheckBox();
            this.grpActions = new System.Windows.Forms.GroupBox();
            this.btnStop = new System.Windows.Forms.Button();
            this.btnGetSelection = new System.Windows.Forms.Button();
            this.btnPolish = new System.Windows.Forms.Button();
            this.btnPreviewDiff = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCopyResult = new System.Windows.Forms.Button();
            this.btnUndo = new System.Windows.Forms.Button();
            this.btnExportLog = new System.Windows.Forms.Button();
            this.btnClearLog = new System.Windows.Forms.Button();
            this.grpPreview = new System.Windows.Forms.GroupBox();
            this.txtOriginal = new System.Windows.Forms.TextBox();
            this.txtPolished = new System.Windows.Forms.TextBox();
            this.lblOriginal = new System.Windows.Forms.Label();
            this.lblPolished = new System.Windows.Forms.Label();
            this.grpLog = new System.Windows.Forms.GroupBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblStatus = new System.Windows.Forms.Label();
            this.grpOptions.SuspendLayout();
            this.grpActions.SuspendLayout();
            this.grpPreview.SuspendLayout();
            this.grpLog.SuspendLayout();
            this.SuspendLayout();
            //
            // lblMode
            //
            this.lblMode.AutoSize = true;
            this.lblMode.Location = new System.Drawing.Point(3, 6);
            this.lblMode.Name = "lblMode";
            this.lblMode.Size = new System.Drawing.Size(56, 17);
            this.lblMode.TabIndex = 0;
            this.lblMode.Text = "润色模式";
            //
            // cmbPolishMode
            //
            this.cmbPolishMode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbPolishMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPolishMode.FormattingEnabled = true;
            this.cmbPolishMode.Location = new System.Drawing.Point(3, 26);
            this.cmbPolishMode.Name = "cmbPolishMode";
            this.cmbPolishMode.Size = new System.Drawing.Size(284, 24);
            this.cmbPolishMode.TabIndex = 1;
            //
            // lblSection
            //
            this.lblSection.AutoSize = true;
            this.lblSection.Location = new System.Drawing.Point(3, 56);
            this.lblSection.Name = "lblSection";
            this.lblSection.Size = new System.Drawing.Size(56, 17);
            this.lblSection.TabIndex = 2;
            this.lblSection.Text = "章节类型";
            //
            // cmbSectionType
            //
            this.cmbSectionType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbSectionType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSectionType.FormattingEnabled = true;
            this.cmbSectionType.Location = new System.Drawing.Point(3, 76);
            this.cmbSectionType.Name = "cmbSectionType";
            this.cmbSectionType.Size = new System.Drawing.Size(284, 24);
            this.cmbSectionType.TabIndex = 3;
            //
            // grpOptions
            //
            this.grpOptions.Controls.Add(this.chkSentenceMode);
            this.grpOptions.Controls.Add(this.chkTermProtect);
            this.grpOptions.Controls.Add(this.chkTrackChanges);
            this.grpOptions.Location = new System.Drawing.Point(3, 106);
            this.grpOptions.Name = "grpOptions";
            this.grpOptions.Size = new System.Drawing.Size(284, 82);
            this.grpOptions.TabIndex = 4;
            this.grpOptions.TabStop = false;
            this.grpOptions.Text = "选项";
            //
            // chkSentenceMode
            //
            this.chkSentenceMode.AutoSize = true;
            this.chkSentenceMode.Location = new System.Drawing.Point(7, 22);
            this.chkSentenceMode.Name = "chkSentenceMode";
            this.chkSentenceMode.Size = new System.Drawing.Size(111, 21);
            this.chkSentenceMode.TabIndex = 0;
            this.chkSentenceMode.Text = "逐句润色（精细）";
            this.chkSentenceMode.UseVisualStyleBackColor = true;
            //
            // chkTermProtect
            //
            this.chkTermProtect.AutoSize = true;
            this.chkTermProtect.Checked = true;
            this.chkTermProtect.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTermProtect.Location = new System.Drawing.Point(7, 46);
            this.chkTermProtect.Name = "chkTermProtect";
            this.chkTermProtect.Size = new System.Drawing.Size(111, 21);
            this.chkTermProtect.TabIndex = 1;
            this.chkTermProtect.Text = "保护专业术语";
            this.chkTermProtect.UseVisualStyleBackColor = true;
            //
            // chkTrackChanges
            //
            this.chkTrackChanges.AutoSize = true;
            this.chkTrackChanges.Checked = true;
            this.chkTrackChanges.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTrackChanges.Location = new System.Drawing.Point(128, 22);
            this.chkTrackChanges.Name = "chkTrackChanges";
            this.chkTrackChanges.Size = new System.Drawing.Size(135, 21);
            this.chkTrackChanges.TabIndex = 2;
            this.chkTrackChanges.Text = "Word修订模式 ✓";
            this.chkTrackChanges.UseVisualStyleBackColor = true;
            //
            // grpActions
            //
            this.grpActions.Controls.Add(this.btnStop);
            this.grpActions.Controls.Add(this.btnGetSelection);
            this.grpActions.Controls.Add(this.btnPolish);
            this.grpActions.Controls.Add(this.btnPreviewDiff);
            this.grpActions.Controls.Add(this.btnApply);
            this.grpActions.Controls.Add(this.btnCopyResult);
            this.grpActions.Controls.Add(this.btnUndo);
            this.grpActions.Controls.Add(this.btnExportLog);
            this.grpActions.Controls.Add(this.btnClearLog);
            this.grpActions.Location = new System.Drawing.Point(3, 194);
            this.grpActions.Name = "grpActions";
            this.grpActions.Size = new System.Drawing.Size(284, 94);
            this.grpActions.TabIndex = 5;
            this.grpActions.TabStop = false;
            this.grpActions.Text = "操作";
            //
            // btnStop
            //
            this.btnStop.BackColor = System.Drawing.Color.IndianRed;
            this.btnStop.Enabled = false;
            this.btnStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStop.ForeColor = System.Drawing.Color.White;
            this.btnStop.Location = new System.Drawing.Point(186, 19);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(92, 30);
            this.btnStop.TabIndex = 8;
            this.btnStop.Text = "⏹ 停止";
            this.btnStop.UseVisualStyleBackColor = false;
            this.btnStop.Visible = false;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            //
            // btnGetSelection
            //
            this.btnGetSelection.Location = new System.Drawing.Point(7, 19);
            this.btnGetSelection.Name = "btnGetSelection";
            this.btnGetSelection.Size = new System.Drawing.Size(85, 30);
            this.btnGetSelection.TabIndex = 0;
            this.btnGetSelection.Text = "获取选中";
            this.btnGetSelection.UseVisualStyleBackColor = true;
            this.btnGetSelection.Click += new System.EventHandler(this.btnGetSelection_Click);
            //
            // btnPolish
            //
            this.btnPolish.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnPolish.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPolish.ForeColor = System.Drawing.Color.White;
            this.btnPolish.Location = new System.Drawing.Point(96, 19);
            this.btnPolish.Name = "btnPolish";
            this.btnPolish.Size = new System.Drawing.Size(85, 30);
            this.btnPolish.TabIndex = 1;
            this.btnPolish.Text = "🚀 执行润色";
            this.btnPolish.UseVisualStyleBackColor = false;
            this.btnPolish.Click += new System.EventHandler(this.btnPolish_Click);
            //
            // btnPreviewDiff
            //
            this.btnPreviewDiff.Enabled = false;
            this.btnPreviewDiff.Location = new System.Drawing.Point(7, 55);
            this.btnPreviewDiff.Name = "btnPreviewDiff";
            this.btnPreviewDiff.Size = new System.Drawing.Size(68, 30);
            this.btnPreviewDiff.TabIndex = 2;
            this.btnPreviewDiff.Text = "预览差异";
            this.btnPreviewDiff.UseVisualStyleBackColor = true;
            this.btnPreviewDiff.Click += new System.EventHandler(this.btnPreviewDiff_Click);
            //
            // btnApply
            //
            this.btnApply.BackColor = System.Drawing.Color.MediumSeaGreen;
            this.btnApply.Enabled = false;
            this.btnApply.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApply.ForeColor = System.Drawing.Color.White;
            this.btnApply.Location = new System.Drawing.Point(79, 55);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(68, 30);
            this.btnApply.TabIndex = 3;
            this.btnApply.Text = "应用到文档";
            this.btnApply.UseVisualStyleBackColor = false;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            //
            // btnCopyResult
            //
            this.btnCopyResult.Enabled = false;
            this.btnCopyResult.Location = new System.Drawing.Point(151, 55);
            this.btnCopyResult.Name = "btnCopyResult";
            this.btnCopyResult.Size = new System.Drawing.Size(62, 30);
            this.btnCopyResult.TabIndex = 4;
            this.btnCopyResult.Text = "复制结果";
            this.btnCopyResult.UseVisualStyleBackColor = true;
            this.btnCopyResult.Click += new System.EventHandler(this.btnCopyResult_Click);
            //
            // btnUndo
            //
            this.btnUndo.Enabled = false;
            this.btnUndo.Location = new System.Drawing.Point(217, 55);
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(61, 30);
            this.btnUndo.TabIndex = 5;
            this.btnUndo.Text = "↩ 撤销";
            this.btnUndo.UseVisualStyleBackColor = true;
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            //
            // btnExportLog
            //
            this.btnExportLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExportLog.Location = new System.Drawing.Point(217, 89);
            this.btnExportLog.Name = "btnExportLog";
            this.btnExportLog.Size = new System.Drawing.Size(61, 1);
            this.btnExportLog.TabIndex = 6;
            this.btnExportLog.Text = "导出";
            this.btnExportLog.UseVisualStyleBackColor = true;
            //
            // btnClearLog
            //
            this.btnClearLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearLog.Location = new System.Drawing.Point(7, 89);
            this.btnClearLog.Name = "btnClearLog";
            this.btnClearLog.Size = new System.Drawing.Size(0, 1);
            this.btnClearLog.TabIndex = 7;
            this.btnClearLog.Text = "清空日志";
            this.btnClearLog.UseVisualStyleBackColor = true;
            this.btnClearLog.Click += new System.EventHandler(this.btnClearLog_Click);
            //
            // grpPreview
            //
            this.grpPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPreview.Controls.Add(this.txtOriginal);
            this.grpPreview.Controls.Add(this.txtPolished);
            this.grpPreview.Controls.Add(this.lblOriginal);
            this.grpPreview.Controls.Add(this.lblPolished);
            this.grpPreview.Location = new System.Drawing.Point(3, 294);
            this.grpPreview.Name = "grpPreview";
            this.grpPreview.Size = new System.Drawing.Size(284, 230);
            this.grpPreview.TabIndex = 6;
            this.grpPreview.TabStop = false;
            this.grpPreview.Text = "预览";
            //
            // txtOriginal
            //
            this.txtOriginal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOriginal.BackColor = System.Drawing.Color.White;
            this.txtOriginal.Location = new System.Drawing.Point(7, 40);
            this.txtOriginal.Multiline = true;
            this.txtOriginal.Name = "txtOriginal";
            this.txtOriginal.ReadOnly = true;
            this.txtOriginal.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtOriginal.Size = new System.Drawing.Size(270, 75);
            this.txtOriginal.TabIndex = 0;
            //
            // txtPolished
            //
            this.txtPolished.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPolished.BackColor = System.Drawing.Color.White;
            this.txtPolished.Location = new System.Drawing.Point(7, 134);
            this.txtPolished.Multiline = true;
            this.txtPolished.Name = "txtPolished";
            this.txtPolished.ReadOnly = true;
            this.txtPolished.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtPolished.Size = new System.Drawing.Size(270, 89);
            this.txtPolished.TabIndex = 1;
            //
            // lblOriginal
            //
            this.lblOriginal.AutoSize = true;
            this.lblOriginal.Location = new System.Drawing.Point(7, 20);
            this.lblOriginal.Name = "lblOriginal";
            this.lblOriginal.Size = new System.Drawing.Size(56, 17);
            this.lblOriginal.TabIndex = 2;
            this.lblOriginal.Text = "原始文本";
            //
            // lblPolished
            //
            this.lblPolished.AutoSize = true;
            this.lblPolished.Location = new System.Drawing.Point(7, 114);
            this.lblPolished.Name = "lblPolished";
            this.lblPolished.Size = new System.Drawing.Size(56, 17);
            this.lblPolished.TabIndex = 3;
            this.lblPolished.Text = "润色结果";
            //
            // grpLog
            //
            this.grpLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpLog.Controls.Add(this.txtLog);
            this.grpLog.Location = new System.Drawing.Point(3, 530);
            this.grpLog.Name = "grpLog";
            this.grpLog.Size = new System.Drawing.Size(284, 120);
            this.grpLog.TabIndex = 7;
            this.grpLog.TabStop = false;
            this.grpLog.Text = "运行日志";
            //
            // txtLog
            //
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.txtLog.ForeColor = System.Drawing.Color.LawnGreen;
            this.txtLog.Location = new System.Drawing.Point(7, 20);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(270, 93);
            this.txtLog.TabIndex = 0;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9F);
            //
            // progressBar
            //
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(3, 289);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(284, 5);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 8;
            this.progressBar.Visible = false;
            //
            // lblStatus
            //
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblStatus.Location = new System.Drawing.Point(3, 653);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(284, 20);
            this.lblStatus.TabIndex = 9;
            this.lblStatus.Text = "就绪";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // MainPanelControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.grpLog);
            this.Controls.Add(this.grpPreview);
            this.Controls.Add(this.grpActions);
            this.Controls.Add(this.grpOptions);
            this.Controls.Add(this.cmbSectionType);
            this.Controls.Add(this.lblSection);
            this.Controls.Add(this.cmbPolishMode);
            this.Controls.Add(this.lblMode);
            this.Name = "MainPanelControl";
            this.Size = new System.Drawing.Size(290, 680);
            this.Load += new System.EventHandler(this.MainPanelControl_Load);
            this.grpOptions.ResumeLayout(false);
            this.grpOptions.PerformLayout();
            this.grpActions.ResumeLayout(false);
            this.grpPreview.ResumeLayout(false);
            this.grpPreview.PerformLayout();
            this.grpLog.ResumeLayout(false);
            this.grpLog.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblMode;
        private System.Windows.Forms.ComboBox cmbPolishMode;
        private System.Windows.Forms.Label lblSection;
        private System.Windows.Forms.ComboBox cmbSectionType;
        private System.Windows.Forms.GroupBox grpOptions;
        private System.Windows.Forms.CheckBox chkSentenceMode;
        private System.Windows.Forms.CheckBox chkTermProtect;
        private System.Windows.Forms.CheckBox chkTrackChanges;
        private System.Windows.Forms.GroupBox grpActions;
        private System.Windows.Forms.Button btnGetSelection;
        private System.Windows.Forms.Button btnPolish;
        private System.Windows.Forms.Button btnPreviewDiff;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnCopyResult;
        private System.Windows.Forms.Button btnUndo;
        private System.Windows.Forms.Button btnExportLog;
        private System.Windows.Forms.Button btnClearLog;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.GroupBox grpPreview;
        private System.Windows.Forms.TextBox txtOriginal;
        private System.Windows.Forms.TextBox txtPolished;
        private System.Windows.Forms.Label lblOriginal;
        private System.Windows.Forms.Label lblPolished;
        private System.Windows.Forms.GroupBox grpLog;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblStatus;
    }
}
