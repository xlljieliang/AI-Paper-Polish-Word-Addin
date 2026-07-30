using System.ComponentModel;

namespace AIPolishCOMAddin.Models
{
    /// <summary>
    /// 润色模式枚举 - 8种面向AI顶会的润色模式
    /// </summary>
    public enum PolishMode
    {
        [Description("🔬 AI顶会学术润色")]
        AcademicPolish = 0,

        [Description("📝 语法纠错+表达修正")]
        GrammarOnly = 1,

        [Description("✂️ 精简压缩")]
        Condense = 2,

        [Description("📈 扩写完善")]
        Expand = 3,

        [Description("🌐 中文转英文顶会文风")]
        ChineseToEnglish = 4,

        [Description("🔄 句式改写重排")]
        Paraphrase = 5,

        [Description("🧪 实验部分专项润色")]
        ExperimentPolish = 6,

        [Description("💡 Discussion专项润色")]
        DiscussionPolish = 7
    }

    /// <summary>
    /// 章节类型 - 用于prompt微调适配不同章节
    /// </summary>
    public enum SectionType
    {
        [Description("通用")]
        General = 0,

        [Description("Abstract 摘要")]
        Abstract = 1,

        [Description("Introduction 引言")]
        Introduction = 2,

        [Description("Related Work 相关工作")]
        RelatedWork = 3,

        [Description("Method 方法")]
        Method = 4,

        [Description("Experiment 实验")]
        Experiment = 5,

        [Description("Discussion 讨论")]
        Discussion = 6,

        [Description("Conclusion 结论")]
        Conclusion = 7
    }

    /// <summary>
    /// 润色模式的元数据辅助类
    /// </summary>
    public static class PolishModeHelper
    {
        /// <summary>
        /// 获取所有润色模式的显示名称列表（用于下拉框）
        /// </summary>
        public static string[] GetAllModeDescriptions()
        {
            return new string[]
            {
                "🔬 AI顶会学术润色（NeurIPS/ICLR/CVPR）",
                "📝 语法纠错+表达修正",
                "✂️ 精简压缩（适合Abstract）",
                "📈 扩写完善（适合Introduction）",
                "🌐 中文转英文顶会文风",
                "🔄 句式改写重排（降重友好）",
                "🧪 实验部分专项润色",
                "💡 Discussion专项润色"
            };
        }

        /// <summary>
        /// 根据下拉框索引获取对应的润色模式
        /// </summary>
        public static PolishMode GetModeByIndex(int index)
        {
            if (index < 0 || index > 7)
                return PolishMode.AcademicPolish;
            return (PolishMode)index;
        }
    }
}
