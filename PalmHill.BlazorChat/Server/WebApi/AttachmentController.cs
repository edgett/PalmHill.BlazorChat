using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.KernelMemory.FileSystem.DevTools;
using PalmHill.BlazorChat.Shared.Models;
using PalmHill.LlmMemory;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PalmHill.BlazorChat.Server.WebApi
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentController : ControllerBase
    {
        private ConversationMemory ConversationMemory { get; }

        public AttachmentController(ConversationMemory conversationMemory)
        {
            ConversationMemory = conversationMemory;
        }

        [HttpGet("{conversationId}")]
        public IEnumerable<AttachmentInfo> GetConversationAttachments(string conversationId)
        {
            var conversationAttachments = ConversationMemory
                .AttachmentInfos
                .Where(a => a.Value.ConversationId == conversationId)
                .Select(a => a.Value);

            return conversationAttachments;
        }

        [HttpGet("{conversationId}/{attachmetId}")]
        public ActionResult<AttachmentInfo> GetAttachmentById(string conversationId, string attachmetId)
        {
            var attchmentFound = ConversationMemory.AttachmentInfos.TryGetValue(attachmetId, out var attachmentInfo);

            if (!attchmentFound)
            {
                return NotFound();
            }

            if (attachmentInfo?.ConversationId == conversationId)
            { 
                return attachmentInfo;
            }
            else
            {
                return NotFound();
            }

        }

        // POST api/<AttachmentController>
        [HttpPost("{conversationId}")]
        public ActionResult<AttachmentInfo> Post([FromForm] IFormFile file, string conversationId)
        {
            var attachmentInfo = new AttachmentInfo()
            {
                Name = file.FileName,
                ContentType = file.ContentType,
                Size = file.Length,
                Status = AttachmentStatus.Pending,
                ConversationId = conversationId,
                FileBytes = file.OpenReadStream().ReadAllBytes()
            };
            ConversationMemory.ImportDocumentAsync(attachmentInfo);
            return attachmentInfo;
        }

        // DELETE api/<AttachmentController>/5
        [HttpDelete("{conversationId}/{attachmetId}")]
        public async Task Delete(string conversationId, string attachmetId)
        {
            await ConversationMemory.DeleteDocument(conversationId, attachmetId);
        }

    }
}
