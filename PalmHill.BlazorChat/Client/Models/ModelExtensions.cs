using PalmHill.BlazorChat.Shared.Models;

namespace PalmHill.BlazorChat.Client.Models
{
    public static class ModelExtensions
    {
        public static LocalStorageSettings CreateCopy(this LocalStorageSettings localStorageSettings)
        { 
            var copy = new LocalStorageSettings();
            copy.DarkMode = localStorageSettings.DarkMode;
            copy.InferenceSettings = localStorageSettings.InferenceSettings.CreateCopy();
            copy.SettingsVersion = localStorageSettings.SettingsVersion;
            copy.SystemMessage = localStorageSettings.SystemMessage;
            return copy;
        }

        public static InferenceSettings CreateCopy(this InferenceSettings inferenceSettings)
        { 
            var copy = new InferenceSettings();
            copy.MaxLength = inferenceSettings.MaxLength;
            copy.PresencePenalty = inferenceSettings.PresencePenalty;
            copy.Temperature = inferenceSettings.Temperature;
            copy.TopP = inferenceSettings.TopP;
            copy.FrequencyPenalty = inferenceSettings.FrequencyPenalty;
            return copy;

        }

    }
}
