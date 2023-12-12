using LLama.Common;
using LLama;
using Microsoft.AspNetCore.SignalR;
using PalmHill.BlazorChat.Shared;
using PalmHill.BlazorChat.Shared.Models;
using System.Web;
using PalmHill.Llama;
using System.Diagnostics;
using PalmHill.Llama.Models;
using PalmHill.LlmMemory;
using PalmHill.BlazorChat.Shared.Models.WebSocket;
using System.Collections.Concurrent;

namespace PalmHill.BlazorChat.Server.SignalR
{

    /// <summary>
    /// The WebSocketChat class is a SignalR Hub that handles real-time chat communication. It uses a LLamaWeights model and ModelParams to process chat conversations and perform inference.
    /// </summary>
    public class WebSocketChat : Hub
    {
        public WebSocketChat(InjectedModel injectedModel, LlmMemory.ServerlessLlmMemory? llmMemory = null)
        {
            InjectedModel = injectedModel;
            LlmMemory = llmMemory;
        }
        private InjectedModel InjectedModel { get; }
        private ServerlessLlmMemory? LlmMemory { get; }
        

        /// <summary>
        /// Sends a chat prompt to the client and waits for a response. The method performs inference on the chat conversation and sends the result back to the client.
        /// </summary>
        /// <param name="messageId">The unique identifier for the message.</param>
        /// <param name="chatConversation">The chat conversation to send.</param>
        /// <returns>A Task that represents the asynchronous operation.</returns>
        /// <exception cref="Exception">Thrown when an error occurs during the inference process.</exception>
        public async Task InferenceRequest(InferenceRequest chatConversation)
        {
            var conversationId = chatConversation.Id;
            var cancellationTokenSource = new CancellationTokenSource();
            ChatCancelation.CancelationTokens[conversationId] = cancellationTokenSource;

            try
            {
                await ThreadLock.InferenceLock.WaitAsync(cancellationTokenSource.Token);
                await DoInferenceAndRespondToClient(Clients.Caller, chatConversation, cancellationTokenSource.Token);

                var inferenceStatusUpdate = new WebSocketInferenceStatusUpdate();
                inferenceStatusUpdate.MessageId = chatConversation.ChatMessages.LastOrDefault()?.Id;
                inferenceStatusUpdate.IsComplete = true;
                inferenceStatusUpdate.Success = true;
                await Clients.Caller.SendAsync("InferenceStatusUpdate", inferenceStatusUpdate);
            }
         
            catch (OperationCanceledException)
            {
                var inferenceStatusUpdate = new WebSocketInferenceStatusUpdate();
                inferenceStatusUpdate.MessageId = chatConversation.ChatMessages.LastOrDefault()?.Id;
                inferenceStatusUpdate.IsComplete = true;
                inferenceStatusUpdate.Success = false;
                await Clients.Caller.SendAsync("InferenceStatusUpdate", inferenceStatusUpdate);
                // Handle the cancellation operation
                Console.WriteLine($"Inference for {conversationId} was canceled.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                ThreadLock.InferenceLock.Release();
                ChatCancelation.CancelationTokens.TryRemove(conversationId, out _);
                
            }
        }


        /// <summary>
        /// Performs inference on a chat conversation and sends the response to the client. The inference is performed using the LLamaWeights model and ModelParams.
        /// </summary>
        /// <param name="respondToClient">The client to respond to.</param>
        /// <param name="messageId">The unique identifier for the message.</param>
        /// <param name="chatConversation">The chat conversation to use for inference.</param>
        /// <returns>A Task that represents the asynchronous operation.</returns>
        private async Task DoInferenceAndRespondToClient(ISingleClientProxy respondToClient,  InferenceRequest chatConversation, CancellationToken cancellationToken)
        {
            // Create a context for the model and a chat session for the conversation
            LLamaContext modelContext = InjectedModel.Model.CreateContext(InjectedModel.ModelParams);
            var session = modelContext.CreateChatSession(chatConversation);
            var inferenceParams = chatConversation.GetInferenceParams(InjectedModel.DefaultAntiPrompts);

            var messageId = chatConversation.ChatMessages.LastOrDefault()?.Id;
            
            var textBuffer = "";
            var fullResponse = "";
            var totalTokens = 0;
            var inferenceStopwatch = new Stopwatch();

            inferenceStopwatch.Start();
            var asyncResponse = session.ChatAsync(session.History,
                                                        inferenceParams,
                                                        cancellationToken);
            // Perform inference and send the response to the client
            await foreach (var text in asyncResponse)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    modelContext.Dispose();
                    inferenceStopwatch.Stop();

                    throw new OperationCanceledException(cancellationToken);
                }

                totalTokens++;
                fullResponse += text;
                textBuffer += text;
                var shouldSendBuffer = ShouldSendBuffer(textBuffer);

                if (shouldSendBuffer)
                {
                    var inferenceString = new WebSocketInferenceString();
                    inferenceString.MessageId = messageId ?? Guid.NewGuid();
                    inferenceString.InferenceString = textBuffer;

                    await respondToClient.SendAsync("ReceiveInferenceString", inferenceString);
                    textBuffer = "";
                }


            }
            modelContext.Dispose(); 

            inferenceStopwatch.Stop();

            if (textBuffer.Length > 0)
            {
                await respondToClient.SendAsync("ReceiveInferenceString", chatConversation.Id, textBuffer);
            }

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
