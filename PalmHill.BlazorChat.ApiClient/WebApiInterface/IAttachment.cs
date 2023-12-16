using PalmHill.BlazorChat.Shared.Models;
using Refit;


namespace PalmHill.BlazorChat.ApiClient.WebApiInterface
{
    public interface IAttachment
    {
        [Get("/api/attachment/list/{conversationId}")]
        Task<IEnumerable<AttachmentInfo>> GetAttachments(string conversationId);

        [Get("/api/attachment/{attachmentId}")]
        Task<ApiResponse<AttachmentInfo>> GetAttachmentById(string attachmentId);

        [Multipart]
        [Post("/api/attachment/{conversationId}/{attachmentId}")]
        Task<ApiResponse<AttachmentInfo>> AddAttachment(Guid conversationId, Guid attachmentId, [AliasAs("file")] StreamPart file);

        [Delete("/api/attachment/{attachmentId}")]
        Task<ApiResponse<bool>> DeleteAttachment(Guid attachmentId);
    }
}
