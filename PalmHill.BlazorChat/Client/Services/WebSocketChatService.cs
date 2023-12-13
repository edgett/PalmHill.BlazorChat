using Microsoft.AspNetCore.SignalR.Client;
using PalmHill.BlazorChat.Client.Models;
using PalmHill.BlazorChat.Shared.Models;
using PalmHill.BlazorChat.Shared.Models.WebSocket;

namespace PalmHill.BlazorChat.Client.Services
{
    /// <summary>
    /// Controlls the WebSocket traffic.
    /// </summary>
    public class WebSocketChatService
    {
        /// <summary>
        /// Setup the WebSocketChatService
        /// </summary>
        /// <param name="chatHubUri"></param>
        /// <param name="webSocketChatMessages">The list of <see cref="WebSocketChatMessage"/> to be interacted with.</param>
        public WebSocketChatService(
            Uri chatHubUri,
            List<WebSocketChatMessage> webSocketChatMessages,
            LocalStorageService localStorageService
            )
        {
            _localStorageService = localStorageService;
            WebSocketChatMessages = webSocketChatMessages;
            HubConnection = new HubConnectionBuilder()
           .WithUrl(chatHubUri)
           .Build();

            setupHubConnection();
        }
        /// <summary>
        /// Used for DB correlation. (Later)
        /// </summary>
        public Guid ConversationId { get; } = Guid.NewGuid();

        private LocalStorageService _localStorageService;

        /// <summary>
        /// The list of <see cref="WebSocketChatMessage"/> to be interacted with.
        /// </summary>
        public List<WebSocketChatMessage> WebSocketChatMessages { get; }

        /// <summary>
        /// The SignalR HubConnection.
        /// </summary>
        public HubConnection HubConnection { get; }

        /// <summary>
        /// Triggered when a <see cref="WebSocketInferenceString"/> is received.
        /// </summary>
        public event EventHandler<WebSocketInferenceString>? OnReceiveInferenceString;

        /// <summary>
        /// Triggered when a <see cref="WebSocketInferenceStatusUpdate"/> is received.
        /// </summary>
        public event EventHandler<WebSocketInferenceStatusUpdate>? OnInferenceStatusUpdate;
        /// <summary>
        /// Triggered when a <see cref="AttachmentInfo"/> is received.
        /// </summary>
        public event EventHandler<AttachmentInfo>? OnAttachmentStatusUpdate;

        /// <summary>
        /// Start the WebSocket connection.
        /// </summary>
        public async Task StartAsync()
        {
            await HubConnection.StartAsync();
        }  

        /// <summary>
        /// Stop the WebSocket connection.
        /// </summary>
        public async Task StopAsync()
        {
            await HubConnection.StopAsync();
        }

        /// <summary>
        /// Send the <see cref="InferenceRequest"/> to the server.
        /// </summary>
        /// <returns></returns>
        public async Task SendInferenceRequestAsync()
        {
            var inferenceRequest = await GetInferenceRequestFromWebsocketMessages();
            await HubConnection.SendAsync("InferenceRequest", inferenceRequest);
        }

        /// <summary>
        /// Wire up messages handlers.
        /// </summary>
        private void setupHubConnection()
        {
            //Whena an InferenceString message is received, add it to the last prompt.
            HubConnection.On<WebSocketInferenceString>("ReceiveInferenceString", (inferenceString) =>
            {
                var lastPrompt = WebSocketChatMessages.SingleOrDefault(cm => cm.Id == inferenceString.WebSocketChatMessageId);
                lastPrompt?.AddResponseString(inferenceString.InferenceString);
                OnReceiveInferenceString?.Invoke(this, inferenceString);
            });

            //When an InferenceStatusUpdate message is received, update the last prompt.
            HubConnection.On<WebSocketInferenceStatusUpdate>("InferenceStatusUpdate", (statusUpdate) =>
            {
                var lastPrompt = WebSocketChatMessages.Single(cm => cm.Id == statusUpdate.MessageId);

                if (statusUpdate.IsComplete)
                {
                    lastPrompt.CompleteResponse(statusUpdate.Success ?? false);
                }

                OnInferenceStatusUpdate?.Invoke(this, statusUpdate);
            });

            ///When an AttachmentStatusUpdate message is received, update the attachment status.
            HubConnection.On<AttachmentInfo>("AttachmentStatusUpdate", (attachmentInfo) =>
            {
                OnAttachmentStatusUpdate?.Invoke(this, attachmentInfo);
            });
        }

        /// <summary>
        /// Combine the <see cref="WebSocketChatMessages"/> into a <see cref="InferenceRequest"/>.
        /// </summary>
        /// <returns>
        /// A new <see cref="InferenceRequest"/> with the <see cref="WebSocketChatMessages"/> combined. And <see cref="InferenceRequest.Settings"/> and <see cref="InferenceRequest.SystemMessage"/> set from <see cref="LocalStorageSettings"/>.
        /// </returns>
        private async Task<InferenceRequest> GetInferenceRequestFromWebsocketMessages()
        {
            var chatConversation = new InferenceRequest();
            var localStorageSettings = await _localStorageService.GetSettings();

            chatConversation.Settings = localStorageSettings.InferenceSettings;
            chatConversation.Id = ConversationId;
            chatConversation.SystemMessage = localStorageSettings.SystemMessage;

            foreach (var promptAndResponse in WebSocketChatMessages)
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
    }
}
