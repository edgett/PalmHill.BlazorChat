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
            WebSocketChatConnection = new WebSocketChatConnection(navigationManager.ToAbsoluteUri("/chathub?customUserId=user1"));
            setupWebSocketChatConnection();
            _localStorage = localStorage;
            _dialogService = dialogService;
            _themeControl = themeControl;
            _ = WebSocketChatConnection.StartAsync();

        }
        public Guid ConversationId { get; } = Guid.NewGuid();
        public string UserInput { get; set; } = string.Empty;
        public LocalStorageSettings LocalStorageSettings { get; set; } = new LocalStorageSettings();
        public List<WebSocketChatMessage> WebsocketChatMessages { get; set; } = new List<WebSocketChatMessage>();
        private InferenceRequest InferenceRequest { get => GetInferenceRequestFromWebsocketMessages(); }
        private WebSocketChatConnection WebSocketChatConnection { get; }
        public event EventHandler<bool>? OnStateChange;



        private readonly ILocalStorageService _localStorage;
        private readonly IDialogService _dialogService;
        private readonly ThemeControl _themeControl;

        private InferenceRequest GetInferenceRequestFromWebsocketMessages()
        {
            var chatConversation = new InferenceRequest();
            chatConversation.Id = ConversationId;
            chatConversation.SystemMessage = LocalStorageSettings.SystemMessage;

            foreach (var promptAndResponse in WebsocketChatMessages)
            {
                var userMessage = new ChatMessage();
                userMessage.Message = promptAndResponse.Prompt;
                userMessage.Id = promptAndResponse.Id;
                userMessage.Role = ChatMessageRole.User;
                chatConversation.ChatMessages.Add(userMessage);

                if (promptAndResponse.IsComplete && promptAndResponse.Success == true)
                {
                    var modelMessage = new ChatMessage();
                    modelMessage.Message = promptAndResponse.Resonse;
                    modelMessage.Role = ChatMessageRole.Assistant;
                    chatConversation.ChatMessages.Add(modelMessage);

                }

            }

            return chatConversation;
        }

        private async Task SendInferenceRequest()
        {
            await WebSocketChatConnection!.SendInferenceRequestAsync(InferenceRequest);
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
            WebSocketChatConnection.OnReceiveInferenceString += (sender, inferenceString) =>
            {
                var lastPrompt = WebsocketChatMessages.SingleOrDefault(cm => cm.Id == inferenceString.MessageId);
                lastPrompt?.AddResponseString(inferenceString.InferenceString);

            };

            WebSocketChatConnection.OnInferenceStatusUpdate += (sender, inferenceStatusUpdate) =>
            {
                var lastPrompt = WebsocketChatMessages.Single(cm => cm.Id == inferenceStatusUpdate.MessageId);

                if (inferenceStatusUpdate.IsComplete)
                {
                    lastPrompt.CompleteResponse(inferenceStatusUpdate.Success ?? false);
                }

                OnStateChange?.Invoke(this, true);
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
