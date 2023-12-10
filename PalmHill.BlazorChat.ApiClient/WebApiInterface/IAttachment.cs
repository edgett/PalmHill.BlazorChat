using PalmHill.BlazorChat.Shared.Models;
using Refit;


namespace PalmHill.BlazorChat.ApiClient.WebApiInterface
{
    public interface IAttachment
    {
        [Get("/api/Attachment/list/{conversationId}")]
        Task<IEnumerable<AttachmentInfo>> GetAttachments(string conversationId);

        [Get("/api/Attachment/{attachmentId}")]
        Task<ApiResponse<AttachmentInfo>> GetAttachmentById(string attachmentId);

        [Multipart]
        [Post("/api/Attachment/{conversationId}/{attachmentId}")]
        Task<ApiResponse<AttachmentInfo>> AddAttachment(string conversationId, string attachmentId, [AliasAs("file")] StreamPart file);

        [Delete("/api/Attachment/{attachmentId}")]
        Task DeleteAttachment(string attachmentId);
    }
}
