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

        public static ServerlessLlmMemory AddLlmMemory(this IServiceCollection services, ModelConfig? textGenerationModel, ModelConfig? embeddingModel)
        {
            if (textGenerationModel == null)
            {
                throw new ArgumentNullException(nameof(textGenerationModel), $"The argument {textGenerationModel} must be supplied.");
            }

            if (embeddingModel == null)
            {
                throw new ArgumentNullException(nameof(embeddingModel), $"The argument {embeddingModel} must be supplied.");
            }

            var textGenerationModelConfig = textGenerationModel.ToLlamasharpConfig();
            var embeddingModelConfig = embeddingModel.ToLlamasharpConfig();

            var memory = new KernelMemoryBuilder()
                .WithLLamaSharpTextGeneration(textGenerationModelConfig)
                .WithLLamaSharpTextEmbeddingGeneration(embeddingModelConfig)
                .Build<MemoryServerless>();

            var llmMemory = new ServerlessLlmMemory(memory);
            services.AddSingleton(llmMemory);
            return llmMemory;

        }

        public static ServerlessLlmMemory AddLlmMemory(this IServiceCollection services, ModelConfig? modelConfig = null)
        {
            if (modelConfig == null)
            {
                throw new ArgumentNullException(nameof(modelConfig), $"The argument {modelConfig} must be supplied.");
            }

            var memoryModelConfig = modelConfig.ToLlamasharpConfig();

            var memory = new KernelMemoryBuilder()
            .WithLLamaSharpDefaults(memoryModelConfig)
            .Build<MemoryServerless>();


            var llmMemory = new ServerlessLlmMemory(memory);
            services.AddSingleton(llmMemory);

            return llmMemory;
        }
        public static LLamaSharpConfig ToLlamasharpConfig(this ModelConfig modelConfig)
        {
            //check if model is present
            var modelExsists = System.IO.File.Exists(modelConfig.ModelPath);
            if (!modelExsists)
            {
                throw new FileNotFoundException($"Model file does not exsist.", modelConfig.ModelPath);
            }

            var llamaSharpConfig = new LLamaSharpConfig(modelConfig.ModelPath);
            llamaSharpConfig.DefaultInferenceParams = new LLama.Common.InferenceParams();
            llamaSharpConfig.DefaultInferenceParams.AntiPrompts = modelConfig.AntiPrompts;
            llamaSharpConfig.ContextSize = modelConfig.ContextSize;
            llamaSharpConfig.GpuLayerCount = modelConfig.GpuLayerCount;

            return llamaSharpConfig;
        }
    }

}
