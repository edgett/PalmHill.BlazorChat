using PalmHill.BlazorChat.Client.Components;

namespace PalmHill.BlazorChat.Client
{
    public class ChatComponents
    {
        public Chat? ChatRenderer { get; set; }
        public ChatInput? ChatInput { get; set; }
        public AttachmentManager? AttachmentManager { get; set; }

    }
}
