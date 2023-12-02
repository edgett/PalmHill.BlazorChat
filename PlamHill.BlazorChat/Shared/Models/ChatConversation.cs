using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalmHill.BlazorChat.Shared.Models
{
    public class ChatConversation
    {
        public string? SystemMessage { get; set; } = "You are a helpful assistant. Repsond in valid markdown only.";
        public List<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
    }
}
