using LLama.Common;
using LLama;
using Microsoft.AspNetCore.SignalR;
using PalmHill.BlazorChat.Shared;
using PalmHill.BlazorChat.Shared.Models;

namespace PlamHill.BlazorChat.Server
{
    public class ChatHub : Hub
    {

        LLamaContext LlamaContext;
        public ChatHub(LLamaContext llamaContext) 
        {
          LlamaContext = llamaContext;
            
        }

        public async Task SendPrompt(Guid messageId, ChatConversation chatConversation)
        {
            var context = LlamaContext;
            var ex = new InteractiveExecutor(context);
            ChatSession session = new ChatSession(ex);
            
            var rawPrompt = chatConversation.ToLlamaPromptString();
            // run the inference in a loop to chat with LLM
            await foreach (var text in session.ChatAsync(rawPrompt, new InferenceParams() { Temperature = 0.6f, AntiPrompts = new List<string> { ChatExtensions.MESSAGE_END } }))
            {
                await Clients.Caller.SendAsync("ReceiveModelString", messageId, text);
            }


        }
    }

}
