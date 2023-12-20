using LLamaSharp.KernelMemory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.KernelMemory;
using PalmHill.Llama;
using PalmHill.Llama.Models;

namespace PalmHill.LlmMemory
{
    public static class ConversationMemoryExtensions
    {

        public static ServerlessLlmMemory AddLlmMemory(this IServiceCollection services, ModelConfig? modelConfig = null)
        {
            if (modelConfig == null)
            {
                throw new ArgumentNullException(nameof(modelConfig), $"The argument {modelConfig} must be supplied.");
            }

            //check if model is present
            var modelExsists = System.IO.File.Exists(modelConfig.ModelPath);
            if (!modelExsists)
            {
                throw new FileNotFoundException($"Model file does not exsist.", modelConfig.ModelPath);
            }

            var memoryModelConfig = new LLamaSharpConfig(modelConfig.ModelPath);
            memoryModelConfig.DefaultInferenceParams = new LLama.Common.InferenceParams();
            memoryModelConfig.DefaultInferenceParams.AntiPrompts = modelConfig.AntiPrompts;
            memoryModelConfig.ContextSize = modelConfig.ContextSize;
            memoryModelConfig.GpuLayerCount = modelConfig.GpuLayerCount;

            var memory = new KernelMemoryBuilder()
            .WithLLamaSharpDefaults(memoryModelConfig)
            .Build<MemoryServerless>();


            var llmMemory = new ServerlessLlmMemory(memory);
            services.AddSingleton(llmMemory);

            return llmMemory;
        }


    }
}
