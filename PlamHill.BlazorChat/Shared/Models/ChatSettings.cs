using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlamHill.BlazorChat.Shared.Models
{
    public class ChatSettings
    {
        [DefaultValue(0.7f)]
        public float Temperature { get; set; } = 0.7f;
        [DefaultValue(256)]
        public int MaxLength { get; set; } = 256;
        [DefaultValue(1)]
        public float TopP { get; set; } = 1;
        [DefaultValue(0)]
        public float FrequencyPenalty { get; set; } = 0;
        [DefaultValue(0)]
        public float PresencePenalty { get; set; } = 0;
    }
}
