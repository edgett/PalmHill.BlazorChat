using LLamaSharp.KernelMemory;
using Microsoft.KernelMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Azure;
using Microsoft.KernelMemory.AppBuilders;



var modelWeights = new LLamaSharpConfig(@"C:\models\orca-2-7b.Q4_K_M.gguf");
var memory = new KernelMemoryBuilder()
.WithLLamaSharpDefaults(modelWeights)
.Build<MemoryServerless>();

var x = await memory.ImportDocumentAsync(@"C:\Users\localadmin\Desktop\constitution.pdf", index: "test");

var r = await memory.AskAsync("Free speech", "test");

Console.WriteLine(r.ToJson(true));
