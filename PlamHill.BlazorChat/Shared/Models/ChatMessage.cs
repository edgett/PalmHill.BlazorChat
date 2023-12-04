using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalmHill.BlazorChat.Shared.Models
{
    public class ChatMessage
    {
        /// <summary>
        /// Who generated the message.
        /// </summary>
        [DefaultValue(ChatMessageRole.User)]
        public ChatMessageRole? Role { get; set; }

        /// <summary>
        /// The message.
        /// </summary>
        [DefaultValue("What are cats?")]
        public string? Message { get; set; }
    }

    public enum ChatMessageRole
    {
        [Description("Assistant Message")]
        Assistant = 0,
        [Description("User Message")]
        User = 1,
    }
}
