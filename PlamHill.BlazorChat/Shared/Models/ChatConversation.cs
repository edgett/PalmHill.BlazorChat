using PlamHill.BlazorChat.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PalmHill.BlazorChat.Shared.Models
{
    /// <summary>
    /// Represents a chat conversation.
    /// </summary>
    public class ChatConversation
    {
        /// <summary>
        /// Gets or sets the system message for the chat conversation.
        /// </summary>
        /// <value>
        /// The system message for the chat conversation.
        /// </value>
        [DefaultValue("You are a helpful assistant.")]
        public string SystemMessage { get; set; } = "You are a helpful assistant.";

        /// <summary>
        /// Gets or sets the chat messages for the chat conversation.
        /// </summary>
        /// <value>
        /// The chat messages for the chat conversation.
        /// </value>
        public List<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

        /// <summary>
        /// Gets or sets the inference settings for the chat conversation.
        /// </summary>
        /// <value>
        /// The inference settings for the chat conversation.
        /// </value>
        public InferenceSettings Settings { get; set; } = new InferenceSettings();
    }
}