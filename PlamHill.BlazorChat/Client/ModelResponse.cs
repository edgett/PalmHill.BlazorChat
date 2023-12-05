using System;
using System.Collections.Generic;

namespace PlamHill.BlazorChat.Client
{
    /// <summary>
    /// Represents a response from the chat model.
    /// </summary>
    public class ModelResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier for the prompt.
        /// </summary>
        public Guid PromptId { get; set; } = Guid.NewGuid();

        /// <summary>
        /// Gets or sets the prompt text.
        /// </summary>
        public string Prompt { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the list of response strings.
        /// </summary>
        public List<string> ResponseStrings { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets a value indicating whether the response is complete.
        /// </summary>
        public bool IsComplete { get; set; } = false;

        /// <summary>
        /// Occurs when the response changes.
        /// </summary>
        public event EventHandler? ResponseChanged;

        /// <summary>
        /// Occurs when the response is completed.
        /// </summary>
        public event EventHandler? ResponseCompleted;

        /// <summary>
        /// Gets the full response text.
        /// </summary>
        public string Resonse
        {
            get
            {
                var fullText = string.Join("", ResponseStrings);
                return fullText;
            }
        }

        /// <summary>
        /// Adds a response string to the <see cref="ResponseStrings"/> list and raises the <see cref="ResponseChanged"/> event.
        /// </summary>
        /// <param name="responseString">The response string to add.</param>
        public void AddResponseString(string responseString)
        {
            ResponseStrings.Add(responseString);
            ResponseChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Marks the response as complete and raises the <see cref="ResponseCompleted"/> event.
        /// </summary>
        public void CompleteResponse()
        {
            IsComplete = true;
            ResponseCompleted?.Invoke(this, EventArgs.Empty);
        }
    }
}