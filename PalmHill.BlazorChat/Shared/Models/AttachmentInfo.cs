﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalmHill.BlazorChat.Shared.Models
{
    public class AttachmentInfo
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = "";
        public string Url { get; set; } = "";
        public string ContentType { get; set; } = "";
        public long Size { get; set; } = 0;
        public AttachmentStatus Status { get; set; } = AttachmentStatus.Pending;
    }

    public enum AttachmentStatus
    {
        Pending,
        Uploaded,
        Failed
    }
}