namespace PalmHill.BlazorChat.Shared.Models.WebSocket
{
    public class WebSocketInferenceStatusUpdate
    {
        public Guid? MessageId { get; set; }
        public bool IsComplete { get; set; } = false;
        public bool? Success { get; set; }
    }
}
