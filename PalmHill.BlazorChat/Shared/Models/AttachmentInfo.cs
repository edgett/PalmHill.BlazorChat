using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PalmHill.BlazorChat.Shared.Models
{
    /// <summary>
    /// Information about an attachment.
    /// </summary>
    public class AttachmentInfo
    {
        /// <summary>
        /// The Id of the attachment.
        /// </summary>
        public Guid Id { get; set; } = Guid.NewGuid();
        /// <summary>
        /// The file name of the attachment.
        /// </summary>
        public string Name { get; set; } = "";
        
        /// <summary>
        /// The file bytes of the attachment. (Not serialized). Used for import/memorization.
        /// </summary>
        [JsonIgnore]
        public byte[]? FileBytes { get; set; }

        /// <summary>
        /// The content type of the attachment.
        /// </summary>
        public string ContentType { get; set; } = "";

        /// <summary>
        /// The size of the attachment.
        /// </summary>
        public long Size { get; set; } = 0;

        /// <summary>
        /// The status of the attachment.
        /// </summary>
        public AttachmentStatus Status { get; set; } = AttachmentStatus.Pending;

        /// <summary>
        /// The Id of the conversation the attachment belongs to. Used for DB (later).
        /// </summary>
        public Guid? ConversationId { get; set; }
    }

    public enum AttachmentStatus
    {
        /// <summary>
        /// The attachment is pending. Processing is not complete.
        /// </summary>
        Pending,
        /// <summary>
        /// The attachment is uploaded. Processing is complete.
        /// </summary>
        Uploaded,
        /// <summary>
        /// The attachment failed to upload or processing is failed.
        /// </summary>
        Failed
    }
}
