namespace PlamHill.BlazorChat.Client
{
    public class ModelResponse
    {
        public Guid PromptId { get; set; } = Guid.NewGuid();
        public string Prompt { get; set; } = string.Empty;
        public List<string> ResponseStrings { get; set; } = new List<string>();
        public string Resonse
        {
            get
            {

                var fullText = string.Join("", ResponseStrings);
                return fullText;
            }
        }

    }
}
