using LLama.Common;
using LLama;
using Microsoft.AspNetCore.Mvc;
using PalmHill.BlazorChat.Shared.Models;
using PalmHill.Llama;
using System.Diagnostics;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PalmHill.BlazorChat.Server.WebApi
{

    /// <summary>
    /// The ApiChat class is responsible for handling chat API requests.
    /// </summary>
    [Route("api/chat", Name = "Chat")]
    [ApiController]
    public class ApiChat : ControllerBase
    {
        /// <summary>
        /// The LLamaWeights instance used for model weights.
        /// </summary>
        LLamaWeights LLamaWeights;

        /// <summary>
        /// The ModelParams instance used for model parameters.
        /// </summary>
        ModelParams ModelParams;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiChat"/> class.
        /// </summary>
        /// <param name="model">The LLamaWeights model.</param>
        /// <param name="modelParams">The model parameters.</param>
        public ApiChat(LLamaWeights model, ModelParams modelParams)
        {
            LLamaWeights = model;
            ModelParams = modelParams;
        }

        /// <summary>
        /// Handles a chat API request.
        /// </summary>
        /// <param name="conversation">The chat conversation.</param>
        /// <returns>Returns a string response from the chat model inference.</returns>
        /// <exception cref="Exception">Thrown when there is an error during the chat model inference.</exception>
        [HttpPost(Name = "Chat")]
        public async Task<ActionResult<string>> Chat([FromBody] ChatConversation conversation)
        {
            var errorText = "";

            await ThreadLock.InferenceLock.WaitAsync();
            try
            {
                var response = await DoInference(conversation);
                return Ok(response);
            }
            catch (Exception ex)
            {
                errorText = ex.ToString();
            }
            finally
            {
                ThreadLock.InferenceLock.Release();
            }

            Console.WriteLine(errorText);

            return StatusCode(500, errorText);
        }

        /// <summary>
        /// Performs inference for a chat conversation.
        /// </summary>
        /// <param name="conversation">The chat conversation for which to perform inference.</param>
        /// <returns>Returns the inference result as a string.</returns>
        private async Task<string> DoInference(ChatConversation conversation)
        {
            LLamaContext modelContext = LLamaWeights.CreateContext(ModelParams);
            var session = modelContext.CreateChatSession(conversation);
            var inferenceParams = conversation.GetInferenceParams();

            var cancelGeneration = new CancellationTokenSource();
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
            }
            modelContext.Dispose();
            inferenceStopwatch.Stop();

            Console.WriteLine($"Inference took {inferenceStopwatch.ElapsedMilliseconds}ms and generated {totalTokens} tokens. {(totalTokens / (inferenceStopwatch.ElapsedMilliseconds / (float)1000)).ToString("F2")} tokens/second.");
            Console.WriteLine(fullResponse);

            return fullResponse;
        }

    }
}
