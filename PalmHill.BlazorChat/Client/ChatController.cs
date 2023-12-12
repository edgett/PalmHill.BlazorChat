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
            ILocalStorageService localStorage,
            IDialogService dialogService,
            ThemeControl themeControl
            )
        {
            WebSocketChatConnection = new WebSocketChatConnection(navigationManager.ToAbsoluteUri("/chathub?customUserId=user1"), WebsocketChatMessages);
            setupWebSocketChatConnection();
            _localStorage = localStorage;
            _dialogService = dialogService;
            _themeControl = themeControl;
            _ = WebSocketChatConnection.StartAsync();

        }
        
        public string UserInput { get; set; } = string.Empty;
        public LocalStorageSettings LocalStorageSettings { get; set; } = new LocalStorageSettings();
        public List<WebSocketChatMessage> WebsocketChatMessages { get; set; } = new List<WebSocketChatMessage>();
        private WebSocketChatConnection WebSocketChatConnection { get; }
        public event EventHandler<bool>? OnStateChange;



        private readonly ILocalStorageService _localStorage;
        private readonly IDialogService _dialogService;
        private readonly ThemeControl _themeControl;

        

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
            //Todo: Save settings to local storage
            await _localStorage.SetItemAsync("LocalStorageSettings", LocalStorageSettings);
        }

        public async Task ShowSettings()
        {

            DialogParameters<LocalStorageSettings> parameters = new()
            {

                Title = $"Settings",
                PrimaryAction = "Save",
                PrimaryActionEnabled = true,
                Width = "500px",
                TrapFocus = true,
                Modal = true,
                PreventScroll = true,
            };

            var currentSettings = await GetStorageSettings();

            var dialog = await _dialogService.ShowDialogAsync<ChatSettings>(currentSettings, parameters);
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

        public async Task LoadSettings()
        {
            LocalStorageSettings = await GetStorageSettings();
            await _themeControl.ChangeTheme(LocalStorageSettings.DarkMode);
        }

        private async Task<LocalStorageSettings> GetStorageSettings()
        {
            var settingsExist = await _localStorage.ContainKeyAsync("LocalStorageSettings");

            if (settingsExist)
            {
                var localStorageSettings = await _localStorage.GetItemAsync<LocalStorageSettings>("LocalStorageSettings");

                if (localStorageSettings.SettingsVersion == LocalStorageSettings.CurrentSettingsVersion)
                {
                    return localStorageSettings;
                }
                else
                {
                    //TODO: Migrate settings
                    return new LocalStorageSettings();
                }
            }

            return new LocalStorageSettings();
        }

        private void setupWebSocketChatConnection()
        {
            

            WebSocketChatConnection.OnInferenceStatusUpdate += (sender, inferenceStatusUpdate) =>
            {
                StateHasChanged();
            };

            WebSocketChatConnection.OnAttachmentStatusUpdate += (sender, attachmentInfo) =>
            {
                //TODO: Handle attachment info
            };
        }

        private void StateHasChanged()
        {
            OnStateChange?.Invoke(this, true);
        }
    }
}
