using System.Text.Json;

namespace PalmHill.Llama.Models
{
    public class ModelConfig
    {
        public string ModelPath { get; set; } = "";
        public int GpuLayerCount { get; set; } = 20;
        public uint ContextSize { get; set; } = 2048;
        public int Gpu { get; set; } = 0;
        public List<string> AntiPrompts { get; set; } = [];

        //TODO: Add FromJson method
        public static ModelConfig? FromJson(string json)
        {
            var modelConfig = JsonSerializer.Deserialize<ModelConfig>(json);
            return modelConfig;
        }

        //TODO: Add ToJson method
        public string ToJson()
        {
            var configJson =  JsonSerializer.Serialize(this);
            return configJson;
        }
        public string ModelName { get => System.IO.Path.GetFileName(ModelPath);}
    }
}
