using PalmHill.BlazorChat.Shared.Models;

namespace PalmHill.BlazorChat.Client.Models
{
    public static class ModelExtensions
    {
        /// <summary>
        /// Creates a copy of the <see cref="LocalStorageSettings"/> object.
        /// </summary>
        /// <param name="localStorageSettings">The object to copy.</param>
        /// <returns>A new instance of <see cref="LocalStorageSettings"/> with values from <paramref name="localStorageSettings"/>.</returns>
        public static LocalStorageSettings CreateCopy(this LocalStorageSettings localStorageSettings)
        {
            var copy = new LocalStorageSettings();
            copy.DarkMode = localStorageSettings.DarkMode;
            copy.InferenceSettings = localStorageSettings.InferenceSettings.CreateCopy();
            copy.SettingsVersion = localStorageSettings.SettingsVersion;
            copy.SystemMessage = localStorageSettings.SystemMessage;
            return copy;
        }

        /// <summary>
        /// Creates a copy of the <see cref="InferenceSettings"/> object.
        /// </summary>
        /// <param name="inferenceSettings">The object top copy.</param>
        /// <returns>A new instance of <see cref="InferenceSettings"/> with values from <paramref name="inferenceSettings"/>.</returns>
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
