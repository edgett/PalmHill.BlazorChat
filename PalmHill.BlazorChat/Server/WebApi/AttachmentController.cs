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
        private LlmMemory.ServerlessLlmMemory LlmMemory { get; }
        private IHubContext<WebSocketChat> WebSocketChat { get; }

        public AttachmentController(
            LlmMemory.ServerlessLlmMemory llmMemory,
            IHubContext<WebSocketChat> webSocketChat
            )
        {
            LlmMemory = llmMemory;
            WebSocketChat = webSocketChat;
        }

        [HttpPost("ask")]
        public async Task<ActionResult<ChatMessage>> Ask(ChatConversation chatConversation)
        {
            var conversationId = chatConversation.Id;
            var question = chatConversation.ChatMessages.LastOrDefault()?.Message;
            if (question == null)
            {
                return BadRequest("No question provided.");
            }


            var answer = await LlmMemory.Ask(conversationId, question);
            var chatMessageAnswer = new ChatMessage()
            {
                Role = ChatMessageRole.Assistant,
                Message = answer.Result,
                AttachmentIds = answer.RelevantSources.Select(s => s.SourceName).ToList()
            };

            return chatMessageAnswer;
        }

        [HttpGet("list/{conversationId}")]
        public IEnumerable<AttachmentInfo> GetAttachments(string conversationId)
        {
            var conversationAttachments = LlmMemory
                .AttachmentInfos
                .Where(a => a.Value.ConversationId == conversationId)
                .Select(a => a.Value);

            return conversationAttachments;
        }

        [HttpGet("{attachmentId}")]
        public ActionResult<AttachmentInfo> GetAttachmentById(string attachmentId)
        {
            var attchmentFound = LlmMemory.AttachmentInfos.TryGetValue(attachmentId, out var attachmentInfo);

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
        [HttpPost("{conversationId}/{attachmentId}")]
        public ActionResult<AttachmentInfo> AddAttachment([FromForm] FileUpload fileUpload, string conversationId, string attachmentId)
        {
            var file = fileUpload.File;

            if (file == null)
            {
                return BadRequest("No file provided.");
            }

            var attachmentInfo = new AttachmentInfo()
            {
                Id = attachmentId,
                Name = file.FileName,
                ContentType = file.ContentType,
                Size = file.Length,
                Status = AttachmentStatus.Pending,
                ConversationId = conversationId,
                FileBytes = file.OpenReadStream().ReadAllBytes()
            };
            var userId = "user1";
            _ = DoImportAsync(userId, attachmentInfo);

            return attachmentInfo;
        }

        private async Task DoImportAsync(string? userId, AttachmentInfo attachmentInfo)
        {
            try
            {
                await LlmMemory.ImportDocumentAsync(attachmentInfo, null, ThreadLock.InferenceLock);
                await WebSocketChat.Clients.User(userId!).SendCoreAsync("AttachmentStatusUpdate", [attachmentInfo]);

            }
            catch (Exception ex)
            {
                attachmentInfo.Status = AttachmentStatus.Failed;
                await WebSocketChat.Clients.User(userId!).SendCoreAsync("AttachmentStatusUpdate", [attachmentInfo]);
            }
        }

        // DELETE api/<AttachmentController>/5
        [HttpDelete("{attachmentId}")]
        public async Task DeleteAttachment(string attachmentId)
        {
            var exists = LlmMemory.AttachmentInfos.TryGetValue(attachmentId, out var attachmentInfo);
            if (!exists)
            {
                return;
            }

            await LlmMemory.DeleteDocument(attachmentInfo?.ConversationId ?? string.Empty, attachmentId);
        }

        [HttpGet("{attachmentId}/file")]
        public ActionResult GetAttachmentFile(string attachmentId)
        { 
            var attachmentInfo = LlmMemory.AttachmentInfos[attachmentId];
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
