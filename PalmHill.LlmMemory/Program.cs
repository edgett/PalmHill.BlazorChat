using LLamaSharp.KernelMemory;
using Microsoft.KernelMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Azure;
using Microsoft.KernelMemory.AppBuilders;



var memoryModelConfig = new LLamaSharpConfig(@"C:\models\mistral-7b-openorca.Q4_K_M.gguf");
memoryModelConfig.DefaultInferenceParams = new LLama.Common.InferenceParams();
memoryModelConfig.DefaultInferenceParams.AntiPrompts = new List<string> { "Question:" };
memoryModelConfig.ContextSize = 2048;
memoryModelConfig.GpuLayerCount = 50;


var memory = new KernelMemoryBuilder()
.WithLLamaSharpDefaults(memoryModelConfig)
.Build<MemoryServerless>();

var x = await memory.ImportDocumentAsync(@"C:\Users\localadmin\Desktop\constitution.pdf", index: "test");

var docIsReady = false;
while (!docIsReady)
{
    docIsReady = await memory.IsDocumentReadyAsync(x, "test");
    Console.WriteLine($"DocId {x} ready state: {docIsReady}");
    if (!docIsReady)
    { 
        System.Threading.Thread.Sleep(1000);
    }
}

var ar = await memory.AskAsync("Free speech", "test");

Console.WriteLine(ar.ToJson(true));
