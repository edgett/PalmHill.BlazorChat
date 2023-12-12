using Refit;
using PalmHill.BlazorChat.Shared.Models;


namespace PalmHill.BlazorChat.ApiClient.WebApiInterface
{
    public interface IChat
    {
        [Post("/api/chat")]
        Task<ApiResponse<string>> Chat(InferenceRequest conversation);

        [Post("/api/chat/docs")]
        Task<ApiResponse<ChatMessage>> Ask(InferenceRequest chatConversation);

        [Delete("/api/chat/cancel/{conversationId}")]
        public Task<ApiResponse<bool>> CancelChat(Guid conversationId);

    }
}
