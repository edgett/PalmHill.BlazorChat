using LLama;
using LLama.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalmHill.Llama.Models
{
    public class InjectedModel
    {
        public LLamaWeights Model { get; }
        public ModelParams ModelParams { get;  }
        public string[] DefaultAntiPrompts { get; set; }

        public InjectedModel(LLamaWeights model, ModelParams modelParams, string[] defaultAntiPrompts)
        {
            Model = model;
            ModelParams = modelParams;
            DefaultAntiPrompts = defaultAntiPrompts;
        }
    }
}
