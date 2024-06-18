using LLama;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PalmHill.BlazorChat.Server.SignalR;
using PalmHill.BlazorChat.Shared.Models;
using PalmHill.Llama;
using PalmHill.Llama.Models;
using System.Diagnostics;
using System.Text;
using Microsoft.KernelMemory;
using Microsoft.SemanticKernel.ChatCompletion;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PalmHill.BlazorChat.Server.WebApi
{

    /// <summary>
    /// The ApiChat class is responsible for handling chat API requests.
    /// </summary>
    [Route("api/chat", Name = "Chat")]
    [ApiController]
    public class ApiChatController : ControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApiChatController"/> class.
        /// </summary>
        /// <param name="model">The LLamaWeights model.</param>
        /// <param name="modelParams">The model parameters.</param>
        public ApiChatController(
            IHubContext<WebSocketChat> webSocketChat,
            LlamaKernel llamaKernel
            )
        {
            WebSocketChat = webSocketChat;
            LlamaKernel = llamaKernel;
            LlmMemory = llamaKernel.Kernel.Services.GetService<ServerlessLlmMemory>();
            ChatCompletion = llamaKernel.Kernel.Services.GetService<IChatCompletionService>();
        }

        private IHubContext<WebSocketChat> WebSocketChat { get; }
        public LlamaKernel LlamaKernel { get; }
        public ServerlessLlmMemory? LlmMemory { get; }
        public IChatCompletionService? ChatCompletion { get; }

        /// <summary>
        /// Handles a chat API request.
        /// </summary>
        /// <param name="conversation">The chat conversation.</param>
        /// <returns>Returns a string response from the chat model inference.</returns>
        /// <exception cref="Exception">Thrown when there is an error during the chat model inference.</exception>
        [HttpPost(Name = "Chat")]
        public async Task<ActionResult<string>> Chat([FromBody] InferenceRequest conversation)
        {
            var errorText = "";

            var conversationId = conversation.Id;
            var cancellationTokenSource = new CancellationTokenSource();
            ChatCancelation.CancelationTokens[conversationId] = cancellationTokenSource;

            try
            {
                //await ThreadLock.InferenceLock.WaitAsync(cancellationTokenSource.Token);
                var response = await DoInference(conversation, cancellationTokenSource.Token);
                return Ok(response);
            }
            catch (OperationCanceledException)
            {
                errorText = $"Inference for {conversationId} was canceled.";
                Console.WriteLine(errorText);
                return StatusCode(444, errorText);
            }
            catch (Exception ex)
            {
                errorText = ex.ToString();
            }
            finally
            {
                //ThreadLock.InferenceLock.Release();
                ChatCancelation.CancelationTokens.TryRemove(conversationId, out _);
            }

            Console.WriteLine(errorText);
            return StatusCode(500, errorText);
        }

        [HttpPost("docs")]
        public async Task<ActionResult<ChatMessage>> Ask(InferenceRequest chatConversation)
        {
            if (LlmMemory == null)
            {
                var result = StatusCode(503, "No LlmMemory loaded.");
                return result;
            }

            var conversationId = chatConversation.Id;
            var cancellationTokenSource = new CancellationTokenSource();
            ChatCancelation.CancelationTokens[conversationId] = cancellationTokenSource;

            var question = chatConversation.ChatMessages.LastOrDefault()?.Message;
            if (question == null)
            {
                return BadRequest("No question provided.");
            }

            try
            {
                var answer = await LlmMemory.Ask(conversationId.ToString(), question, cancellationTokenSource.Token);

                var chatMessageAnswer = new ChatMessage()
                {
                    Role = ChatMessageRole.Assistant,
                    Message = answer.Result,
                    AttachmentIds = answer.RelevantSources.Select(s => s.SourceName).ToList()
                };

                if (cancellationTokenSource.Token.IsCancellationRequested)
                {
                    throw new OperationCanceledException(cancellationTokenSource.Token);
                }

                return chatMessageAnswer;
            }
            catch (OperationCanceledException ex)
            {
                return StatusCode(444, ex.ToString());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        [HttpDelete("cancel/{conversationId}", Name = "CancelChat")]
        public async Task<bool> CancelChat(Guid conversationId)
        {
            var cancelToken = ChatCancelation.CancelationTokens[conversationId];
            if (cancelToken == null)
            {
                return false;
            }
            else
            {
                await cancelToken.CancelAsync();
                return true;
            }
        }

        /// <summary>
        /// Performs inference for a chat conversation.
        /// </summary>
        /// <param name="conversation">The chat conversation for which to perform inference.</param>
        /// <returns>Returns the inference result as a string.</returns>
        private async Task<string> DoInference(InferenceRequest conversation, CancellationToken cancellationToken)
        {

            var fullResponse = new StringBuilder();
            var totalTokens = 0;
            var inferenceStopwatch = new Stopwatch();

            var chatSession = new ChatHistory(conversation.SystemMessage);
            foreach (var message in conversation.ChatMessages)
            {
                var role = (message.Role == ChatMessageRole.Assistant ? AuthorRole.Assistant : AuthorRole.User);
                var chatHistoryItem = new Microsoft.SemanticKernel.ChatMessageContent(role, message.Message);
                chatSession.Add(chatHistoryItem);
            }

            var chatExecutionSettings = conversation.GetPromptExecutionSettings();

            inferenceStopwatch.Start();
            var asyncResponse = ChatCompletion.GetStreamingChatMessageContentsAsync(chatSession, 
                chatExecutionSettings, 
                cancellationToken: cancellationToken);


            await foreach (var text in asyncResponse)
            {
                totalTokens++;
                fullResponse.Append(text);
            }
            inferenceStopwatch.Stop();
            var fullResponseString = fullResponse.ToString();
            Console.WriteLine($"Inference took {inferenceStopwatch.ElapsedMilliseconds}ms and generated {totalTokens} tokens. {(totalTokens / (inferenceStopwatch.ElapsedMilliseconds / (float)1000)).ToString("F2")} tokens/second.");
            Console.WriteLine(fullResponseString);

            return fullResponseString;
        }

    }
}
