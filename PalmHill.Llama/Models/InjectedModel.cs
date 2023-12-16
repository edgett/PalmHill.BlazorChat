using LLama;
using LLama.Common;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PalmHill.Llama.Models
{

    public class InjectedModel : IAsyncDisposable, IDisposable
    {
        [JsonIgnore]
        public LLamaWeights Model { get; }
        public ModelParams ModelParams { get; }
        public ModelConfig LoadConfig { get; }

        public InjectedModel(LLamaWeights model, ModelParams modelParams, ModelConfig defaultAntiPrompts)
        {
            Model = model;
            ModelParams = modelParams;
            LoadConfig = defaultAntiPrompts;
        }

        public async ValueTask DisposeAsync()
        {
            await Task.Run(() => Model.Dispose());
        }

        public void Dispose()
        {
            Model.Dispose();
        }

        public string ToJson()
        {
            var options = new JsonSerializerOptions
            {
                Converters = { new EncodingConverter() }
            };

            var json = JsonSerializer.Serialize(this, options);
            return json;
        }
    }
}
