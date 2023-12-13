namespace PalmHill.BlazorChat.Shared.Models.WebSocket
{
    /// <summary>
    /// An inference string sent over a WebSocket.
    /// </summary>
    public class WebSocketInferenceString
    {
        
        public Guid WebSocketChatMessageId { get; set; } = Guid.NewGuid();
        public string InferenceString { get; set; } = string.Empty;
    }
}
