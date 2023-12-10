using PalmHill.BlazorChat.Client.Components.Chat;
using PalmHill.BlazorChat.Client.Components.Attachment;

namespace PalmHill.BlazorChat.Client.Models
{
    public class ChatComponents
    {
        public Chat? ChatRenderer { get; set; }
        public ChatInput? ChatInput { get; set; }
        public AttachmentManager? AttachmentManager { get; set; }

    }
}
