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
            if (responseString.EndsWith(ChatExtensions.MESSAGE_END))
            {
                IsComplete = true;
                var stringToAdd = responseString.Substring(0, responseString.Length - ChatExtensions.MESSAGE_END.Length);

                if (stringToAdd.Length > 0)
                { 
                    ResponseStrings.Add(stringToAdd);
                    ResponseChanged?.Invoke(this, EventArgs.Empty);
                }

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
