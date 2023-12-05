using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlamHill.BlazorChat.Shared.Models
{
    /// <summary>
    /// Represents the settings for inference.
    /// </summary>
    /// <summary>
    /// Represents the settings for inference with a large language model.
    /// </summary>
    public class InferenceSettings
    {
        /// <summary>
        /// Gets or sets the temperature for the inference.
        /// Higher values (closer to 1) make the output more random, while lower values make the output more deterministic.
        /// </summary>
        /// <value>
        /// The temperature for the inference. Default value is 0.7f.
        /// </value>
        [DefaultValue(0.7f)]
        public float Temperature { get; set; } = 0.7f;

        /// <summary>
        /// Gets or sets the maximum length for the inference.
        /// This is the maximum number of tokens that the model will generate.
        /// </summary>
        /// <value>
        /// The maximum length for the inference. Default value is 256.
        /// </value>
        [DefaultValue(256)]
        public int MaxLength { get; set; } = 256;

        /// <summary>
        /// Gets or sets the top P for the inference.
        /// This is the nucleus sampling parameter that controls the size of the token set considered for generation at each step.
        /// Lower values make the output more focused, while higher values make it more diverse.
        /// </summary>
        /// <value>
        /// The top P for the inference. Default value is 1.
        /// </value>
        [DefaultValue(1)]
        public float TopP { get; set; } = 1;

        /// <summary>
        /// Gets or sets the frequency penalty for the inference.
        /// Higher values discourage the model from using frequent tokens, while lower values make no change to the model's preference.
        /// </summary>
        /// <value>
        /// The frequency penalty for the inference. Default value is 0.
        /// </value>
        [DefaultValue(0)]
        public float FrequencyPenalty { get; set; } = 0;

        /// <summary>
        /// Gets or sets the presence penalty for the inference.
        /// Higher values discourage the model from using new tokens, while lower values make no change to the model's preference.
        /// </summary>
        /// <value>
        /// The presence penalty for the inference. Default value is 0.
        /// </value>
        [DefaultValue(0)]
        public float PresencePenalty { get; set; } = 0;
    }
}
