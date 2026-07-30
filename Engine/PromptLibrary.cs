using AIPolishCOMAddin.Models;

namespace AIPolishCOMAddin.Engine
{
    /// <summary>
    /// Prompt 库 — 面向 AI 顶会的全套润色 Prompt
    /// 包含：基础系统前缀 + 8种模式子Prompt + 章节微调 + 逐句模式追加
    /// </summary>
    public static class PromptLibrary
    {
        /// <summary>
        /// 基础系统 Prompt（公共前缀，所有模式共享）
        /// </summary>
        private const string BASE_SYSTEM_PROMPT =
@"你是人工智能领域顶会论文写作助手，面向NeurIPS, ICLR, ICML, CVPR, ICCV, ECCV, AAAI。

硬性规则：
1. 绝对不要修改科学结论、实验结果、数值、模型名称、数据集、评价指标。
2. 文本中[[TERM_xxx]]是术语占位符，禁止修改、删除、改写任何占位符。
3. 只优化表达、逻辑衔接、学术句式；不编造新实验、不编造不存在观点。
4. 不要输出解释、不要输出总结，只输出处理完成后的文本。
5. 严格保留原文段落换行结构。
";

        /// <summary>
        /// 获取完整系统 Prompt（基础 + 模式特定 + 章节微调 + 逐句标记）
        /// </summary>
        public static string BuildSystemPrompt(PolishMode mode, SectionType section, bool isSentenceMode)
        {
            string prompt = BASE_SYSTEM_PROMPT;

            // 追加模式特定 Prompt
            prompt += GetModeSpecificPrompt(mode);

            // 追加章节微调
            if (section != SectionType.General)
            {
                prompt += GetSectionPrompt(section);
            }

            return prompt;
        }

        /// <summary>
        /// 构建用户 Prompt — 实际要润色的文本
        /// </summary>
        public static string BuildUserPrompt(string textToPolish, PolishMode mode)
        {
            // 中文转英文模式下不需要额外包裹
            if (mode == PolishMode.ChineseToEnglish)
            {
                return textToPolish;
            }

            return textToPolish;
        }

        /// <summary>
        /// 获取逐句模式的追加指令
        /// </summary>
        public static string GetSentenceModeSuffix()
        {
            return "\n注意：你只处理当前这一个句子，输出处理后的单句，不要添加额外解释。";
        }

        /// <summary>
        /// 8种模式的模式特定 Prompt
        /// </summary>
        private static string GetModeSpecificPrompt(PolishMode mode)
        {
            return mode switch
            {
                PolishMode.AcademicPolish =>
        @"
【模式：AI顶会学术润色】
优化英文/中文的学术表达，修正不通顺语句，强化逻辑衔接，去除口语化表达，保持原意不变。
- 改进用词精准度和学术性
- 优化句子流利度和可读性
- 强化段落内逻辑衔接
- 适合方法、实验章节
- 不改动任何技术细节和科学结论",

                PolishMode.GrammarOnly =>
        @"
【模式：语法纠错+表达修正】
仅修正语法错误、拼写错误、标点错误。
最大程度保留原有句式和表达，尽量不改写句子结构。
- 修正主谓一致、时态、冠词、介词等语法问题
- 修正拼写错误
- 修正标点符号
- 适合初稿校对，不改变原文风格",

                PolishMode.Condense =>
        @"
【模式：精简压缩】
在不丢失关键信息的前提下压缩冗余表达，保持学术严谨风格。
- 删除重复表述
- 简化冗长句式
- 合并冗余的修饰语
- 适合摘要Abstract
- 保留所有技术术语、数据、指标",

                PolishMode.Expand =>
        @"
【模式：扩写完善】
在不改动原意基础上，补充适当的逻辑过渡句，强化论证链条。
- 补充段落间的逻辑衔接
- 扩充过于简略的论述
- 增加学术表达规范度
- 适合Introduction、Related Work
- 不要凭空捏造实验结果或不存在观点",

                PolishMode.ChineseToEnglish =>
        @"
【模式：中文转英文顶会文风】
将中文论文内容翻译成高质量AI顶会英文写作风格。
- 符合CV/ML/NLP领域论文英文写作习惯
- 避免中式英语（Chinglish）
- 使用规范的学术句式结构
- 保持所有技术术语正确翻译
- 不改变任何科学结论和数据
- 直接输出英文，不需要中文对照",

                PolishMode.Paraphrase =>
        @"
【模式：句式改写重排】
语义完全不变，变换句式结构，用于改写降重。
- 同义替换
- 主动/被动语态互换
- 句子成分重新排列
- 不改变科学内容和技术细节
- 保持学术严谨性",

                PolishMode.ExperimentPolish =>
        @"
【模式：实验部分专项润色】
严格保护所有实验指标、数据集、baseline模型名称。
优化实验描述语句，保持客观陈述风格。
- 实验设置描述规范化
- 结果描述优化
- 对比表述清晰化
- 绝对不修改任何数据、指标数值、模型名称
- 适合Experiment章节",

                PolishMode.DiscussionPolish =>
        @"
【模式：Discussion专项润色】
强化论证逻辑，优化对结果的分析讨论、局限性描述、未来工作展望。
- 符合AI会议Discussion写作范式
- 提升论证深度和逻辑性
- 优化对实验结果的解读表述
- 保持客观谦逊的学术语气
- 适合Discussion章节",

                _ => ""
            };
        }

        /// <summary>
        /// 章节微调 Prompt
        /// </summary>
        private static string GetSectionPrompt(SectionType section)
        {
            return section switch
            {
                SectionType.Abstract =>
        @"
【章节：Abstract】
注意摘要写作规范：简洁明了，包含问题、方法、结果、结论四要素。
保持每个句子信息密度高。",

                SectionType.Introduction =>
        @"
【章节：Introduction】
注意引言写作规范：从大背景入手，逐步聚焦到问题，突出贡献。
强化motivation和gap分析的逻辑链条。",

                SectionType.RelatedWork =>
        @"
【章节：Related Work】
注意相关工作写作规范：按主题分类讨论，指出每类工作的优缺点。
明确本文与现有工作的差异。",

                SectionType.Method =>
        @"
【章节：Method】
注意方法章节写作规范：技术描述清晰准确，公式和算法表述规范。
确保每一步的技术逻辑可理解。",

                SectionType.Experiment =>
        @"
【章节：Experiment】
注意实验章节写作规范：实验设置详尽，结果描述客观公平。
定量描述优于定性描述。",

                SectionType.Discussion =>
        @"
【章节：Discussion】
注意讨论章节写作规范：深入分析结果含义、局限性、与已有工作对比。
指出未来方向时保持务实态度。",

                SectionType.Conclusion =>
        @"
【章节：Conclusion】
注意结论写作规范：总结主要贡献和发现，不引入新内容。
简明有力，突出影响力。",

                _ => ""
            };
        }
    }
}
