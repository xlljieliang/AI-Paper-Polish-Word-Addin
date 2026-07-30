namespace AIPolishCOMAddin.UI
{
    partial class SettingPanelControl
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
            this.grpApi = new System.Windows.Forms.GroupBox();
            this.lblApiBaseUrl = new System.Windows.Forms.Label();
            this.txtApiBaseUrl = new System.Windows.Forms.TextBox();
            this.lblApiKey = new System.Windows.Forms.Label();
            this.txtApiKey = new System.Windows.Forms.TextBox();
            this.lblModel = new System.Windows.Forms.Label();
            this.txtModel = new System.Windows.Forms.TextBox();
            this.lblTemperature = new System.Windows.Forms.Label();
            this.trackTemperature = new System.Windows.Forms.TrackBar();
            this.lblTempValue = new System.Windows.Forms.Label();
            this.lblMaxTokens = new System.Windows.Forms.Label();
            this.nudMaxTokens = new System.Windows.Forms.NumericUpDown();
            this.lblTimeout = new System.Windows.Forms.Label();
            this.nudTimeout = new System.Windows.Forms.NumericUpDown();
            this.lblRetry = new System.Windows.Forms.Label();
            this.nudRetry = new System.Windows.Forms.NumericUpDown();
            this.lblCustomTerms = new System.Windows.Forms.Label();
            this.txtCustomTerms = new System.Windows.Forms.TextBox();
            this.grpPresets = new System.Windows.Forms.GroupBox();
            this.btnDeepSeek = new System.Windows.Forms.Button();
            this.btnOpenAI = new System.Windows.Forms.Button();
            this.btnKimi = new System.Windows.Forms.Button();
            this.btnGLM = new System.Windows.Forms.Button();
            this.btnTongyi = new System.Windows.Forms.Button();
            this.grpTempPreset = new System.Windows.Forms.GroupBox();
            this.btnTempStrict = new System.Windows.Forms.Button();
            this.btnTempBalanced = new System.Windows.Forms.Button();
            this.btnTempFree = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnTest = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.grpApi.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackTemperature)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxTokens)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTimeout)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRetry)).BeginInit();
            this.grpPresets.SuspendLayout();
            this.grpTempPreset.SuspendLayout();
            this.SuspendLayout();
            //
            // grpApi
            //
            this.grpApi.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpApi.Controls.Add(this.lblApiBaseUrl);
            this.grpApi.Controls.Add(this.txtApiBaseUrl);
            this.grpApi.Controls.Add(this.lblApiKey);
            this.grpApi.Controls.Add(this.txtApiKey);
            this.grpApi.Controls.Add(this.lblModel);
            this.grpApi.Controls.Add(this.txtModel);
            this.grpApi.Controls.Add(this.lblTemperature);
            this.grpApi.Controls.Add(this.trackTemperature);
            this.grpApi.Controls.Add(this.lblTempValue);
            this.grpApi.Controls.Add(this.lblMaxTokens);
            this.grpApi.Controls.Add(this.nudMaxTokens);
            this.grpApi.Controls.Add(this.lblTimeout);
            this.grpApi.Controls.Add(this.nudTimeout);
            this.grpApi.Controls.Add(this.lblRetry);
            this.grpApi.Controls.Add(this.nudRetry);
            this.grpApi.Controls.Add(this.lblCustomTerms);
            this.grpApi.Controls.Add(this.txtCustomTerms);
            this.grpApi.Location = new System.Drawing.Point(3, 3);
            this.grpApi.Name = "grpApi";
            this.grpApi.Size = new System.Drawing.Size(284, 360);
            this.grpApi.TabIndex = 0;
            this.grpApi.TabStop = false;
            this.grpApi.Text = "API 配置";
            //
            // lblApiBaseUrl
            //
            this.lblApiBaseUrl.AutoSize = true;
            this.lblApiBaseUrl.Location = new System.Drawing.Point(7, 22);
            this.lblApiBaseUrl.Name = "lblApiBaseUrl";
            this.lblApiBaseUrl.Size = new System.Drawing.Size(91, 17);
            this.lblApiBaseUrl.TabIndex = 0;
            this.lblApiBaseUrl.Text = "API Base URL";
            //
            // txtApiBaseUrl
            //
            this.txtApiBaseUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtApiBaseUrl.Location = new System.Drawing.Point(7, 42);
            this.txtApiBaseUrl.Name = "txtApiBaseUrl";
            this.txtApiBaseUrl.Size = new System.Drawing.Size(270, 23);
            this.txtApiBaseUrl.TabIndex = 1;
            this.txtApiBaseUrl.Text = "https://api.deepseek.com/v1";
            //
            // lblApiKey
            //
            this.lblApiKey.AutoSize = true;
            this.lblApiKey.Location = new System.Drawing.Point(7, 72);
            this.lblApiKey.Name = "lblApiKey";
            this.lblApiKey.Size = new System.Drawing.Size(54, 17);
            this.lblApiKey.TabIndex = 2;
            this.lblApiKey.Text = "API Key";
            //
            // txtApiKey
            //
            this.txtApiKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtApiKey.Location = new System.Drawing.Point(7, 92);
            this.txtApiKey.Name = "txtApiKey";
            this.txtApiKey.PasswordChar = '●';
            this.txtApiKey.Size = new System.Drawing.Size(270, 23);
            this.txtApiKey.TabIndex = 3;
            //
            // lblModel
            //
            this.lblModel.AutoSize = true;
            this.lblModel.Location = new System.Drawing.Point(7, 122);
            this.lblModel.Name = "lblModel";
            this.lblModel.Size = new System.Drawing.Size(44, 17);
            this.lblModel.TabIndex = 4;
            this.lblModel.Text = "Model";
            //
            // txtModel
            //
            this.txtModel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtModel.Location = new System.Drawing.Point(7, 142);
            this.txtModel.Name = "txtModel";
            this.txtModel.Size = new System.Drawing.Size(270, 23);
            this.txtModel.TabIndex = 5;
            this.txtModel.Text = "deepseek-chat";
            //
            // lblTemperature
            //
            this.lblTemperature.AutoSize = true;
            this.lblTemperature.Location = new System.Drawing.Point(7, 172);
            this.lblTemperature.Name = "lblTemperature";
            this.lblTemperature.Size = new System.Drawing.Size(87, 17);
            this.lblTemperature.TabIndex = 6;
            this.lblTemperature.Text = "Temperature";
            //
            // trackTemperature
            //
            this.trackTemperature.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackTemperature.Location = new System.Drawing.Point(7, 192);
            this.trackTemperature.Maximum = 100;
            this.trackTemperature.Name = "trackTemperature";
            this.trackTemperature.Size = new System.Drawing.Size(193, 45);
            this.trackTemperature.TabIndex = 7;
            this.trackTemperature.TickFrequency = 10;
            this.trackTemperature.Value = 10;
            this.trackTemperature.Scroll += new System.EventHandler(this.trackTemperature_Scroll);
            //
            // lblTempValue
            //
            this.lblTempValue.AutoSize = true;
            this.lblTempValue.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblTempValue.Location = new System.Drawing.Point(206, 195);
            this.lblTempValue.Name = "lblTempValue";
            this.lblTempValue.Size = new System.Drawing.Size(29, 17);
            this.lblTempValue.TabIndex = 8;
            this.lblTempValue.Text = "0.1";
            //
            // lblMaxTokens
            //
            this.lblMaxTokens.AutoSize = true;
            this.lblMaxTokens.Location = new System.Drawing.Point(7, 230);
            this.lblMaxTokens.Name = "lblMaxTokens";
            this.lblMaxTokens.Size = new System.Drawing.Size(74, 17);
            this.lblMaxTokens.TabIndex = 9;
            this.lblMaxTokens.Text = "Max Tokens";
            //
            // nudMaxTokens
            //
            this.nudMaxTokens.Increment = new decimal(new int[] { 256, 0, 0, 0 });
            this.nudMaxTokens.Location = new System.Drawing.Point(100, 228);
            this.nudMaxTokens.Maximum = new decimal(new int[] { 32768, 0, 0, 0 });
            this.nudMaxTokens.Minimum = new decimal(new int[] { 128, 0, 0, 0 });
            this.nudMaxTokens.Name = "nudMaxTokens";
            this.nudMaxTokens.Size = new System.Drawing.Size(100, 23);
            this.nudMaxTokens.TabIndex = 10;
            this.nudMaxTokens.Value = new decimal(new int[] { 2048, 0, 0, 0 });
            //
            // lblTimeout
            //
            this.lblTimeout.AutoSize = true;
            this.lblTimeout.Location = new System.Drawing.Point(7, 258);
            this.lblTimeout.Name = "lblTimeout";
            this.lblTimeout.Size = new System.Drawing.Size(68, 17);
            this.lblTimeout.TabIndex = 11;
            this.lblTimeout.Text = "超时(秒)";
            //
            // nudTimeout
            //
            this.nudTimeout.Location = new System.Drawing.Point(100, 256);
            this.nudTimeout.Maximum = new decimal(new int[] { 300, 0, 0, 0 });
            this.nudTimeout.Minimum = new decimal(new int[] { 10, 0, 0, 0 });
            this.nudTimeout.Name = "nudTimeout";
            this.nudTimeout.Size = new System.Drawing.Size(100, 23);
            this.nudTimeout.TabIndex = 12;
            this.nudTimeout.Value = new decimal(new int[] { 60, 0, 0, 0 });
            //
            // lblRetry
            //
            this.lblRetry.AutoSize = true;
            this.lblRetry.Location = new System.Drawing.Point(7, 286);
            this.lblRetry.Name = "lblRetry";
            this.lblRetry.Size = new System.Drawing.Size(56, 17);
            this.lblRetry.TabIndex = 13;
            this.lblRetry.Text = "重试次数";
            //
            // nudRetry
            //
            this.nudRetry.Location = new System.Drawing.Point(100, 284);
            this.nudRetry.Maximum = new decimal(new int[] { 5, 0, 0, 0 });
            this.nudRetry.Name = "nudRetry";
            this.nudRetry.Size = new System.Drawing.Size(100, 23);
            this.nudRetry.TabIndex = 14;
            this.nudRetry.Value = new decimal(new int[] { 2, 0, 0, 0 });
            //
            // lblCustomTerms
            //
            this.lblCustomTerms.AutoSize = true;
            this.lblCustomTerms.Location = new System.Drawing.Point(7, 316);
            this.lblCustomTerms.Name = "lblCustomTerms";
            this.lblCustomTerms.Size = new System.Drawing.Size(160, 17);
            this.lblCustomTerms.TabIndex = 15;
            this.lblCustomTerms.Text = "自定义术语（逗号分隔）";
            //
            // txtCustomTerms
            //
            this.txtCustomTerms.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCustomTerms.Location = new System.Drawing.Point(7, 336);
            this.txtCustomTerms.Name = "txtCustomTerms";
            this.txtCustomTerms.Size = new System.Drawing.Size(270, 23);
            this.txtCustomTerms.TabIndex = 16;
            //
            // grpPresets
            //
            this.grpPresets.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPresets.Controls.Add(this.btnDeepSeek);
            this.grpPresets.Controls.Add(this.btnOpenAI);
            this.grpPresets.Controls.Add(this.btnKimi);
            this.grpPresets.Controls.Add(this.btnGLM);
            this.grpPresets.Controls.Add(this.btnTongyi);
            this.grpPresets.Location = new System.Drawing.Point(3, 369);
            this.grpPresets.Name = "grpPresets";
            this.grpPresets.Size = new System.Drawing.Size(284, 65);
            this.grpPresets.TabIndex = 1;
            this.grpPresets.TabStop = false;
            this.grpPresets.Text = "快速预设";
            //
            // btnDeepSeek
            //
            this.btnDeepSeek.Location = new System.Drawing.Point(7, 22);
            this.btnDeepSeek.Name = "btnDeepSeek";
            this.btnDeepSeek.Size = new System.Drawing.Size(52, 30);
            this.btnDeepSeek.TabIndex = 0;
            this.btnDeepSeek.Text = "DeepSeek";
            this.btnDeepSeek.UseVisualStyleBackColor = true;
            this.btnDeepSeek.Click += new System.EventHandler(this.btnPreset_Click);
            //
            // btnOpenAI
            //
            this.btnOpenAI.Location = new System.Drawing.Point(63, 22);
            this.btnOpenAI.Name = "btnOpenAI";
            this.btnOpenAI.Size = new System.Drawing.Size(52, 30);
            this.btnOpenAI.TabIndex = 1;
            this.btnOpenAI.Text = "GPT-4o";
            this.btnOpenAI.UseVisualStyleBackColor = true;
            this.btnOpenAI.Click += new System.EventHandler(this.btnPreset_Click);
            //
            // btnKimi
            //
            this.btnKimi.Location = new System.Drawing.Point(119, 22);
            this.btnKimi.Name = "btnKimi";
            this.btnKimi.Size = new System.Drawing.Size(46, 30);
            this.btnKimi.TabIndex = 2;
            this.btnKimi.Text = "Kimi";
            this.btnKimi.UseVisualStyleBackColor = true;
            this.btnKimi.Click += new System.EventHandler(this.btnPreset_Click);
            //
            // btnGLM
            //
            this.btnGLM.Location = new System.Drawing.Point(169, 22);
            this.btnGLM.Name = "btnGLM";
            this.btnGLM.Size = new System.Drawing.Size(46, 30);
            this.btnGLM.TabIndex = 3;
            this.btnGLM.Text = "GLM-4";
            this.btnGLM.UseVisualStyleBackColor = true;
            this.btnGLM.Click += new System.EventHandler(this.btnPreset_Click);
            //
            // btnTongyi
            //
            this.btnTongyi.Location = new System.Drawing.Point(219, 22);
            this.btnTongyi.Name = "btnTongyi";
            this.btnTongyi.Size = new System.Drawing.Size(58, 30);
            this.btnTongyi.TabIndex = 4;
            this.btnTongyi.Text = "通义千问";
            this.btnTongyi.UseVisualStyleBackColor = true;
            this.btnTongyi.Click += new System.EventHandler(this.btnPreset_Click);
            //
            // grpTempPreset
            //
            this.grpTempPreset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpTempPreset.Controls.Add(this.btnTempStrict);
            this.grpTempPreset.Controls.Add(this.btnTempBalanced);
            this.grpTempPreset.Controls.Add(this.btnTempFree);
            this.grpTempPreset.Location = new System.Drawing.Point(3, 440);
            this.grpTempPreset.Name = "grpTempPreset";
            this.grpTempPreset.Size = new System.Drawing.Size(284, 45);
            this.grpTempPreset.TabIndex = 2;
            this.grpTempPreset.TabStop = false;
            this.grpTempPreset.Text = "温度预设";
            //
            // btnTempStrict
            //
            this.btnTempStrict.Location = new System.Drawing.Point(7, 18);
            this.btnTempStrict.Name = "btnTempStrict";
            this.btnTempStrict.Size = new System.Drawing.Size(85, 23);
            this.btnTempStrict.TabIndex = 0;
            this.btnTempStrict.Text = "严谨 0.05";
            this.btnTempStrict.UseVisualStyleBackColor = true;
            this.btnTempStrict.Click += new System.EventHandler(this.btnTempPreset_Click);
            //
            // btnTempBalanced
            //
            this.btnTempBalanced.Location = new System.Drawing.Point(98, 18);
            this.btnTempBalanced.Name = "btnTempBalanced";
            this.btnTempBalanced.Size = new System.Drawing.Size(85, 23);
            this.btnTempBalanced.TabIndex = 1;
            this.btnTempBalanced.Text = "平衡 0.20";
            this.btnTempBalanced.UseVisualStyleBackColor = true;
            this.btnTempBalanced.Click += new System.EventHandler(this.btnTempPreset_Click);
            //
            // btnTempFree
            //
            this.btnTempFree.Location = new System.Drawing.Point(189, 18);
            this.btnTempFree.Name = "btnTempFree";
            this.btnTempFree.Size = new System.Drawing.Size(85, 23);
            this.btnTempFree.TabIndex = 2;
            this.btnTempFree.Text = "自由 0.60";
            this.btnTempFree.UseVisualStyleBackColor = true;
            this.btnTempFree.Click += new System.EventHandler(this.btnTempPreset_Click);
            //
            // btnSave
            //
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(3, 491);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(135, 36);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "💾 保存设置";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            //
            // btnTest
            //
            this.btnTest.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTest.Location = new System.Drawing.Point(144, 491);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(143, 36);
            this.btnTest.TabIndex = 4;
            this.btnTest.Text = "🔌 测试连通性";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            //
            // btnReset
            //
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReset.ForeColor = System.Drawing.Color.Gray;
            this.btnReset.Location = new System.Drawing.Point(3, 533);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(284, 30);
            this.btnReset.TabIndex = 5;
            this.btnReset.Text = "🔄 重置全部设置";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            //
            // lblStatus
            //
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblStatus.Location = new System.Drawing.Point(3, 566);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(284, 50);
            this.lblStatus.TabIndex = 6;
            this.lblStatus.Text = "请在右侧「润色工作台」开始使用";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // SettingPanelControl
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.grpApi);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.grpPresets);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.grpTempPreset);
            this.Controls.Add(this.btnSave);
            this.Name = "SettingPanelControl";
            this.Size = new System.Drawing.Size(290, 680);
            this.Load += new System.EventHandler(this.SettingPanelControl_Load);
            this.grpApi.ResumeLayout(false);
            this.grpApi.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackTemperature)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudMaxTokens)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudTimeout)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudRetry)).EndInit();
            this.grpPresets.ResumeLayout(false);
            this.grpTempPreset.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.GroupBox grpApi;
        private System.Windows.Forms.Label lblApiBaseUrl;
        private System.Windows.Forms.TextBox txtApiBaseUrl;
        private System.Windows.Forms.Label lblApiKey;
        private System.Windows.Forms.TextBox txtApiKey;
        private System.Windows.Forms.Label lblModel;
        private System.Windows.Forms.TextBox txtModel;
        private System.Windows.Forms.Label lblTemperature;
        private System.Windows.Forms.TrackBar trackTemperature;
        private System.Windows.Forms.Label lblTempValue;
        private System.Windows.Forms.Label lblMaxTokens;
        private System.Windows.Forms.NumericUpDown nudMaxTokens;
        private System.Windows.Forms.Label lblTimeout;
        private System.Windows.Forms.NumericUpDown nudTimeout;
        private System.Windows.Forms.Label lblRetry;
        private System.Windows.Forms.NumericUpDown nudRetry;
        private System.Windows.Forms.Label lblCustomTerms;
        private System.Windows.Forms.TextBox txtCustomTerms;
        private System.Windows.Forms.GroupBox grpPresets;
        private System.Windows.Forms.Button btnDeepSeek;
        private System.Windows.Forms.Button btnOpenAI;
        private System.Windows.Forms.Button btnKimi;
        private System.Windows.Forms.Button btnGLM;
        private System.Windows.Forms.Button btnTongyi;
        private System.Windows.Forms.GroupBox grpTempPreset;
        private System.Windows.Forms.Button btnTempStrict;
        private System.Windows.Forms.Button btnTempBalanced;
        private System.Windows.Forms.Button btnTempFree;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnTest;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Label lblStatus;
    }
}
