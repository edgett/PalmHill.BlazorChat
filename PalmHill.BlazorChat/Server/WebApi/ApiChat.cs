using LLama.Common;
using LLama;
using Microsoft.AspNetCore.Mvc;
using PalmHill.BlazorChat.Shared.Models;
using PalmHill.Llama;
using System.Diagnostics;
using PalmHill.Llama.Models;

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
        /// Initializes a new instance of the <see cref="ApiChat"/> class.
        /// </summary>
        /// <param name="model">The LLamaWeights model.</param>
        /// <param name="modelParams">The model parameters.</param>
        public ApiChat(InjectedModel injectedModel)
        {
            InjectedModel = injectedModel;
        }

        private InjectedModel InjectedModel { get; }

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
        private async Task<string> DoInference(InferenceRequest conversation)
        {
            LLamaContext modelContext = InjectedModel.Model.CreateContext(InjectedModel.ModelParams);
            var session = modelContext.CreateChatSession(conversation);
            var inferenceParams = conversation.GetInferenceParams(InjectedModel.DefaultAntiPrompts);

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
