using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using PalmHill.BlazorChat.Client.Components.Settings;
using PalmHill.BlazorChat.Client.Models;
using PalmHill.BlazorChat.Shared.Models;
using PalmHill.BlazorChat.Shared.Models.WebSocket;
using System.Data;

namespace PalmHill.BlazorChat.Client
{
    public class ChatController
    {

        public ChatController(
            NavigationManager navigationManager,
            LocalStorageService localStorage,
            IDialogService dialogService,
            ThemeControl themeControl
            )
        {
            WebSocketChatConnection = new WebSocketChatService(navigationManager.ToAbsoluteUri("/chathub?customUserId=user1"), WebsocketChatMessages);
            setupWebSocketChatConnection();
            _localStorage = localStorage;
            _dialogService = dialogService;
            _themeControl = themeControl;
        }
        
        public string UserInput { get; set; } = string.Empty;
        public LocalStorageSettings LocalStorageSettings { get; set; } = new LocalStorageSettings();
        public List<WebSocketChatMessage> WebsocketChatMessages { get; set; } = new List<WebSocketChatMessage>();
        private WebSocketChatService WebSocketChatConnection { get; }
        public event EventHandler<bool>? OnStateChange;



        private readonly LocalStorageService _localStorage;
        private readonly IDialogService _dialogService;
        private readonly ThemeControl _themeControl;

        public async Task StartChat()
        {
           LocalStorageSettings = await _localStorage.GetSettings();
           await WebSocketChatConnection.StartAsync();
        }

        private async Task SendInferenceRequest()
        {
            await WebSocketChatConnection!.SendInferenceRequestAsync();
        }

        public async Task SendPrompt()
        {
            var prompt = new WebSocketChatMessage();
            prompt.Prompt = UserInput;
            WebsocketChatMessages.Add(prompt);
            UserInput = string.Empty;
            StateHasChanged();
            await SendInferenceRequest();
        }

        public async Task SaveSettings()
        {
            await _localStorage.SaveSettings(LocalStorageSettings);
        }

        public async Task ShowSettings()
        {


            var currentSettings = LocalStorageSettings.CreateCopy();
            var dialogParameters = ChatSettings.DefaultDialogParameters;
            var dialog = await _dialogService.ShowDialogAsync<ChatSettings>(currentSettings, dialogParameters);
            var dialogResult = await dialog.Result;


            if (dialogResult?.Cancelled == true)
            {
                //Reset the theme if cancel.
                await _themeControl.ChangeTheme(LocalStorageSettings.DarkMode);
            }
            else
            {
                //Save the settings.
                LocalStorageSettings = (LocalStorageSettings?)dialogResult?.Data ?? new LocalStorageSettings();
                await SaveSettings();
            }

        }

  
       

        private void setupWebSocketChatConnection()
        {
            WebSocketChatConnection.OnInferenceStatusUpdate += (sender, inferenceStatusUpdate) =>
            {
                StateHasChanged();
            };
        }

        private void StateHasChanged()
        {
            OnStateChange?.Invoke(this, true);
        }
    }
}
