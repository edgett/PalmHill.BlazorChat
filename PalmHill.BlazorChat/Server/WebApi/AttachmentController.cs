using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.KernelMemory.FileSystem.DevTools;
using PalmHill.BlazorChat.Server.SignalR;
using PalmHill.BlazorChat.Shared.Models;
using PalmHill.LlmMemory;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PalmHill.BlazorChat.Server.WebApi
{
    [Route("api/[controller]", Name = "Attachment")]
    [ApiController]
    public class AttachmentController : ControllerBase
    {
        private ConversationMemory ConversationMemory { get; }
        private IHubContext<WebSocketChat> WebSocketChat { get; }

        public AttachmentController(
            ConversationMemory conversationMemory,
            IHubContext<WebSocketChat> webSocketChat
            )
        {
            ConversationMemory = conversationMemory;
            WebSocketChat = webSocketChat;
        }

        [HttpGet("list/{conversationId}")]
        public IEnumerable<AttachmentInfo> GetAttachments(string conversationId)
        {
            var conversationAttachments = ConversationMemory
                .AttachmentInfos
                .Where(a => a.Value.ConversationId == conversationId)
                .Select(a => a.Value);

            return conversationAttachments;
        }

        [HttpGet("{attachmentId}")]
        public ActionResult<AttachmentInfo> GetAttachmentById(string attachmentId)
        {
            var attchmentFound = ConversationMemory.AttachmentInfos.TryGetValue(attachmentId, out var attachmentInfo);

            if (!attchmentFound)
            {
                return NotFound();
            }

            return Ok(attachmentInfo);
        }



        public class FileUpload
        {
            public IFormFile? File { get; set; }
        }

        // POST api/<AttachmentController>
        [HttpPost("{conversationId}")]
        public ActionResult<AttachmentInfo> AddAttachment([FromForm] FileUpload fileUpload, string conversationId)
        {
            var file = fileUpload.File;

            if (file == null)
            {
                return BadRequest("No file provided.");
            }

            var attachmentInfo = new AttachmentInfo()
            {
                Name = file.FileName,
                ContentType = file.ContentType,
                Size = file.Length,
                Status = AttachmentStatus.Pending,
                ConversationId = conversationId,
                FileBytes = file.OpenReadStream().ReadAllBytes()
            };
            var userId = Request.Headers["UserId"].SingleOrDefault();
            _ = DoImport(userId, attachmentInfo);

            return attachmentInfo;
        }

        private async Task DoImport(string? userId, AttachmentInfo attachmentInfo)
        {
            await ConversationMemory.ImportDocumentAsync(attachmentInfo);

            if (string.IsNullOrWhiteSpace(userId))
            {
                Console.WriteLine($"UserId is null/empty. AttachmentId: {attachmentInfo.Id}");
                return;
            }

            await WebSocketChat.Clients.User(userId).SendCoreAsync("AttachmentStatusUpdate", [attachmentInfo]);
        }

        // DELETE api/<AttachmentController>/5
        [HttpDelete("{attachmentId}")]
        public async Task DeleteAttachment(string attachmentId)
        {
            var exists = ConversationMemory.AttachmentInfos.TryGetValue(attachmentId, out var attachmentInfo);
            if (!exists)
            {
                return;
            }

            await ConversationMemory.DeleteDocument(attachmentInfo?.ConversationId ?? string.Empty, attachmentId);
        }

        [HttpGet("{attachmentId}/file")]
        public ActionResult GetAttachmentFile(string attachmentId)
        { 
            var attachmentInfo = ConversationMemory.AttachmentInfos[attachmentId];
            if (attachmentInfo == null)
            {
                   return NotFound();
            }

            if (attachmentInfo.FileBytes == null || attachmentInfo.Status != AttachmentStatus.Uploaded)
            { 
                return BadRequest("File not ready.");
            }

            return File(attachmentInfo.FileBytes, attachmentInfo.ContentType, attachmentInfo.Name);

        }
    }
}
