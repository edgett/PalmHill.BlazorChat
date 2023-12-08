using LLama.Common;
using LLama;
using Microsoft.AspNetCore.SignalR;
using PalmHill.BlazorChat.Shared;
using PalmHill.BlazorChat.Shared.Models;
using System.Web;
using PalmHill.Llama;
using System.Diagnostics;

namespace PalmHill.BlazorChat.Server.SignalR
{

    /// <summary>
    /// The WebSocketChat class is a SignalR Hub that handles real-time chat communication. It uses a LLamaWeights model and ModelParams to process chat conversations and perform inference.
    /// </summary>
    public class WebSocketChat : Hub
    {
        LLamaWeights LLamaWeights;
        ModelParams ModelParams;
        public WebSocketChat(LLamaWeights model, ModelParams modelParams)
        {
            LLamaWeights = model;
            ModelParams = modelParams;
        }




        /// <summary>
        /// Sends a chat prompt to the client and waits for a response. The method performs inference on the chat conversation and sends the result back to the client.
        /// </summary>
        /// <param name="messageId">The unique identifier for the message.</param>
        /// <param name="chatConversation">The chat conversation to send.</param>
        /// <returns>A Task that represents the asynchronous operation.</returns>
        /// <exception cref="Exception">Thrown when an error occurs during the inference process.</exception>
        public async Task SendPrompt(Guid messageId, ChatConversation chatConversation)
        {
            await ThreadLock.InferenceLock.WaitAsync();

            try
            {
                await DoInferenceAndRespondToClient(Clients.Caller, messageId, chatConversation);

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


        /// <summary>
        /// Performs inference on a chat conversation and sends the response to the client. The inference is performed using the LLamaWeights model and ModelParams.
        /// </summary>
        /// <param name="respondToClient">The client to respond to.</param>
        /// <param name="messageId">The unique identifier for the message.</param>
        /// <param name="chatConversation">The chat conversation to use for inference.</param>
        /// <returns>A Task that represents the asynchronous operation.</returns>
        private async Task DoInferenceAndRespondToClient(ISingleClientProxy respondToClient, Guid messageId, ChatConversation chatConversation)
        {

            // Create a context for the model and a chat session for the conversation
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
            // Perform inference and send the response to the client
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
            Console.WriteLine($"Inference took {inferenceStopwatch.ElapsedMilliseconds}ms and generated {totalTokens} tokens. {(totalTokens / (inferenceStopwatch.ElapsedMilliseconds / (float)1000)).ToString("F2")} tokens/second.");
            Console.WriteLine(fullResponse);
        }

        /// <summary>
        /// Determines whether the text buffer should be sent to the client. The buffer is sent if it is not empty and the last character is a punctuation mark or whitespace.
        /// </summary>
        /// <param name="textBuffer">The text buffer to check.</param>
        /// <returns>True if the text buffer should be sent; otherwise, false.</returns>
        private bool ShouldSendBuffer(string textBuffer)
        {

            if (string.IsNullOrEmpty(textBuffer))
            {
                return false;
            }

            // Check if the last character is a punctuation mark or whitespace
            // This is done to ensure that the buffer is sent only when a sentence or phrase is complete
            char lastChar = textBuffer[^1]; // Using ^1 to get the last character
            if (char.IsPunctuation(lastChar) || char.IsWhiteSpace(lastChar))
            {
                return true;
            }

            return false;
        }
    }

}
