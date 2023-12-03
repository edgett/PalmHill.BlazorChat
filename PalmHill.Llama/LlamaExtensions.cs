using LLama.Common;
using LLama;
using PalmHill.BlazorChat.Shared.Models;
using static LLama.Common.ChatHistory;

namespace PalmHill.Llama
{
    public static class LlamaExtensions
    {
        public static void LoadChatHistory(this ChatSession chatSession, ChatConversation chatConversation, bool lastMessageContainsPrompt = true)
        { 

            if (!string.IsNullOrWhiteSpace(chatConversation.SystemMessage))
            { 
                chatSession.History.AddMessage(AuthorRole.System, chatConversation.SystemMessage);
            }

            var lastMessage = chatConversation.ChatMessages.LastOrDefault();
            var messageIndex = 0;

            foreach (var chatMessage in chatConversation.ChatMessages)
            {
                if (string.IsNullOrWhiteSpace(chatMessage.Message))
                {
                    messageIndex++;
                    continue;
                }

                if (lastMessageContainsPrompt && lastMessage == chatMessage)
                {
                    return;
                }

                switch (chatMessage.Role)
                {
                    case ChatMessageRole.User:
                        chatSession.History.AddMessage(AuthorRole.User, chatMessage.Message);
                        break;
                    case ChatMessageRole.Assistant:
                        chatSession.History.AddMessage(AuthorRole.Assistant, chatMessage.Message);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException($"Chat conversation is malformed at {nameof(ChatMessage)}[{messageIndex}]: The {nameof(chatMessage.Role)} must be {nameof(ChatMessageRole.Assistant)} or {nameof(ChatMessageRole.User)}");
                }

                messageIndex++;
            }
        }
    }
}
