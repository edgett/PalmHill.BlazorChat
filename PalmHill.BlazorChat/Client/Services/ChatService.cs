using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using PalmHill.BlazorChat.ApiClient;
using PalmHill.BlazorChat.Client.Components.Settings;
using PalmHill.BlazorChat.Client.Models;
using PalmHill.BlazorChat.Shared.Models;
using PalmHill.BlazorChat.Shared.Models.WebSocket;
using System.Data;

namespace PalmHill.BlazorChat.Client.Services
{
    public class ChatService
    {

        public ChatService(
            NavigationManager navigationManager,
            LocalStorageService localStorage,
            IDialogService dialogService,
            ThemeService themeControl,
            BlazorChatApi blazorChatApi
            )
        {
            WebSocketChatConnection = new WebSocketChatService(navigationManager.ToAbsoluteUri("/chathub?customUserId=user1"), WebsocketChatMessages);
            setupWebSocketChatConnection();
            _localStorage = localStorage;
            _dialogService = dialogService;
            _themeControl = themeControl;
            _blazorChatApi = blazorChatApi;
        }

        public string UserInput { get; set; } = string.Empty;
        public bool CanSend { get; set; } = true;
        public bool CanStop { get; set; } = false;
        public bool AttachmentsEnabled { get; set; } = false;
        public bool AttachmentsVisible { get; private set; } = false;
        public List<AttachmentInfo> SelectedFiles = new List<AttachmentInfo>();
        public List<AttachmentInfo> UploadedFiles = new List<AttachmentInfo>();
        public LocalStorageSettings LocalStorageSettings { get; private set; } = new LocalStorageSettings();
        public List<WebSocketChatMessage> WebsocketChatMessages { get; private set; } = new List<WebSocketChatMessage>();
        public WebSocketChatService WebSocketChatConnection { get; }

        public event EventHandler<bool>? OnStateChange;



        private readonly LocalStorageService _localStorage;
        private readonly IDialogService _dialogService;
        private readonly ThemeService _themeControl;
        private readonly BlazorChatApi _blazorChatApi;

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
            if (AttachmentsEnabled == false)
            { 
                await SendToWebSocketChat();
            }

            if (AttachmentsEnabled == true)
            {
                await AskDocumentApi();
            }
        }

        public async Task SendToWebSocketChat()
        {
            //Set the UI state.
            CanSend = false;
            CanStop = true;

            var prompt = new WebSocketChatMessage();
            prompt.Prompt = UserInput;
            WebsocketChatMessages.Add(prompt);
            UserInput = string.Empty;
            StateHasChanged();
            await SendInferenceRequest();
        }

        public async Task AskDocumentApi()
        {

            CanSend = false;
            CanStop = true;

            var prompt = new WebSocketChatMessage();
            prompt.Prompt = UserInput;
            WebsocketChatMessages.Add(prompt);
            UserInput = string.Empty;
            StateHasChanged();

            var infrerenceRequest = new InferenceRequest();
            infrerenceRequest.Id = WebSocketChatConnection.ConversationId;
            var chatMessage = new ChatMessage();
            chatMessage.Id = prompt.Id;
            chatMessage.Message = prompt.Prompt;
            chatMessage.Role = ChatMessageRole.Question;
            
            infrerenceRequest.ChatMessages.Add(chatMessage);

            var apiResponse = await _blazorChatApi!.Chat.Ask(infrerenceRequest);

            if (apiResponse.IsSuccessStatusCode)
            {
                var chatMessageResponse = apiResponse.Content;
                prompt.AddResponseString(chatMessageResponse?.Message ?? "");
                prompt.CompleteResponse(true);
            }
            else
            {
                prompt.CompleteResponse(false);
            }
            SetReady();
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

        public void ToggleAttachments()
        {
            AttachmentsVisible = !AttachmentsVisible;
            StateHasChanged();
        }

        public void ShowAttachments()
        {
            AttachmentsVisible = true;
            StateHasChanged();
        }

        public void HideAttachments()
        {
            AttachmentsVisible = false;
            StateHasChanged();
        }


        public void SetReady()
        {
            CanSend = true;
            CanStop = false;
            StateHasChanged();
        }

        public async Task CancelTextGeneration()
        {
            var canceled = await _blazorChatApi!.Chat.CancelChat(WebSocketChatConnection.ConversationId);

            if (canceled.Content)
            { 
                SetReady();
            }

            Console.WriteLine($"CancelTextGeneration failed ({canceled.StatusCode}): {canceled.ReasonPhrase}");
        }

        private void setupWebSocketChatConnection()
        {
            WebSocketChatConnection.OnInferenceStatusUpdate += (sender, inferenceStatusUpdate) =>
            {
                if (inferenceStatusUpdate.IsComplete == true)
                { 
                    SetReady();
                }
            };

            WebSocketChatConnection.OnAttachmentStatusUpdate += (sender, attachmentInfo) =>
            {
                var attachmentInfoToUpdate = UploadedFiles.SingleOrDefault(af => af.Id == attachmentInfo.Id);
                attachmentInfoToUpdate!.Status = attachmentInfo.Status;
                StateHasChanged();
            };
        }

        private void StateHasChanged()
        {
            OnStateChange?.Invoke(this, true);
        }
    }
}
