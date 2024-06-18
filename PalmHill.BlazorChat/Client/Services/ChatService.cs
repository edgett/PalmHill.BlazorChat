using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components;
using PalmHill.BlazorChat.ApiClient;
using PalmHill.BlazorChat.Client.Components.Settings;
using PalmHill.BlazorChat.Client.Models;
using PalmHill.BlazorChat.Shared.Models;
using System.Reflection.Emit;

namespace PalmHill.BlazorChat.Client.Services
{
    /// <summary>
    /// The main service that controls the chat UI. 
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
            BlazorChatApi blazorChatApi,
            ILogger<ChatService> logger
            )
        {

            _localStorageService = localStorage;
            _dialogService = dialogService;
            _blazorChatApi = blazorChatApi;
            _navigationManager = navigationManager;
            _logger = logger;
            setupAttachmentService();
            setupWebSocketChatConnection();
        }


        public Guid ConversationId { get; set; } = Guid.NewGuid();

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
        /// The AttachmentService that handles the attachment upload.
        /// </summary>
        public AttachmentService? AttachmentService { get; private set; }

        /// <summary>
        /// Event handler for when the state changes.
        /// </summary>
        public event EventHandler<bool>? OnStateChange;


        private readonly LocalStorageService _localStorageService;
        private readonly IDialogService _dialogService;
        private readonly BlazorChatApi _blazorChatApi;
        private readonly NavigationManager _navigationManager;
        private readonly ILogger<ChatService> _logger;


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
            if (
                AttachmentService is null
                ||
                !AttachmentService.AttachmentsEnabled
                )
            {
                await SendToWebSocketChat();
            }

            if (AttachmentService?.AttachmentsEnabled == true)
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
            prompt.ConversationId = ConversationId;
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
            prompt.ConversationId = ConversationId;
            WebsocketChatMessages.Add(prompt);
            UserInput = string.Empty;
            StateHasChanged();

            var infrerenceRequest = new InferenceRequest();
            infrerenceRequest.Id = ConversationId;
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
                await _localStorageService.SyncTheme();
            }
            else
            {
                //Save the settings.
                LocalStorageSettings = (LocalStorageSettings?)dialogResult?.Data ?? new LocalStorageSettings();
                await SaveSettings();
            }
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
            var canceled = await _blazorChatApi!.Chat.CancelChat(ConversationId);

            if (canceled.Content)
            {
                SetReady();
            }

            _logger.LogWarning($"Text generation for ConversationId {ConversationId} canceled via API: ({canceled.StatusCode}): {canceled.ReasonPhrase}");
        }

        /// <summary>
        /// Sets up the <see cref="WebSocketChatConnection"/> property.
        /// Configures the event handlers for the <see cref="WebSocketChatConnection"/>.
        /// </summary>
        private void setupWebSocketChatConnection()
        {
            WebSocketChatConnection = new WebSocketChatService(
                    ConversationId,
                    _navigationManager.ToAbsoluteUri("/chathub?customUserId=user1"),
                    WebsocketChatMessages,
                    _localStorageService
                    );

            WebSocketChatConnection.OnInferenceStatusUpdate += (sender, inferenceStatusUpdate) =>
            {
                if (inferenceStatusUpdate.IsComplete)
                {
                    SetReady();
                }
            };

            WebSocketChatConnection.OnAttachmentStatusUpdate += (sender, attachmentInfo) =>
            {
                var attachmentInfoToUpdate = AttachmentService!.UploadedFiles.SingleOrDefault(af => af.Id == attachmentInfo.Id);
                attachmentInfoToUpdate!.Status = attachmentInfo.Status;
                StateHasChanged();
            };
        }

        private void setupAttachmentService()
        {
            AttachmentService = new AttachmentService(
                _blazorChatApi,
                ConversationId
            );


            AttachmentService.OnStateChange += (sender, state) =>
            {
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
