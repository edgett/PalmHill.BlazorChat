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
    /// <summary>
    /// The the main service that controls the chat UI. 
    /// </summary>
    public class ChatService
    {

        /// <summary>
        /// Main constructor. Uses dependency injection to get the required services.
        /// </summary>
        /// <param name="navigationManager"></param>
        /// <param name="localStorage"></param>
        /// <param name="dialogService"></param>
        /// <param name="themeControl"></param>
        /// <param name="blazorChatApi"></param>
        public ChatService(
            NavigationManager navigationManager,
            LocalStorageService localStorage,
            IDialogService dialogService,
            ThemeService themeControl,
            BlazorChatApi blazorChatApi
            )
        {
            
            _localStorageService = localStorage;
            _dialogService = dialogService;
            _themeControl = themeControl;
            _blazorChatApi = blazorChatApi;
            _navigationManager = navigationManager;
            setupWebSocketChatConnection();
        }

        /// <summary>
        /// User input from the chat box.
        /// </summary>
        public string UserInput { get; set; } = string.Empty;
        /// <summary>
        /// Whether the chat is ready to send a message.
        /// </summary>
        public bool CanSend { get; set; } = true;
        /// <summary>
        /// Whether the chat is ready to stop.
        /// </summary>
        public bool CanStop { get; set; } = false;
        /// <summary>
        /// Whether the chat is in Attachment mode. Will only reference attached douments.
        /// </summary>
        public bool AttachmentsEnabled { get; set; } = false;
        /// <summary>
        /// Show the attachment panel.
        /// </summary>
        public bool AttachmentsVisible { get; private set; } = false;
        /// <summary>
        /// The list of files that have been selected for Chat. This is non-functional for now.
        /// </summary>
        public List<AttachmentInfo> SelectedFiles = new List<AttachmentInfo>();

        /// <summary>
        /// The list of files that have been uploaded for Chat.
        /// </summary>
        public List<AttachmentInfo> UploadedFiles = new List<AttachmentInfo>();

        /// <summary>
        /// The local storage settings.
        /// </summary>
        public LocalStorageSettings LocalStorageSettings { get; private set; } = new LocalStorageSettings();
        /// <summary>
        /// The list of chat messages. Containing a prompt and its response.
        /// </summary>
        public List<WebSocketChatMessage> WebsocketChatMessages { get; private set; } = new List<WebSocketChatMessage>();

        /// <summary>
        /// The WebSocketChatService that handles the WebSocket connection.
        /// </summary>
        public WebSocketChatService? WebSocketChatConnection { get; private set; }

        /// <summary>
        /// Event handler for when the state changes.
        /// </summary>
        public event EventHandler<bool>? OnStateChange;



        private readonly LocalStorageService _localStorageService;
        private readonly IDialogService _dialogService;
        private readonly ThemeService _themeControl;
        private readonly BlazorChatApi _blazorChatApi;
        private readonly NavigationManager _navigationManager;


        /// <summary>
        /// Starts the chat over the WebSocket connection.
        /// </summary>
        /// <returns></returns>
        public async Task StartChat()
        {
            LocalStorageSettings = await _localStorageService.GetSettings();
            await WebSocketChatConnection!.StartAsync();
        }

        /// <summary>
        /// Stops the chat over the WebSocket connection.
        /// </summary>
        /// <returns></returns>
        private async Task SendInferenceRequest()
        {
            await WebSocketChatConnection!.SendInferenceRequestAsync();
        }

        /// <summary>
        /// Sends the prompt to the WebSocketChatConnection or the Document API if in <see cref="AttachmentsEnabled" /> is true.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Sends the prompt to the WebSocketChatConnection.
        /// </summary>
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

        /// <summary>
        /// Sends the prompt to the Document API.
        /// </summary>
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
            infrerenceRequest.Id = WebSocketChatConnection!.ConversationId;
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

        /// <summary>
        /// Saves the settings to local storage.
        /// </summary>
        /// <returns></returns>
        public async Task SaveSettings()
        {
            await _localStorageService.SaveSettings(LocalStorageSettings);
        }

        /// <summary>
        /// Shows the <see cref="ChatSettings"/> dialog.
        /// </summary>
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

        /// <summary>
        /// Toggles the <see cref="AttachmentsVisible"/> property.
        /// This does not toggle the <see cref="AttachmentsEnabled"/> property.
        /// </summary>
        public void ToggleAttachmentsVisible()
        {
            AttachmentsVisible = !AttachmentsVisible;
            StateHasChanged();
        }

        /// <summary>
        /// Shows the <see cref="Client.Components.Attachment.AttachmentManager"/>.
        /// </summary>
        public void ShowAttachments()
        {
            AttachmentsVisible = true;
            StateHasChanged();
        }

        /// <summary>
        /// Hides the <see cref="Client.Components.Attachment.AttachmentManager"/>.
        /// </summary>
        public void HideAttachments()
        {
            AttachmentsVisible = false;
            StateHasChanged();
        }

        /// <summary>
        /// Chat is ready to send a message.
        /// Sets the <see cref="CanSend"/> property true.
        /// Sets the <see cref="CanStop"/> property false.
        /// </summary>
        public void SetReady()
        {
            CanSend = true;
            CanStop = false;
            StateHasChanged();
        }

        /// <summary>
        /// Cancel the text generation via http request. Effectively stops the chat.
        /// </summary>
        public async Task CancelTextGeneration()
        {
            var canceled = await _blazorChatApi!.Chat.CancelChat(WebSocketChatConnection!.ConversationId);

            if (canceled.Content)
            { 
                SetReady();
            }

            Console.WriteLine($"CancelTextGeneration failed ({canceled.StatusCode}): {canceled.ReasonPhrase}");
        }

        /// <summary>
        /// Sets up the <see cref="WebSocketChatConnection"/> property.
        /// Configures the event handlers for the <see cref="WebSocketChatConnection"/>.
        /// </summary>
        private void setupWebSocketChatConnection()
        {
            WebSocketChatConnection = new WebSocketChatService(
                    _navigationManager.ToAbsoluteUri("/chathub?customUserId=user1"), 
                    WebsocketChatMessages,
                    _localStorageService
                    );

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

        /// <summary>
        /// Invokes the <see cref="OnStateChange"/> event.
        /// </summary>
        private void StateHasChanged()
        {
            OnStateChange?.Invoke(this, true);
        }
    }
}
