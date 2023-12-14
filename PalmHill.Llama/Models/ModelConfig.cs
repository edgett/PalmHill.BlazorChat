namespace PalmHill.Llama.Models
{
    public class ModelConfig
    {
        public string ModelPath { get; set; } = "";
        public int GpuLayerCount { get; set; } = 20;
        public uint ContextSize { get; set; } = 2048;
        public int Gpu { get; set; } = 0;
        public List<string> AntiPrompts { get; set; } = [];
    }
}
