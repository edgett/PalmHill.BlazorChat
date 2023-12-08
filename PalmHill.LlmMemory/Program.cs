using LLamaSharp.KernelMemory;
using Microsoft.KernelMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Azure;
using Microsoft.KernelMemory.AppBuilders;



var modelWeights = new LLamaSharpConfig(@"C:\models\orca-2-13b.Q6_K.gguf");
var memory = new KernelMemoryBuilder()
.WithLLamaSharpDefaults(modelWeights)
.Build<MemoryServerless>();


var x = await memory.ImportDocumentAsync(@"C:\Users\localadmin\OneDrive\Documents\Creative Outfit CRM.docx");

var r = await memory.SearchAsync("CRM");

Console.WriteLine(r.ToJson(true));
