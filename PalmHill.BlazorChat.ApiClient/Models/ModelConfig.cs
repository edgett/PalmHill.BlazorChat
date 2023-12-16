using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PalmHill.BlazorChat.ApiClient.Models
{
    public class ModelConfig
    {
        public string? ModelPath { get; set; }
        public int? GpuLayerCount { get; set; }
        public uint? ContextSize { get; set; }
        public int? Gpu { get; set; }
        public List<string>? AntiPrompts { get; set; }
        public string? ModelName { get; set; }

        //TODO: Add FromJson method
        public static ModelConfig? FromJson(string json)
        {
            var modelConfig = JsonSerializer.Deserialize<ModelConfig>(json);
            return modelConfig;
        }

        //TODO: Add ToJson method
        public string ToJson()
        {
            var configJson = JsonSerializer.Serialize(this);
            return configJson;
        }
    }
}
