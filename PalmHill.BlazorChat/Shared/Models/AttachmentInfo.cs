using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PalmHill.BlazorChat.Shared.Models
{
    public class AttachmentInfo
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "";
        [JsonIgnore]
        public byte[]? FileBytes { get; set; }
        public string ContentType { get; set; } = "";
        public long Size { get; set; } = 0;
        public AttachmentStatus Status { get; set; } = AttachmentStatus.Pending;
        public string ConversationId { get; set; } = "";
    }

    public enum AttachmentStatus
    {
        Pending,
        Uploaded,
        Failed
    }
}
