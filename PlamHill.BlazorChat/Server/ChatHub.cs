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
        LLamaWeights LLamaWeights;
        ModelParams ModelParams;
        public ChatHub(LLamaWeights model, ModelParams modelParams)
        {
            LLamaWeights = model;
            ModelParams = modelParams;
        }



        public async Task SendPrompt(Guid messageId, ChatConversation chatConversation)
        {
            await ThreadLock.InferenceLock.WaitAsync();

            try
            {
                await DoInfrenceAndRespondToClient(Clients.Caller, messageId, chatConversation);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                ThreadLock.InferenceLock.Release();
            }
        }

        private async Task DoInfrenceAndRespondToClient(ISingleClientProxy respondToClient, Guid messageId, ChatConversation chatConversation)
        {


            LLamaContext modelContext = LLamaWeights.CreateContext(ModelParams);
            var session = modelContext.CreateChatSession(chatConversation);
            var inferenceParams = chatConversation.GetInferenceParams();

            var cancelGeneration = new CancellationTokenSource();
            var textBuffer = "";
            var fullResponse = "";
            var totalTokens = 0;
            var inferenceStopwatch = new Stopwatch();

            inferenceStopwatch.Start();
            var asyncResponse = session.ChatAsync(session.History,
                                                        inferenceParams,
                                                        cancelGeneration.Token);
            await foreach (var text in asyncResponse)
            {

                totalTokens++;
                fullResponse += text;
                textBuffer += text;
                var shouldSendBuffer = ShouldSendBuffer(textBuffer);

                if (shouldSendBuffer)
                {
                    await respondToClient.SendAsync("ReceiveModelString", messageId, textBuffer);
                    textBuffer = "";
                }


            }
            modelContext.Dispose();

            inferenceStopwatch.Stop();

            if (textBuffer.Length > 0)
            {
                await respondToClient.SendAsync("ReceiveModelString", messageId, textBuffer);
            }

            await respondToClient.SendAsync("MessageComplete", messageId, "success");
            Console.WriteLine($"Inference took {inferenceStopwatch.ElapsedMilliseconds}ms and generated {totalTokens} tokens. {((float)totalTokens / ((float)inferenceStopwatch.ElapsedMilliseconds / (float)1000)).ToString("F2")} tokens/second.");
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
