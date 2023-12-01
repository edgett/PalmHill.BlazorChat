using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalmHill.BlazorChat.Shared.Models
{
    public class ChatMessage
    {
        public ChatMessageRole? Role { get; set; }
        public string? Message { get; set; }
    }

    public enum ChatMessageRole
    {
        Assistant = 0,
        User = 1,
    }
}
