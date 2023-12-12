using Microsoft.AspNetCore.SignalR.Client;
using PalmHill.BlazorChat.Client.Models;
using PalmHill.BlazorChat.Shared.Models;
using PalmHill.BlazorChat.Shared.Models.WebSocket;

namespace PalmHill.BlazorChat.Client.Services
{
    public class WebSocketChatService
    {
        public WebSocketChatService(Uri chatHubUri, List<WebSocketChatMessage> webSocketChatMessages)
        {
            WebSocketChatMessages = webSocketChatMessages;
            HubConnection = new HubConnectionBuilder()
           .WithUrl(chatHubUri)
           .Build();

            setupHubConnection();
        }
        public Guid ConversationId { get; } = Guid.NewGuid();
        public string SystemMessage = string.Empty;
        public List<WebSocketChatMessage> WebSocketChatMessages { get; }
        public HubConnection HubConnection { get; }

        public event EventHandler<WebSocketInferenceString>? OnReceiveInferenceString;
        public event EventHandler<WebSocketInferenceStatusUpdate>? OnInferenceStatusUpdate;
        public event EventHandler<AttachmentInfo>? OnAttachmentStatusUpdate;


        public async Task StartAsync()
        {
            await HubConnection.StartAsync();
        }

        public async Task StopAsync()
        {
            await HubConnection.StopAsync();
        }

        public async Task SendInferenceRequestAsync()
        {
            var inferenceRequest = GetInferenceRequestFromWebsocketMessages();
            await HubConnection.SendAsync("InferenceRequest", inferenceRequest);
        }

        private void setupHubConnection()
        {
            HubConnection.On<WebSocketInferenceString>("ReceiveInferenceString", (inferenceString) =>
            {
                var lastPrompt = WebSocketChatMessages.SingleOrDefault(cm => cm.Id == inferenceString.MessageId);
                lastPrompt?.AddResponseString(inferenceString.InferenceString);
                OnReceiveInferenceString?.Invoke(this, inferenceString);
            });

            HubConnection.On<WebSocketInferenceStatusUpdate>("InferenceStatusUpdate", (statusUpdate) =>
            {
                var lastPrompt = WebSocketChatMessages.Single(cm => cm.Id == statusUpdate.MessageId);

                if (statusUpdate.IsComplete)
                {
                    lastPrompt.CompleteResponse(statusUpdate.Success ?? false);
                }

                OnInferenceStatusUpdate?.Invoke(this, statusUpdate);
            });

            HubConnection.On<AttachmentInfo>("AttachmentStatusUpdate", (attachmentInfo) =>
            {
                OnAttachmentStatusUpdate?.Invoke(this, attachmentInfo);
            });
        }

        private InferenceRequest GetInferenceRequestFromWebsocketMessages()
        {
            var chatConversation = new InferenceRequest();
            chatConversation.Id = ConversationId;
            chatConversation.SystemMessage = SystemMessage;

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
