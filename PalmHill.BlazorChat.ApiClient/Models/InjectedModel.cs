using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalmHill.BlazorChat.ApiClient.Models
{
    public class InjectedModel
    {
        public ModelParams? ModelParams { get; set; }
        public ModelConfig? LoadConfig { get; }
    }
}
