using PalmHill.BlazorChat.Shared.Models;

namespace PalmHill.BlazorChat.Client
{
    public class UISettings : InferenceSettings
    {
        public string SystemMessage { get; set; } = "You are a helpful assistant.";
        public bool DarkMode { get; set; } = false;
    }
}
