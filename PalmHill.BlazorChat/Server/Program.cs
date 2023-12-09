using LLama.Common;
using LLama;
using Microsoft.AspNetCore.ResponseCompression;
using PalmHill.Llama;
using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using PalmHill.BlazorChat.Server.SignalR;
using Microsoft.AspNetCore.SignalR;
using LLamaSharp.KernelMemory;
using Microsoft.KernelMemory;
using PalmHill.LlmMemory;
using PalmHill.Llama.Models;

var builder = WebApplication.CreateBuilder(args);

// Initlize Llama
//builder.AddLlamaModel();
// End Initlize Llama


// Initiaize Memory
builder.AddLlmMemory();
// End Initiaize Memory


builder.Services.AddControllers().AddJsonOptions(options =>
        //Make Swagger use enums.
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter())
    );

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

//Add signalR custom user id provider.
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();

////Compress websockets traffic.
//builder.Services.AddResponseCompression(opts =>
//{
//    opts.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(
//          new[] { "application/octet-stream" });
//});

//Configure swagger.
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Blazor Chat", Version = "v1" });
    c.UseInlineDefinitionsForEnums();
    c.UseAllOfToExtendReferenceSchemas();
});





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
