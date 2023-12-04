using LLama.Common;
using LLama;
using Microsoft.AspNetCore.ResponseCompression;
using PalmHill.Llama;
using PlamHill.BlazorChat.Server;
using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
          new[] { "application/octet-stream" });
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Blazor Chat", Version = "v1" });
    c.UseInlineDefinitionsForEnums();
    c.UseAllOfToExtendReferenceSchemas();
});

//Initlize Llama
string modelPath = @"C:\models\orca-2-13b.Q6_K.gguf"; // change it to your own model path
ModelParams parameters = new ModelParams(modelPath)
{
    ContextSize = 4096,
    GpuLayerCount = 90,
};
LLamaWeights model = LLamaWeights.LoadFromFile(parameters);
builder.Services.AddSingleton(model);
builder.Services.AddSingleton(parameters);
//End Initlize Llama

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


app.MapHub<WebSocketChat>("/chathub");

app.UseSwagger();
// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
// specifying the Swagger JSON endpoint.
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Chat"));

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
