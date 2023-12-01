using PalmHill.BlazorChat.Shared;

namespace PlamHill.BlazorChat.Client
{
    public class ModelResponse
    {
        public Guid PromptId { get; set; } = Guid.NewGuid();
        public string Prompt { get; set; } = string.Empty;
        public List<string> ResponseStrings { get; set; } = new List<string>();
        public bool IsComplete { get; set; } = false;
        public event EventHandler? ResponseChanged;
        public event EventHandler? ResponseCompleted;


        public string Resonse
        {
            get
            {

                var fullText = string.Join("", ResponseStrings);
                return fullText;
            }
        }

        public void AddResponseString(string responseString)
        {
            if (responseString == ChatExtensions.MESSAGE_END)
            {
                IsComplete = true;
                ResponseCompleted?.Invoke(this, EventArgs.Empty);
            }
            else 
            { 
                ResponseStrings.Add(responseString);
                ResponseChanged?.Invoke(this, EventArgs.Empty);
            }

        }

    }
}
