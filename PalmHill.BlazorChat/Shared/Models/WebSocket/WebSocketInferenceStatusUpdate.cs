namespace PalmHill.BlazorChat.Shared.Models.WebSocket
{
    /// <summary>
    /// A status update sent over a WebSocket.
    /// </summary>
    public class WebSocketInferenceStatusUpdate
    {
        public Guid? MessageId { get; set; }
        public bool IsComplete { get; set; } = false;
        public bool? Success { get; set; }
    }
}
