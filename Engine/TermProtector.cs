using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace AIPolishCOMAddin.Engine
{
    /// <summary>
    /// 术语保护模块 — 防止大模型修改 AI 顶会专有名词
    /// 流程：术语匹配 → 替换为占位符 → LLM处理后 → 还原
    /// </summary>
    public class TermProtector
    {
        // 内置 AI 顶会术语词典
        private readonly HashSet<string> _builtinTerms;
        // 用户自定义术语
        private readonly HashSet<string> _userTerms;
        // 占位符映射：占位符 → 原词
        private readonly Dictionary<string, string> _placeholderMap;

        // 占位符正则
        private static readonly Regex _placeholderPattern = new Regex(
            @"\[\[TERM_\d{3}\]\]",
            RegexOptions.Compiled);

        // 占位符计数
        private int _termCounter;

        /// <summary>
        /// 构造术语保护器
        /// </summary>
        /// <param name="customTerms">用户自定义术语列表（逗号分隔）</param>
        public TermProtector(string customTerms = "")
        {
            _builtinTerms = InitializeBuiltinTerms();
            _userTerms = ParseUserTerms(customTerms);
            _placeholderMap = new Dictionary<string, string>();
            _termCounter = 0;
        }

        /// <summary>
        /// 保护术语：将文本中的术语替换为占位符
        /// </summary>
        public string Protect(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            _placeholderMap.Clear();
            _termCounter = 0;

            string result = text;

            // 合并全部术语
            var allTerms = new HashSet<string>(_builtinTerms, StringComparer.OrdinalIgnoreCase);
            foreach (var term in _userTerms)
            {
                if (!string.IsNullOrWhiteSpace(term))
                    allTerms.Add(term.Trim());
            }

            // 按长度降序排序，避免短词先匹配导致长词无法匹配（如 "ViT" 和 "ViT-B/16"）
            var sortedTerms = new List<string>(allTerms);
            sortedTerms.Sort((a, b) => b.Length.CompareTo(a.Length));

            foreach (var term in sortedTerms)
            {
                if (string.IsNullOrWhiteSpace(term)) continue;

                string placeholder = GetOrCreatePlaceholder(term);

                // 使用正则进行整词匹配（大小写不敏感）
                string pattern = Regex.Escape(term);
                result = Regex.Replace(result, pattern, placeholder, RegexOptions.IgnoreCase);
            }

            return result;
        }

        /// <summary>
        /// 还原术语：将占位符替换回原始术语
        /// </summary>
        public string Restore(string text)
        {
            if (string.IsNullOrEmpty(text) || _placeholderMap.Count == 0)
                return text;

            string result = text;
            foreach (var kvp in _placeholderMap)
            {
                result = result.Replace(kvp.Key, kvp.Value);
            }

            // 检查是否有漏网之鱼（LLM 可能自己修改了占位符格式）
            result = _placeholderPattern.Replace(result, match =>
            {
                // 如果映射表中不存在，按原样保留（但几乎不会发生）
                return _placeholderMap.TryGetValue(match.Value, out string original)
                    ? original
                    : match.Value;
            });

            return result;
        }

        /// <summary>
        /// 是否为占位符
        /// </summary>
        public static bool IsPlaceholder(string text)
        {
            return _placeholderPattern.IsMatch(text);
        }

        /// <summary>
        /// 获取或创建占位符
        /// </summary>
        private string GetOrCreatePlaceholder(string term)
        {
            // 先查是否已有映射
            foreach (var kvp in _placeholderMap)
            {
                if (string.Equals(kvp.Value, term, StringComparison.OrdinalIgnoreCase))
                    return kvp.Key;
            }

            // 创建新的占位符
            _termCounter++;
            string placeholder = $"[[TERM_{_termCounter:D3}]]";
            _placeholderMap[placeholder] = term;
            return placeholder;
        }

        /// <summary>
        /// 更新用户自定义术语列表
        /// </summary>
        public void UpdateUserTerms(string customTerms)
        {
            _userTerms.Clear();
            foreach (var term in ParseUserTerms(customTerms))
            {
                _userTerms.Add(term);
            }
        }

        /// <summary>
        /// 解析用户自定义术语（逗号/分号/空格分隔）
        /// </summary>
        private static HashSet<string> ParseUserTerms(string input)
        {
            var terms = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (string.IsNullOrWhiteSpace(input))
                return terms;

            // 支持逗号、分号、中文逗号、空格分隔
            string[] parts = Regex.Split(input, @"[,;，；\s]+");
            foreach (var part in parts)
            {
                string trimmed = part.Trim();
                if (!string.IsNullOrWhiteSpace(trimmed))
                {
                    terms.Add(trimmed);
                }
            }

            return terms;
        }

        /// <summary>
        /// 初始化内置 AI 顶会术语词典
        /// </summary>
        private static HashSet<string> InitializeBuiltinTerms()
        {
            return new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                // ===== 经典卷积神经网络模型 =====
                "ResNet", "ResNet-18", "ResNet-34", "ResNet-50", "ResNet-101", "ResNet-152",
                "VGG", "VGG-16", "VGG-19",
                "Inception", "Inception-v1", "Inception-v2", "Inception-v3", "Inception-v4",
                "Inception-ResNet", "Inception-ResNet-v2",
                "Xception", "MobileNet", "MobileNet-v1", "MobileNet-v2", "MobileNet-v3",
                "EfficientNet", "EfficientNet-B0", "EfficientNet-B4", "EfficientNet-B7",
                "DenseNet", "DenseNet-121", "DenseNet-169", "DenseNet-201",
                "SENet", "SENet-50",
                "ConvNeXt", "ConvNeXt-T", "ConvNeXt-S", "ConvNeXt-B", "ConvNeXt-L",

                // ===== Transformer / ViT 系列 =====
                "ViT", "ViT-B/16", "ViT-B/32", "ViT-L/14", "ViT-H/14",
                "DeiT", "DeiT-S", "DeiT-B", "DeiT-T",
                "Swin", "Swin-T", "Swin-S", "Swin-B", "Swin-L",
                "Swin-v2", "Swin-v2-T", "Swin-v2-B", "Swin-v2-L",
                "PVT", "PVT-v1", "PVT-v2",
                "CvT", "T2T-ViT", "CrossViT", "PiT", "PiT-B",
                "MAE", "MAE-L", "MAE-H",
                "BEiT", "BEiT-v2", "BEiT-3",
                "DINO", "DINOv1", "DINOv2",

                // ===== CLIP / 多模态系列 =====
                "CLIP", "CLIP-ViT-B/16", "CLIP-ViT-L/14",
                "BLIP", "BLIP-2", "BLIP-3",
                "ALBEF",
                "VLMO",
                "Flamingo",
                "LLaVA", "LLaVA-1.5", "LLaVA-NeXT",

                // ===== 大语言模型 =====
                "LLaMA", "LLaMA-2", "LLaMA-3", "LLaMA-3.1",
                "Qwen", "Qwen-2", "Qwen-2.5", "Qwen-VL",
                "GPT-2", "GPT-3", "GPT-3.5", "GPT-4", "GPT-4o", "GPT-4o-mini",
                "Claude", "Claude-3", "Claude-3.5", "Claude-4",
                "Gemini", "Gemini-1.5", "Gemini-2.0",
                "Mistral", "Mistral-7B", "Mixtral", "Mixtral-8x7B",
                "DeepSeek", "DeepSeek-V2", "DeepSeek-V3", "DeepSeek-R1",
                "Gemma", "Gemma-2",
                "Phi", "Phi-3", "Phi-4",
                "T5", "T5-small", "T5-base", "T5-large", "Flan-T5",
                "BERT", "BERT-base", "BERT-large", "RoBERTa", "RoBERTa-base", "RoBERTa-large",
                "ALBERT", "DeBERTa", "DeBERTa-v3",
                "XLNet",
                "BART", "BART-large",

                // ===== Diffusion / 生成模型 =====
                "Diffusion", "DDPM", "DDIM", "SDE",
                "Stable Diffusion", "Stable Diffusion 3", "Stable Diffusion XL", "SDXL",
                "DALL-E", "DALL-E 2", "DALL-E 3",
                "Midjourney",
                "GAN", "GANs", "StyleGAN", "StyleGAN-v2", "StyleGAN-v3",
                "BigGAN", "CycleGAN", "Pix2Pix",

                // ===== 数据集 =====
                "ImageNet", "ImageNet-1K", "ImageNet-21K",
                "COCO", "COCO-Stuff", "COCO-2014", "COCO-2017",
                "MNIST", "Fashion-MNIST",
                "CIFAR-10", "CIFAR-100",
                "SVHN",
                "PASCAL VOC", "VOC-2007", "VOC-2012",
                "ADE20K",
                "Cityscapes",
                "KITTI",
                "NYU-Depth-v2",
                "SUN RGB-D",
                "LAION-5B", "LAION-400M",
                "CC3M", "CC12M",
                "SBU Captions",
                "VisDial",
                "VQA-v2",
                "GQA",
                "Clever",
                "ScanNet",
                "ModelNet-40",
                "ShapeNet",
                "SQuAD", "SQuAD-1.1", "SQuAD-2.0",
                "GLUE",
                "SuperGLUE",
                "MMLU",
                "HumanEval",
                "GSM8K",
                "MATH",

                // ===== 评价指标 =====
                "FID", "Fréchet Inception Distance",
                "IS", "Inception Score",
                "LPIPS", "Learned Perceptual Image Patch Similarity",
                "PSNR", "Peak Signal-to-Noise Ratio",
                "SSIM", "Structural Similarity Index",
                "mAP", "mean Average Precision",
                "AP", "Average Precision",
                "AR", "Average Recall",
                "IoU", "Intersection over Union",
                "mIoU", "mean Intersection over Union",
                "BLEU", "BLEU-1", "BLEU-2", "BLEU-3", "BLEU-4",
                "ROUGE", "ROUGE-L", "ROUGE-1", "ROUGE-2",
                "CIDEr",
                "METEOR",
                "SPICE",
                "Perplexity", "PPL",
                "Accuracy", "Acc.",
                "Precision", "Recall", "F1", "F1-score",
                "AUC", "AUC-ROC",
                "NLL", "Negative Log Likelihood",
                "MAE", "Mean Absolute Error",
                "MSE", "Mean Squared Error",
                "RMSE", "Root Mean Squared Error",
                "FLOPs",
                "MACs",
                "Latency",
                "Throughput",

                // ===== 常用学术术语 =====
                "SOTA", "state-of-the-art",
                "baseline", "baselines",
                "ablation",
                "cross-validation",
                "fine-tune", "fine-tuning", "fine-tuned",
                "zero-shot", "zero-shot learning",
                "few-shot", "few-shot learning",
                "multi-modal", "multimodal",
                "end-to-end",
                "out-of-distribution", "OOD",
                "data augmentation",
                "transfer learning",
                "domain adaptation",
                "self-attention", "self-attention mechanism",
                "cross-attention",
                "multi-head attention", "MHSA",
                "layer normalization", "LayerNorm",
                "batch normalization", "BatchNorm", "BN",
                "stochastic gradient descent", "SGD",
                "Adam", "AdamW",
                "ReLU", "GELU", "SiLU", "Swish",
                "softmax",
                "positional encoding",
                "positional embedding",
                "masked language modeling", "MLM",
                "contrastive learning",
                "knowledge distillation",
                "teacher-student",
                "variational autoencoder", "VAE",
                "autoencoder",
                "backbone",
                "neck",
                "head",
                "embedding", "embeddings",
                "latent space",
                "representation learning",
                "self-supervised", "self-supervised learning",
                "semi-supervised", "semi-supervised learning",
                "unsupervised", "unsupervised learning",
                "supervised", "supervised learning",
                "reinforcement learning", "RL",
                "reinforcement learning from human feedback", "RLHF",
                "direct preference optimization", "DPO",
                "chain-of-thought", "CoT",
                "retrieval-augmented generation", "RAG",
                "large language model", "LLM", "LLMs",
                "vision-language model", "VLM", "VLMs",
                "foundation model",
                "parameter-efficient fine-tuning", "PEFT",
                "low-rank adaptation", "LoRA",
                "quantization",
                "pruning",
                "distillation",
                "mixup",
                "CutMix",
                "RandAugment",
                "AdamW", "Adam",

                // ===== 会议名称 =====
                "NeurIPS", "ICLR", "ICML", "CVPR", "ICCV", "ECCV", "AAAI",
                "IJCAI", "ACL", "EMNLP", "NAACL", "EACL",
                "CoRL", "RSS", "ICRA", "IROS",
                "WACV", "ACCV", "BMVC",
                "SIGGRAPH", "SIGGRAPH Asia",
                "ACM MM",
                "ECCV",
                "UAI", "AISTATS", "COLT",
                "ICLR", "ICLR 2024", "ICLR 2025",
                "NeurIPS 2024", "NeurIPS 2025",
                "CVPR 2024", "CVPR 2025",
            };
        }
    }
}
