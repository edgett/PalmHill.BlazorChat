﻿using Microsoft.AspNetCore.SignalR.Client;
using PalmHill.BlazorChat.Client.Models;
using PalmHill.BlazorChat.Shared.Models;
using PalmHill.BlazorChat.Shared.Models.WebSocket;
using System.Linq;

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
            Guid conversationId,
            Uri chatHubUri,
            List<WebSocketChatMessage> webSocketChatMessages,
            LocalStorageService localStorageService
            )
        {
            ConversationId = conversationId;
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
        public Guid ConversationId { get; }

        private readonly LocalStorageService _localStorageService;

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
            var lastPrompt = WebSocketChatMessages.SingleOrDefault(s=> s.Id == inferenceRequest.ChatMessages.Last().Id);

            await foreach (var m in HubConnection.StreamAsync<WebSocketInferenceString>("InferenceRequest", inferenceRequest))
            {
                lastPrompt?.AddResponseString(m.InferenceString);
                OnReceiveInferenceString?.Invoke(this, m);
            };

            lastPrompt!.CompleteResponse(true);

            var statusUpdate = new WebSocketInferenceStatusUpdate()
            {
                MessageId = inferenceRequest.ChatMessages.Last().Id,
                IsComplete = true,
                Success = true
            };

            OnInferenceStatusUpdate?.Invoke(this, statusUpdate);
        }

        /// <summary>
        /// Wire up messages handlers.
        /// </summary>
        private void setupHubConnection()
        {
  

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

                if (promptAndResponse.IsComplete && promptAndResponse.Success)
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
