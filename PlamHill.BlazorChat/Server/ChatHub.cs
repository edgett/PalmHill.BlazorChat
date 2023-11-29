using LLama.Common;
using LLama;
using Microsoft.AspNetCore.SignalR;

namespace PlamHill.BlazorChat.Server
{
    public class ChatHub : Hub
    {

        LLamaContext LlamaContext;
        public ChatHub(LLamaContext llamaContext) 
        {
          LlamaContext = llamaContext;
            
        }

        public async Task SendPrompt(string user, string messageId, string message)
        {



            var context = LlamaContext;
            var ex = new InteractiveExecutor(context);
            ChatSession session = new ChatSession(ex);

            //// show the prompt
            //Console.WriteLine();
            //Console.Write(prompt);

            // run the inference in a loop to chat with LLM

            await foreach (var text in session.ChatAsync(message, new InferenceParams() { Temperature = 0.6f, AntiPrompts = new List<string> { "User:" } }))
            {
                await Clients.Caller.SendAsync("ReceiveModelString", user, messageId, text);
            }
        }
    }

}
