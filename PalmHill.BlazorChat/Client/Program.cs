using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FluentUI.AspNetCore.Components;
using PalmHill.BlazorChat.ApiClient;
using PalmHill.BlazorChat.Client;
using PalmHill.BlazorChat.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddHttpClient("PlamHill.BlazorChat.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("PlamHill.BlazorChat.ServerAPI"));
builder.Services.AddFluentUIComponents();
builder.Services.AddBlazoredLocalStorage();

//Add BlazorChatApi for operating the API.
builder.Services.AddScoped<BlazorChatApi>();

//Add ThemeControler Service
builder.Services.AddSingleton<ThemeService>();

//Add LocalStorageService
builder.Services.AddScoped<LocalStorageService>();

//Add ChatService
builder.Services.AddScoped<ChatService>();


await builder.Build().RunAsync();
