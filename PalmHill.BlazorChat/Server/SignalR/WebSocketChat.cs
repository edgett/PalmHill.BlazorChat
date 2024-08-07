﻿using Azure.AI.OpenAI;
using LLama;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using Microsoft.SemanticKernel.ChatCompletion;
using PalmHill.BlazorChat.Server.WebApi;
using PalmHill.BlazorChat.Shared.Models;
using PalmHill.BlazorChat.Shared.Models.WebSocket;
using PalmHill.Llama;
using PalmHill.Llama.Models;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace PalmHill.BlazorChat.Server.SignalR
{

    /// <summary>
    /// The WebSocketChat class is a SignalR Hub that handles real-time chat communication. It uses a LLamaWeights model and ModelParams to process chat conversations and perform inference.
    /// </summary>
    public class WebSocketChat : Hub
    {
        public WebSocketChat(LlamaKernel llamaKernel, ILogger<WebSocketChat> logger)
        {
            LlamaKernel = llamaKernel;
            ChatCompletion = llamaKernel.Kernel.Services.GetService<IChatCompletionService>();
            _logger = logger;
        }

        public LlamaKernel LlamaKernel { get; }
        public IChatCompletionService? ChatCompletion { get; }
        private ILogger<WebSocketChat> _logger { get; }

        /// <summary>
        /// Sends a chat prompt to the client and waits for a response. The method performs inference on the chat conversation and sends the result back to the client.
        /// </summary>
        /// <param name="messageId">The unique identifier for the message.</param>
        /// <param name="chatConversation">The chat conversation to send.</param>
        /// <returns>A Task that represents the asynchronous operation.</returns>
        /// <exception cref="Exception">Thrown when an error occurs during the inference process.</exception>
        public async IAsyncEnumerable<WebSocketInferenceString> InferenceRequest(InferenceRequest chatConversation, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var conversationId = chatConversation.Id;

          
                //await ThreadLock.InferenceLock.WaitAsync(cancellationTokenSource.Token);
                await foreach (var m in DoInferenceAndRespondToClient(Clients.Caller, chatConversation, cancellationToken))
                { 
                    yield return m;
                
                }
        }


        /// <summary>
        /// Performs inference on a chat conversation and sends the response to the client. The inference is performed using the LLamaWeights model and ModelParams.
        /// </summary>
        /// <param name="respondToClient">The client to respond to.</param>
        /// <param name="messageId">The unique identifier for the message.</param>
        /// <param name="chatConversation">The chat conversation to use for inference.</param>
        /// <returns>A Task that represents the asynchronous operation.</returns>
        [SerialExecution("ModelOperation")]
        private async IAsyncEnumerable<WebSocketInferenceString> DoInferenceAndRespondToClient(ISingleClientProxy respondToClient, InferenceRequest chatConversation, CancellationToken cancellationToken)
        {
            // Create a context for the model and a chat session for the conversation
            var chatHistory = chatConversation.GetChatHistory();
            var inferenceParams = chatConversation.GetPromptExecutionSettings();

            var messageId = chatConversation.ChatMessages.LastOrDefault()?.Id;

            var textBuffer = "";
            var fullResponse = "";
            var totalTokens = 0;
            var inferenceStopwatch = new Stopwatch();


            inferenceStopwatch.Start();
            var asyncResponse = ChatCompletion?.GetStreamingChatMessageContentsAsync(chatHistory, inferenceParams, cancellationToken: cancellationToken);

            if (asyncResponse == null)
            {
                _logger.LogError($"{nameof(IChatCompletionService)} not implemented.");
                //await respondToClient.SendAsync("ReceiveInferenceString", $"Error: {nameof(IChatCompletionService)} not implemented.");
                yield return new WebSocketInferenceString() { WebSocketChatMessageId = messageId ?? Guid.NewGuid(), InferenceString = "Error" };
                
            }


            // Perform inference and send the response to the client
            await foreach (var text in asyncResponse!)
            {
                if (cancellationToken.IsCancellationRequested)
                {
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
                    inferenceString.WebSocketChatMessageId = messageId ?? Guid.NewGuid();
                    inferenceString.InferenceString = textBuffer;

                    yield return inferenceString;
                    textBuffer = "";
                }


            }
            
            inferenceStopwatch.Stop();
            
            if (textBuffer.Length > 0)
            {
                yield return new WebSocketInferenceString() { WebSocketChatMessageId = messageId ?? Guid.NewGuid(), InferenceString = textBuffer };
            }

            _logger.LogInformation($"Inference took {inferenceStopwatch.ElapsedMilliseconds}ms and generated {totalTokens} tokens. {(totalTokens / (inferenceStopwatch.ElapsedMilliseconds / (float)1000)).ToString("F2")} tokens/second.");
            _logger.LogInformation(fullResponse);
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
