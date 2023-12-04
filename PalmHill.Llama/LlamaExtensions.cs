using LLama.Common;
using LLama;
using PalmHill.BlazorChat.Shared.Models;
using static LLama.Common.ChatHistory;
using PalmHill.BlazorChat.Shared;

namespace PalmHill.Llama
{
    public static class LlamaExtensions
    {
        public static void LoadChatHistory(this ChatSession chatSession, ChatConversation chatConversation)
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


        public static ChatSession CreateChatSession(this LLamaContext lLamaContext, ChatConversation chatConversation)
        {
            var ex = new InteractiveExecutor(lLamaContext);
            ChatSession session = new ChatSession(ex);

            var specialTokensToIgnore = new string[] { "Assistant:", "User:"};
            session = session.WithOutputTransform(new LLamaTransforms.KeywordTextOutputStreamTransform(specialTokensToIgnore, redundancyLength: 8));
            session.LoadChatHistory(chatConversation);
            var promptMessage = chatConversation.ChatMessages.Last();

            if (promptMessage.Role != ChatMessageRole.User)
            {
                throw new ArgumentOutOfRangeException(nameof(chatConversation.ChatMessages), "For inference the last message in the conversation must be a User message.");
            }

            if (string.IsNullOrWhiteSpace(promptMessage.Message))
            {
                throw new ArgumentNullException(nameof(chatConversation.ChatMessages), "No prompt supplied.");
            }

            return session;
        }


        public static InferenceParams GetInferenceParams(this ChatConversation chatConversation)
        {
            var inferenceParams = new InferenceParams() { 
                Temperature = chatConversation.Settings.Temperature,
                MaxTokens = chatConversation.Settings.MaxLength,
                TopP = chatConversation.Settings.TopP,
                FrequencyPenalty = chatConversation.Settings.FrequencyPenalty,
                PresencePenalty = chatConversation.Settings.PresencePenalty,
                AntiPrompts = ["User:"] };
            return inferenceParams;
        }
    }
}
