using Refit;
using PalmHill.BlazorChat.Shared.Models;


namespace PalmHill.BlazorChat.ApiClient.WebApiInterface
{
    public interface IChat
    {
        [Post("/api/chat")]
        Task<ApiResponse<string>> Chat(ChatConversation conversation);

        [Post("/api/attachment/ask")]
        Task<ApiResponse<ChatMessage>> Ask(ChatConversation chatConversation);
    }
}
