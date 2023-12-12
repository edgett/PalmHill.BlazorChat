using PalmHill.BlazorChat.Shared.Models;

namespace PalmHill.BlazorChat.Client.Models
{
    public class LocalStorageSettings
    {
        public InferenceSettings InferenceSettings { get; set; } = new InferenceSettings();
        public bool DarkMode { get; set; } = false;
        public string SystemMessage { get; set; } = "You are a helpful assistant.";
        public int SettingsVersion { get; set; } = CurrentSettingsVersion;
        public const int CurrentSettingsVersion = 1;
    }
}
