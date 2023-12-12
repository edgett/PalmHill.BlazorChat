using Microsoft.AspNetCore.SignalR.Client;
using PalmHill.BlazorChat.Shared.Models;
using PalmHill.BlazorChat.Shared.Models.WebSocket;

namespace PalmHill.BlazorChat.Client
{
    public class WebSocketChatConnection
    {
        public WebSocketChatConnection(Uri chatHubUri)
        {
            HubConnection = new HubConnectionBuilder()
           .WithUrl(chatHubUri)
           .Build();

            setupHubConnection();
        }

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

        public async Task SendInferenceRequestAsync(InferenceRequest inferenceRequest)
        {
            await HubConnection.SendAsync("InferenceRequest", inferenceRequest);
        }

        private void setupHubConnection()
        {
            HubConnection.On<WebSocketInferenceString>("ReceiveInferenceString", (inferenceString) =>
            {
                OnReceiveInferenceString?.Invoke(this, inferenceString);
            });

            HubConnection.On<WebSocketInferenceStatusUpdate>("InferenceStatusUpdate", (statusUpdate) =>
            {
                OnInferenceStatusUpdate?.Invoke(this, statusUpdate);
            });

            HubConnection.On<AttachmentInfo>("AttachmentStatusUpdate", (attachmentInfo) =>
            {
                OnAttachmentStatusUpdate?.Invoke(this, attachmentInfo);
            });
        }
    }
}
