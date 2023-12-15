using LLama;
using LLama.Common;

namespace PalmHill.Llama.Models
{
    public class InjectedModel : IAsyncDisposable, IDisposable
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

        public async ValueTask DisposeAsync()
        {
            await Task.Run(() => Model.Dispose());
        }

        public void Dispose()
        {
            Model.Dispose();
        }
    }
}
