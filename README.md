# 🎯 AI论文润色助手 — Word COM加载项

> **专为 AI 顶会论文设计的 Word 润色插件**  
> 支持 NeurIPS / ICLR / ICML / CVPR / ICCV / ECCV / AAAI  
> 基于大模型 API，原生 Word TrackChanges 修订模式

---

## 📦 安装指南

### 系统要求

| 项目 | 要求 |
|------|------|
| 操作系统 | Windows 10 / 11（64位） |
| Word 版本 | Microsoft Word 2016 / 2019 / Microsoft 365（仅限 Windows） |
| .NET 版本 | .NET Framework 4.8（Win10/11 自带） |
| 大模型 API | 任意 OpenAI 兼容 API（DeepSeek / OpenAI / Kimi / 智谱 / 通义等） |

> ⚠️ **不支持 Mac Word**（VSTO 技术仅限 Windows）

### 一键安装（推荐）

```
1. 将整个 AIPolishCOMAddin 文件夹放到一个固定位置
   （例如 D:\Tools\AIPolish\，安装后不要再移动！）

2. 双击运行 install.bat
   ╰→ 自动注册插件到 Word

3. 打开 Word → 文件 → 选项 → 加载项
   → 管理: [COM加载项] → 转到
   → 勾选 "AI论文润色助手" → 确定

4. 右侧出现侧边栏 → 首次使用会自动引导配置 API
```

> **install.bat 做了什么？**
> - 检查 dll 文件是否存在
> - 自动在注册表中注册 Word COM 加载项
> - 验证安装结果

### 手动安装

如果不使用 install.bat，也可以手动操作：

```
1. 用记事本打开 Register_Addin.reg
2. 将 [INSTALL_DIR] 替换为 dll 的实际路径（例如 D:\\Tools\\AIPolish）
3. 保存，双击导入注册表
4. 在 Word COM 加载项列表中启用
```

---

## 🗑️ 卸载指南

### 方式一：使用卸载脚本（推荐）

```
双击运行 uninstall.bat
╰→ 自动从 Word 移除插件注册
╰→ 可选：是否同时删除 API 配置数据
```

### 方式二：手动卸载

```
1. 双击 Unregister_Addin.reg
2. （可选）删除注册表设置：
   HKCU\Software\AIPaperPolishAddin
3. 删除插件文件夹
```

---

## 🚀 功能详解

### 界面布局

插件加载后，Word 右侧出现固定侧边栏，分为两个 Tab：

```
┌─ 润色工作台 ─────────────────────┐
│  润色模式： [下拉选择 ▼]          │
│  ☑ 启用逐句润色                   │
│  ☑ 保护专业术语                   │
│  ☑ Word原生修订模式               │
│                                    │
│  [获取选中内容]  [执行AI润色]     │
│  [预览差异]     [应用到文档]      │
│  [复制结果]     [撤销]           │
│                                    │
│  ┌─ 原文预览 ──────────────────┐ │
│  │                              │ │
│  └──────────────────────────────┘ │
│  ┌─ 润色结果 (diff高亮) ───────┐ │
│  │                              │ │
│  └──────────────────────────────┘ │
│  ┌─ 运行日志 ──────────────────┐ │
│  │                              │ │
│  └──────────────────────────────┘ │
└────────────────────────────────────┘

┌─ 模型设置 ───────────────────────┐
│  API Base URL: [_______________] │
│  API Key:      [_______________] │
│  Model:        [_______________] │
│  Temperature:  ═══●══════ 0.1   │
│  Max Tokens:   [2048]           │
│  超时(秒):     [60]             │
│  重试次数:     [2]              │
│                                    │
│  [DeepSeek] [GPT-4o] [Kimi] [GLM]│
│  [保存设置]  [测试连通性]        │
│  [重置全部设置]                   │
└────────────────────────────────────┘
```

### 🎨 8 种润色模式

| # | 模式名称 | 适用场景 |
|---|---------|---------|
| 1 | 🔬 AI顶会学术润色 | 方法、实验章节，优化句式，不改科学结论 |
| 2 | 📝 语法纠错+表达修正 | 初稿校对，只改语法拼写，保留原句结构 |
| 3 | ✂️ 精简压缩 | 摘要 Abstract，压缩冗余，保持学术严谨 |
| 4 | 📈 扩写完善 | Introduction、Related Work，补充逻辑衔接 |
| 5 | 🌐 中文转英文顶会文风 | 中文原稿 → CVPR/ICML 风格英文 |
| 6 | 🔄 句式改写重排 | 降重友好，变换句式，不改变语义 |
| 7 | 🧪 实验部分专项润色 | 保护指标/数据集/模型名，Experiment 章节 |
| 8 | 💡 Discussion 专项润色 | 强化论证逻辑，结果分析、局限性描述 |

### 🧠 章节感知

选择润色章节类型（Abstract / Intro / Related Work / Method / Experiment / Discussion / Conclusion），Prompt 自动微调，适配不同章节的写作范式。

### 🛡️ 术语保护（AI 顶会专用）

内置词典自动保护以下内容不被大模型篡改：

| 类别 | 示例 |
|------|------|
| 模型名 | ResNet, ViT, CLIP, LLaMA, Qwen, Diffusion, Transformer, U-Net ... |
| 数据集 | ImageNet, COCO, MNIST, CIFAR-10, COCO-Stuff ... |
| 评价指标 | FID, IS, LPIPS, mAP, PSNR, SSIM, AUC ... |

**支持用户自定义术语**：在设置面板中添加项目专用术语，逗号分隔。

### 📝 Word 原生修订模式（TrackChanges）

**这是本插件的核心亮点，对标小绿鲸的修订体验。**

- ✅ 修改内容以 **Word 原生修订标记** 写入文档
- ✅ **红色删除线** = 删掉的内容 | **绿色下划线** = 新增的内容
- ✅ 与 Word「审阅」功能完全兼容
- ✅ 用户可以使用 Word「接受/拒绝修订」逐条确认
- ✅ 可在「审阅」窗格中查看所有修改

**关闭修订模式**：直接替换选中文本，不留痕迹。

### 🔄 逐句润色

- 将选中段落拆分为独立句子
- 逐句调用大模型（效果更好，精度更高）
- 显示进度：「正在处理第 3/17 句」
- 支持中途**取消**（逐句过程中随时终止）

### 🖱️ 右键快捷菜单

在 Word 文档中选中文本 → 右键 → **「AI润色选中段落」**，无需打开侧边栏即可调用。

### 📊 Token 估算

调用前显示预估消耗：
```
预估输入token: 1,234 | 预估输出token: ~500
```

### 📤 导出润色记录

导出 Markdown 格式的原文-润色对照表，方便复盘和校对。

---

## ⚙️ 设置说明

### 注册表持久化

所有配置保存在 Windows 注册表中：

```
HKEY_CURRENT_USER\Software\AIPaperPolishAddin
```

- 首次配置 → 永久保存 → 每次启动自动加载
- 支持随时修改和重置
- 不需要任何配置文件

### 推荐模型参数

| 模型 | Base URL | Model 名 | Temperature |
|------|---------|---------|-------------|
| DeepSeek-V3 | `https://api.deepseek.com/v1` | `deepseek-chat` | 0.1 |
| OpenAI GPT-4o-mini | `https://api.openai.com/v1` | `gpt-4o-mini` | 0.2 |
| Kimi (月之暗面) | `https://api.moonshot.cn/v1` | `moonshot-v1-8k` | 0.15 |
| 智谱 GLM-4-Flash | `https://open.bigmodel.cn/api/paas/v4` | `glm-4-flash` | 0.1 |
| 通义千问 | `https://dashscope.aliyuncs.com/compatible-mode/v1` | `qwen-turbo` | 0.1 |

### Temperature 建议

| 场景 | 推荐值 | 说明 |
|------|--------|------|
| 严谨校对 | **0.05** | 几乎不改原意，仅修语法错误 |
| 平衡润色 | **0.20** | 优化表达，保持原意 |
| 自由改写 | **0.60** | 较大幅度改写，适合降重 |

---

## 🔧 开发说明

### 编译环境

- Visual Studio 2022 Community+（需安装 "Office/SharePoint 开发" 工作负载）
- .NET Framework 4.8 SDK
- 目标平台：Any CPU

### 编译步骤

```
1. 用 Visual Studio 打开 AIPolishCOMAddin.csproj
2. 右键解决方案 → 重新生成
3. 编译输出：bin\Release\AIPolishCOMAddin.dll
4. 将 dll 复制到安装目录
5. 运行 install.bat 注册
```

### 项目结构

```
AIPolishCOMAddin/
├── ThisAddIn.cs                 # 插件入口，Word事件钩子
├── UI/
│   ├── MainPanelControl.cs      # 润色工作台面板
│   └── SettingPanelControl.cs   # 模型设置面板
├── Engine/
│   ├── PolishEngine.cs          # 润色引擎编排
│   ├── PromptLibrary.cs         # 8套Prompt库
│   ├── SentenceSplitter.cs      # 中英文分句器
│   ├── TermProtector.cs         # 术语保护模块
│   ├── TrackChangesInjector.cs  # Word修订注入
│   └── InMemoryDiff.cs          # 文本差异比对
├── Infrastructure/
│   ├── LLMClient.cs             # OpenAI兼容HTTP客户端
│   ├── RegistrySettings.cs      # 注册表读写封装
│   └── Logger.cs                # 日志模块
├── Models/
│   └── PolishMode.cs            # 润色模式枚举
│   └── PolishResult.cs          # 润色结果数据
├── Utils/
│   └── WordHelper.cs            # Word辅助工具
├── install.bat                  # 一键安装脚本
├── uninstall.bat                # 卸载脚本
├── Register_Addin.reg           # 注册表注册脚本
├── Unregister_Addin.reg         # 注册表卸载脚本
└── README.md                    # 本文件
```

---

## ❓ 常见问题

### Q: 插件安装后 Word 里看不到？
A: 依次检查：
1. Word → 文件 → 选项 → 加载项 → 管理: [COM加载项] → 转到
2. 看列表中是否有 "AI论文润色助手" 并已勾选
3. 重新运行 install.bat 确认路径正确
4. 确认 .NET Framework 4.8 已安装

### Q: 润色后格式乱了？
A: 插件会尽力保护格式。如果遇到格式问题：
- 使用「撤销」按钮回滚（Ctrl+Z）
- 关闭「逐句润色」试试（整段润色格式更稳定）
- 关闭「修订模式」试试

### Q: 大模型调用失败？
A: 检查以下内容：
1. 设置面板 → 点「测试连通性」
2. 确认 API Key 有效
3. 确认 Base URL 正确（注意 `/v1` 后缀）
4. 确认模型名正确
5. 确认账户余额充足

### Q: 逐句润色太慢？
A: 逐句润色是串行调用，速度取决于 API 响应时间。建议：
- 短段落用逐句（质量高）
- 长段落（>10句）用整段润色（速度快）
- 降低 Temperature 可加快响应

### Q: 插件会修改我的文档吗？
A: 只在**显式点击「应用到文档」** 时才修改。预览差异阶段不会触碰文档。开启修订模式后，所有修改都可撤销。

### Q: 可以离线使用吗？
A: 不可以。插件需要调用云端大模型 API，必须联网。

---

## ⚠️ 已知限制

| 限制 | 说明 |
|------|------|
| 平台 | 仅 Windows Word（2016/2019/365），不支持 Mac |
| 依赖 | 需要 .NET Framework 4.8 和 VSTO 运行时（Windows 自带） |
| 联网 | 必须联网调用大模型 API |
| 复杂文档 | 含大量域、公式、脚注的文档，修订模式可能偶发格式问题（保留撤销入口） |
| 长文本 | 单次建议不超过 4000 tokens（约 3000 英文词 / 2000 中文字） |

---

## 📄 许可

本项目仅供个人学习和使用。使用本插件调用大模型 API 所产生的费用由用户自行承担。

---

*Made with ❤️ for AI researchers who write papers.*
