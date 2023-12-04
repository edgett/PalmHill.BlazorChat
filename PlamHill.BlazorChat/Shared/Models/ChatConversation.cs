using PlamHill.BlazorChat.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalmHill.BlazorChat.Shared.Models
{
    public class ChatConversation
    {
        /// <summary>
        /// Controls the persona of the model.
        /// </summary>
        /// <example>You are a helpful assistant. Repsond in valid markdown only.</example>
        [DefaultValue("You are a helpful assistant. Repsond in valid markdown only.")]
        public string SystemMessage { get; set; } = "You are a helpful assistant. Repsond in valid markdown only.";
        public List<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
        public InferenceSettings Settings { get; set; } = new InferenceSettings();

    }
}
