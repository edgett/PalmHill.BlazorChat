using LLama.Common;
using LLama;
using Microsoft.AspNetCore.SignalR;
using PalmHill.BlazorChat.Shared;
using PalmHill.BlazorChat.Shared.Models;
using System.Web;
using PalmHill.Llama;
using System.Diagnostics;

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

            var session = LlamaContext.CreateChatSession(chatConversation);
            var inferenceParams = chatConversation.GetInferenceParams();

            var cancelGeneration = new CancellationTokenSource();
            var textBuffer = "";
            var fullResponse = "";
            var totalTokens = 0;
            var inferenceStopwatch = new Stopwatch();

            inferenceStopwatch.Start();
            await foreach (var text in session.ChatAsync(session.History,
                                                        inferenceParams,
                                                        cancelGeneration.Token)
                          )
            {
                
                totalTokens++;
                fullResponse += text;
                textBuffer += text;
                var shouldSendBuffer = ShouldSendBuffer(textBuffer);

                if (shouldSendBuffer)
                {
                    await Clients.Caller.SendAsync("ReceiveModelString", messageId, textBuffer);
                    textBuffer = "";
                }

               
            }
            inferenceStopwatch.Stop();

            if (textBuffer.Length > 0)
            {
                await Clients.Caller.SendAsync("ReceiveModelString", messageId, textBuffer);
            }

            await Clients.Caller.SendAsync("MessageComplete", messageId, "success");
            Console.WriteLine($"Inference took {inferenceStopwatch.ElapsedMilliseconds}ms and generated {totalTokens} tokens. {((float)totalTokens/((float)inferenceStopwatch.ElapsedMilliseconds / (float)1000)).ToString("F2")} tokens/second.");
            Console.WriteLine(fullResponse);
        }


        private bool ShouldSendBuffer(string textBuffer)
        {
          
            if (string.IsNullOrEmpty(textBuffer))
            {
                return false;
            }

            // Check if the last character is a punctuation mark or whitespace
            char lastChar = textBuffer[^1]; // Using ^1 to get the last character
            if (char.IsPunctuation(lastChar) || char.IsWhiteSpace(lastChar))
            {
                return true;
            }

            return false;
        }
    }

}
