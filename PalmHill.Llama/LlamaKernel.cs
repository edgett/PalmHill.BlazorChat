using LLama;
using LLamaSharp.KernelMemory;
using LLamaSharp.SemanticKernel.ChatCompletion;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using PalmHill.Llama.Models;


namespace PalmHill.Llama
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

            kernelBuilder.Services.AddSingleton<Microsoft.KernelMemory.AI.ITextGenerator>(sp =>
            {
                var ctx = injectedModel.Model.CreateContext(injectedModel.ModelParams);
                var textGenerator = new LlamaSharpTextGenerator(injectedModel.Model, ctx);
                return textGenerator;
            });

            kernelBuilder.Services.AddKernelMemory(km =>
            {
                var embedding = new LLamaEmbedder(injectedModel.Model, injectedModel.EmbeddingParameters);
                var textEmbeddingGeneration = new LLamaSharpTextEmbeddingGenerator(embedding);
                km.WithLLamaSharpTextEmbeddingGeneration(textEmbeddingGeneration);
                km.WithCustomTextPartitioningOptions( new Microsoft.KernelMemory.Configuration.TextPartitioningOptions { 
                    MaxTokensPerParagraph = 1,
                    MaxTokensPerLine = 1,
                    OverlappingTokens = 0
                })
                .Build<MemoryServerless>();
            })
            .AddSingleton<ServerlessLlmMemory>();


            Kernel = kernelBuilder.Build();

        }
    }
}
