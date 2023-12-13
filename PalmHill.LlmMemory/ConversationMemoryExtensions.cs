using LLama.Common;
using LLama;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PalmHill.Llama.Models;
using PalmHill.Llama;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LLamaSharp.KernelMemory;
using Microsoft.KernelMemory;

namespace PalmHill.LlmMemory
{
    public static class ConversationMemoryExtensions
    {

        public static ServerlessLlmMemory AddLlmMemory(this IHostApplicationBuilder builder, ModelConfig? modelConfig = null)
        {
            var defaultModelConfigSection = "EmbeddingModelConfig";

            if (modelConfig == null)
            {
                //Attemt to get model config from config
                modelConfig = builder.GetModelConfigFromConfigSection(defaultModelConfigSection);
            }

            if (modelConfig == null)
            {
                throw new ArgumentNullException(nameof(modelConfig), $"The argument {modelConfig} must be supplied if there is no {defaultModelConfigSection} section in app configuartion.");
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
            builder.Services.AddSingleton(llmMemory);

            return llmMemory;
        }


    }
}
