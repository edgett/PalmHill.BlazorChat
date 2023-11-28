using LLama.Common;
using LLama;
using Microsoft.AspNetCore.SignalR;

namespace PlamHill.BlazorChat.Server
{
    public class ChatHub : Hub
    {

        LLamaWeights model;
        ModelParams parameters;
        public ChatHub(LLamaWeights lLamaWeights, ModelParams modelParams) 
        {
            model = lLamaWeights;
            parameters = modelParams;
            
        }

        public async Task SendMessage(string user, string message)
        {

          

            using var context = model.CreateContext(parameters);
            var ex = new InteractiveExecutor(context);
            ChatSession session = new ChatSession(ex);

            //// show the prompt
            //Console.WriteLine();
            //Console.Write(prompt);

            // run the inference in a loop to chat with LLM

            await foreach (var text in session.ChatAsync(message, new InferenceParams() { Temperature = 0.6f, AntiPrompts = new List<string> { "User:" } }))
            {
                await Clients.Caller.SendAsync("ReceiveMessage", user, text);
            }
        }
    }

}
