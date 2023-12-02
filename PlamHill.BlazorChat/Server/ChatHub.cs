using LLama.Common;
using LLama;
using Microsoft.AspNetCore.SignalR;
using PalmHill.BlazorChat.Shared;
using PalmHill.BlazorChat.Shared.Models;
using System.Web;

namespace PlamHill.BlazorChat.Server
{
    public class ChatHub : Hub
    {

        LLamaContext LlamaContext;
        public ChatHub(LLamaContext llamaContext) 
        {
          LlamaContext = llamaContext;
            
        }

        public async Task SendPrompt(Guid messageId, ChatConversation chatConversation)
        {
            var context = LlamaContext;
            var ex = new InteractiveExecutor(context);
            ChatSession session = new ChatSession(ex);
            
            var rawPrompt = chatConversation.ToLlamaPromptString();
            Console.WriteLine(rawPrompt);

            var cancelGeneration = new CancellationTokenSource();
            var textBuffer = "";
            var fullResponse = "";
            // run the inference in a loop to chat with LLM
            await foreach (var text in session.ChatAsync(rawPrompt,
                                                        new InferenceParams() { Temperature = 0.6f, AntiPrompts = new List<string> { ChatExtensions.MESSAGE_END } },
                                                        cancelGeneration.Token)
                                                        )
            {
                fullResponse += text;
                textBuffer += text;
                var shouldSendBuffer = ShouldSendBuffer(textBuffer);


                if (shouldSendBuffer)
                {
                    var shouldCancelGeneration = ShouldCancelGeneration(textBuffer);
                    var textToSend = PreProcessResponse(textBuffer, shouldCancelGeneration);
                    await Clients.Caller.SendAsync("ReceiveModelString", messageId, textToSend);
                    textBuffer = "";

                    if (shouldCancelGeneration)
                    {
                        cancelGeneration.Cancel();
                        break;
                    }
                }
            }

            await Clients.Caller.SendAsync("MessageComplete", messageId, "success");

            Console.WriteLine(fullResponse);
        }

        public string PreProcessResponse(string response, bool removeMessageStart = false)
        {
            var preProcessedResponse = response.Replace(ChatExtensions.MESSAGE_END, "");
            if (removeMessageStart)
            {
                var messageStartIndex = preProcessedResponse.IndexOf(ChatExtensions.MESSAGE_START);
                preProcessedResponse = preProcessedResponse.Substring(messageStartIndex, response.Length - messageStartIndex);
            }


            return preProcessedResponse;
        }

        private bool ShouldCancelGeneration(string textBuffer)
        {
            if (textBuffer.Contains(ChatExtensions.MESSAGE_START))
            {
                return true;
            }

            return false;
        }

        private bool ShouldSendBuffer(string textBuffer)
        {
            // Check if the end of the text is ChatExtensions.MESSAGE_END
            if (textBuffer.EndsWith(ChatExtensions.MESSAGE_END))
            {
                return true;
            }


            if (string.IsNullOrEmpty(textBuffer))
            {
                return false;
            }

            // Check if the last character is a punctuation mark or whitespace
            char lastChar = textBuffer[^1]; // Using ^1 to get the last character
            if (char.IsPunctuation(lastChar) || char.IsWhiteSpace(lastChar))
            {
                if (textBuffer.Contains("<")) 
                {
                    return false;
                }

                return true;
            }

            return false;
        }
    }

}
