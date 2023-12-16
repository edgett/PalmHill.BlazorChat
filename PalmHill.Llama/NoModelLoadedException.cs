using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PalmHill.Llama
{
    public class NoModelLoadedException : InvalidOperationException
    {
        public NoModelLoadedException() : base()
        {
            base.Data.Add("ModelProvider", "No model has been loaded.");
        }

        public NoModelLoadedException(string message) : base(message)
        {
            base.Data.Add("ModelProvider", "No model has been loaded.");
        }

        public NoModelLoadedException(string message, Exception innerException) : base(message, innerException)
        {
            base.Data.Add("ModelProvider", "No model has been loaded.");
        }

    }
}
