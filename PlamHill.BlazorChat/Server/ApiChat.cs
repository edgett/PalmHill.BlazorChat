using LLama.Common;
using LLama;
using Microsoft.AspNetCore.Mvc;
using PalmHill.BlazorChat.Shared.Models;
using PalmHill.Llama;
using System.Diagnostics;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PlamHill.BlazorChat.Server
{
    [Route("api/chat", Name = "Chat")]
    [ApiController]
    public class ApiChat : ControllerBase
    {
        LLamaWeights LLamaWeights;
        ModelParams ModelParams;
        public ApiChat(LLamaWeights model, ModelParams modelParams)
        {
            LLamaWeights = model;
            ModelParams = modelParams;
        }

 
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

            Console.WriteLine($"Inference took {inferenceStopwatch.ElapsedMilliseconds}ms and generated {totalTokens} tokens. {((float)totalTokens / ((float)inferenceStopwatch.ElapsedMilliseconds / (float)1000)).ToString("F2")} tokens/second.");
            Console.WriteLine(fullResponse);

            return fullResponse;
        }

    }
}
