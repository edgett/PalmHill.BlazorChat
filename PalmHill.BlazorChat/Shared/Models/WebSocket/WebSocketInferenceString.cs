namespace PalmHill.BlazorChat.Shared.Models.WebSocket
{
    public class WebSocketInferenceString
    {
        public Guid MessageId { get; set; } = Guid.NewGuid();
        public string InferenceString { get; set; } = string.Empty;
    }
}
