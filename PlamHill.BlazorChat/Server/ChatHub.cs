﻿using LLama.Common;
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
            

            chatConversation.SystemMessage = "You are a helful assistant. Repsond in markdown.";
            var rawPrompt = chatConversation.ToLlamaPromptString();

            var textBuffer = "";

            // run the inference in a loop to chat with LLM
            await foreach (var text in session.ChatAsync(rawPrompt, new InferenceParams() { Temperature = 0.6f, AntiPrompts = new List<string> { ChatExtensions.MESSAGE_END } }))
            {
                Console.WriteLine(text);

                textBuffer += text;
                var shouldSendBuffer = ShouldSendBuffer(textBuffer);

                if (shouldSendBuffer)
                {
                    await Clients.Caller.SendAsync("ReceiveModelString", messageId, textBuffer);
                    textBuffer = "";
                }
            }
        }

        private bool ShouldSendBuffer(string textBuffer)
        {



            // Check if the end of the text is ChatExtensions.MESSAGE_END
            if (textBuffer.EndsWith(ChatExtensions.MESSAGE_END))
            {
                return true;
            }


            if (string.IsNullOrEmpty(textBuffer))
            {
                return false;
            }

            // Check if the last character is a punctuation mark or whitespace
            char lastChar = textBuffer[^1]; // Using ^1 to get the last character
            if (char.IsPunctuation(lastChar) || char.IsWhiteSpace(lastChar))
            {

                if (textBuffer.Contains("<")) 
                {
                    return false;
                }

                return true;
            }

           

            return false;
        }
    }

}
