using Microsoft.AspNetCore.SignalR;
using Microsoft.OpenApi.Models;
using PalmHill.BlazorChat.Server.SignalR;
using PalmHill.Llama;
using PalmHill.LlmMemory;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Initlize Llama
builder.Services.AddLlamaModelProvider();
// End Initlize Llama

var llmMemoryConfig = builder.Configuration.GetModelConfigFromConfigSection("EmbeddingModelConfig");
// Initiaize Memory
builder.Services.AddLlmMemory(llmMemoryConfig);
// End Initiaize Memory


builder.Services.AddControllers().AddJsonOptions(options =>
{
    //Json convert for encoding in ModelParams.
    options.JsonSerializerOptions.Converters.Add(new EncodingConverter());
    //Make Swagger use enums.
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

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
