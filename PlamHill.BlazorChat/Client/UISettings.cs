using PlamHill.BlazorChat.Shared.Models;

namespace PlamHill.BlazorChat.Client
{
    public class UISettings : InferenceSettings
    {
        public string SystemMessage { get; set; } = "You are a helpful assistant. Repsond in valid markdown only.";
    }
}
