using LLama;
using LLama.Common;

namespace PalmHill.Llama.Models
{
    public class InjectedModel
    {
        public LLamaWeights Model { get; }
        public ModelParams ModelParams { get; }
        public List<string> DefaultAntiPrompts { get; set; }

        public InjectedModel(LLamaWeights model, ModelParams modelParams, List<string> defaultAntiPrompts)
        {
            Model = model;
            ModelParams = modelParams;
            DefaultAntiPrompts = defaultAntiPrompts;
        }
    }
}
