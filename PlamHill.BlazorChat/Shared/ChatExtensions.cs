using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using PalmHill.BlazorChat.Shared.Models;

namespace PalmHill.BlazorChat.Shared
{
    public static class ChatExtensions
    {
        // Constants for message start and end tags
        public const string MESSAGE_START = "<|im_start|>";
        public const string MESSAGE_END = "<|im_end|>";

        public const string SYSTEM_MESSAGE_START = MESSAGE_START + "system" + "\n";
        public const string USER_MESSAGE_START = MESSAGE_START + "user" + "\n";
        public const string ASSISTANT_MESSAGE_START = MESSAGE_START + "assistant";

        /// <summary>
        /// Converts a ChatConversation object into a string format suitable for Orca.
        /// </summary>
        /// <param name="chatConversation">The ChatConversation object to be converted.</param>
        /// <returns>A string representation of the ChatConversation object in a format suitable for Orca.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the role of a ChatMessage is neither User nor Assistant.</exception>
        public static string ToOrcaPromptString(this ChatConversation chatConversation)
        {
            var promptString = new StringBuilder();

            // Check if there is a system message in the conversation
            if (!string.IsNullOrWhiteSpace(chatConversation.SystemMessage))
            {
                // Append system message start tag
                promptString.Append(SYSTEM_MESSAGE_START);
                // Append the system message
                promptString.Append(chatConversation.SystemMessage);
                // Append system message end tag
                promptString.Append(MESSAGE_END);
                // Append a newline character
                promptString.Append("\n");
            }

            var messageIndex = 0;

            // Iterate over each chat message in the conversation
            foreach (var chatMessage in chatConversation.ChatMessages)
            {
                var messageStartString = "";
                // Determine the role of the chat message
                switch (chatMessage.Role)
                {
                    case ChatMessageRole.User:
                        // Set start tag for user message
                        messageStartString = USER_MESSAGE_START;
                        break;
                    case ChatMessageRole.Assistant:
                        // Set start tag for assistant message
                        messageStartString = ASSISTANT_MESSAGE_START;
                        break;
                    default:
                        // Throw an exception if the role is neither User nor Assistant
                        throw new ArgumentOutOfRangeException($"Chat conversation is malformed at {nameof(ChatMessage)}[{messageIndex}]: The {nameof(chatMessage.Role)} must be {nameof(ChatMessageRole.Assistant)} or {nameof(ChatMessageRole.User)}");
                }

                // Append the start tag for the message
                promptString.Append(messageStartString);
                // Append the message
                promptString.Append(chatMessage.Message);
                // Append the end tag for the message
                promptString.Append(MESSAGE_END);
                // Append a newline character
                promptString.Append("\n");

                messageIndex++;
            }

            // Append the start tag for the assistant message
            promptString.Append(ASSISTANT_MESSAGE_START);

            var prompt = promptString.ToString();
            return prompt;
        }


    }
}
