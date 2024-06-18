using LLama;
using LLamaSharp.KernelMemory;
using LLamaSharp.SemanticKernel.ChatCompletion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using PalmHill.Llama.Models;
using LLamaSharp.SemanticKernel.TextEmbedding;
using Microsoft.SemanticKernel.Memory;
using LLama.Common;


namespace PalmHill.LlamaKernel
{
    public class LlamaKernel
    {

        public Kernel Kernel { get; }

        public LlamaKernel(InjectedModel injectedModel)
        {
            var kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.Services.AddSingleton<IChatCompletionService>(sp =>
            {
                var chatExecutor = new StatelessExecutor(injectedModel.Model, injectedModel.ModelParams);
                var llamaSharpChatCompletion = new LLamaSharpChatCompletion(chatExecutor);
                return llamaSharpChatCompletion;
            });
            kernelBuilder.Services.AddKernelMemory(km => {
                var embedding = new LLamaEmbedder(injectedModel.Model, injectedModel.ModelParams);
                var textEmbeddingGeneration = new LLamaSharpTextEmbeddingGenerator(embedding);
                km.WithLLamaSharpTextEmbeddingGeneration(textEmbeddingGeneration);
                km.Build<MemoryServerless>();
            });


            Kernel = kernelBuilder.Build();

        }
    }
}
