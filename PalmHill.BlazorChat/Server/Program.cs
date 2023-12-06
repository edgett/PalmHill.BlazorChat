using LLama.Common;
using LLama;
using Microsoft.AspNetCore.ResponseCompression;
using PalmHill.Llama;
using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using PalmHill.BlazorChat.Server;

var builder = WebApplication.CreateBuilder(args);

//Make Swagger use enums.
builder.Services.AddControllers().AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

//Compress websockets traffic.
builder.Services.AddResponseCompression(opts =>
{
    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
          new[] { "application/octet-stream" });
});

//Configure swagger.
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Blazor Chat", Version = "v1" });
    c.UseInlineDefinitionsForEnums();
    c.UseAllOfToExtendReferenceSchemas();
});

//get model path from appsettings.json
string? modelPath = builder.Configuration["DefaultModelPath"]; ; // change in appsettings.json

//check if model is present
var modelExsists = System.IO.File.Exists(modelPath);
if (!modelExsists)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"Model at path {modelPath} does not exsist.");
    Console.ResetColor();
    Console.WriteLine("Press any key to exit.");
    Console.Read();
    return;
}

//Initlize Llama
ModelParams parameters = new ModelParams(modelPath ?? "")
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

//Disable compression for development.
if (!app.Environment.IsDevelopment())
{
    app.UseResponseCompression();
}
app.UseRouting();

//Configure WebSocket URI.
app.MapHub<WebSocketChat>("/chathub");

// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
// specifying the Swagger JSON endpoint.
app.UseSwagger();

app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Chat"));

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
