using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PalmHill.BlazorChat.ApiClient.Models
{
    public record ModelParams 
    {
        public uint? ContextSize { get; set; }

        public int? MainGpu { get; set; }

        public int? GpuLayerCount { get; set; }


        public uint? Seed { get; set; }


        public bool? UseFp16Memory { get; set; }


        public bool? UseMemorymap { get; set; }


        public bool? UseMemoryLock { get; set; }

        public bool? Perplexity { get; set; }

        public string? ModelPath { get; set; }

        //public AdapterCollection LoraAdapters { get; set; } = new AdapterCollection();


        public string? LoraBase { get; set; }


        public uint? Threads { get; set; }

        public uint? BatchThreads { get; set; }

        public uint? BatchSize { get; set; }


        public bool? EmbeddingMode { get; set; }


        public float[] TensorSplits { get; set; } = new float[0];


        public float? RopeFrequencyBase { get; set; }

        public float? RopeFrequencyScale { get; set; }

        public float? YarnExtrapolationFactor { get; set; }

        public float? YarnAttentionFactor { get; set; }

        public float? YarnBetaFast { get; set; }

        public float? YarnBetaSlow { get; set; }

        public uint? YarnOriginalContext { get; set; }

        public RopeScalingType? YarnScalingType { get; set; }

        public bool? MulMatQ { get; set; }

        public bool? VocabOnly { get; set; }

        public string? Encoding { get; set; }
    }

    public enum RopeScalingType : sbyte
    {
        LLAMA_ROPE_SCALING_UNSPECIFIED = -1,
        LLAMA_ROPE_SCALING_NONE,
        LLAMA_ROPE_SCALING_LINEAR,
        LLAMA_ROPE_SCALING_YARN
    }
}
