using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalmHill.Llama.Models
{
    public class ChatConversation
    {
        public string? SystemMessage { get; set; }
        public List<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
    }
}
