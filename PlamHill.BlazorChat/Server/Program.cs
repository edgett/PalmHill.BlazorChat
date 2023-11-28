using LLama.Common;
using LLama;
using Microsoft.AspNetCore.ResponseCompression;
using PalmHill.Llama;
using PlamHill.BlazorChat.Server;
using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
          new[] { "application/octet-stream" });
});
//Initlize LLLAMA
string modelPath = @"C:\Users\localadmin\Downloads\orca-2-13b.Q6_K.gguf"; // change it to your own model path
                                                                          // Load a model
ModelParams parameters = new ModelParams(modelPath)
{
    ContextSize = 1024,
    Seed = 1337,
    GpuLayerCount = 5,
};
LLamaWeights model = LLamaWeights.LoadFromFile(parameters);
LLamaContext modelContext = model.CreateContext(parameters);
builder.Services.AddSingleton<LLamaContext>(modelContext);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseResponseCompression();

app.UseRouting();

app.MapHub<ChatHub>("/chathub");

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

//Initlize LLLAMA

app.Run();
