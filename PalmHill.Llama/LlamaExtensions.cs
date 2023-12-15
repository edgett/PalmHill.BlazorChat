using LLama;
using LLama.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PalmHill.BlazorChat.Shared.Models;
using PalmHill.Llama.Models;

namespace PalmHill.Llama
{
    /// <summary>
    /// Provides extension methods for the <see cref="LLamaContext"/> class.
    /// </summary>
    public static class LlamaExtensions
    {
        /// <summary>
        /// Loads the chat history into a <see cref="ChatSession"/>.
        /// </summary>
        /// <param name="chatSession">The <see cref="ChatSession"/> to load the history into.</param>
        /// <param name="chatConversation">The <see cref="InferenceRequest"/> containing the chat history.</param>
        public static void LoadChatHistory(this ChatSession chatSession, InferenceRequest chatConversation)
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

        /// <summary>
        /// Creates a new <see cref="ChatSession"/> from a <see cref="InferenceRequest"/>.
        /// </summary>
        /// <param name="lLamaContext">The <see cref="LLamaContext"/> to use for the session.</param>
        /// <param name="chatConversation">The <see cref="InferenceRequest"/> to create the session from.</param>
        /// <returns>A new <see cref="ChatSession"/>.</returns>
        public static ChatSession CreateChatSession(this LLamaContext lLamaContext, InferenceRequest chatConversation)
        {
            var ex = new InteractiveExecutor(lLamaContext);
            ChatSession session = new ChatSession(ex);

            var specialTokensToIgnore = new string[] { "Assistant:", "User:" };
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

        /// <summary>
        /// Gets the inference parameters from a <see cref="InferenceRequest"/>.
        /// </summary>
        /// <param name="chatConversation">The <see cref="InferenceRequest"/> to get the parameters from.</param>
        /// <returns>The <see cref="InferenceParams"/> for the conversation.</returns>
        public static InferenceParams GetInferenceParams(this InferenceRequest chatConversation, List<string>? defaultAntiPrompts = null)
        {
            var inferenceParams = new InferenceParams()
            {
                Temperature = chatConversation.Settings.Temperature,
                MaxTokens = chatConversation.Settings.MaxLength,
                TopP = chatConversation.Settings.TopP,
                FrequencyPenalty = chatConversation.Settings.FrequencyPenalty,
                PresencePenalty = chatConversation.Settings.PresencePenalty,
                AntiPrompts = defaultAntiPrompts ?? []
            };
            return inferenceParams;
        }


       
    }
}
