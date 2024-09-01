using LLama;
using LLama.Common;

namespace PalmHill.Llama.Models
{
    public class InjectedModel
    {
        public LLamaWeights Model { get; }
        public ModelParams ModelParams { get; }
        public List<string> DefaultAntiPrompts { get; set; }

        public ModelParams EmbeddingParameters
        {
            get
            {
                var embeddingParams = new ModelParams(ModelParams.ModelPath);
                embeddingParams.Embeddings = true;
                embeddingParams.ContextSize = ModelParams.ContextSize;
                embeddingParams.GpuLayerCount = ModelParams.GpuLayerCount;
                embeddingParams.MainGpu = ModelParams.MainGpu;
                embeddingParams.SplitMode = ModelParams.SplitMode;
                embeddingParams.TensorSplits = ModelParams.TensorSplits;
                return embeddingParams;
            }
        }

        public InjectedModel(LLamaWeights model, ModelParams modelParams, List<string> defaultAntiPrompts)
        {
            Model = model;
            ModelParams = modelParams;
            DefaultAntiPrompts = defaultAntiPrompts;
        }
    }
}
