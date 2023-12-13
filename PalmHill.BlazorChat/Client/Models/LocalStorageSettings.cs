using PalmHill.BlazorChat.Shared.Models;

namespace PalmHill.BlazorChat.Client.Models
{
    /// <summary>
    /// Settings that are stored in the browser's local storage.
    /// </summary>
    public class LocalStorageSettings
    {
        /// <summary>
        /// Parameters for the inference request.
        /// </summary>
        public InferenceSettings InferenceSettings { get; set; } = new InferenceSettings();
        
        /// <summary>
        /// System message for the inference request.
        /// </summary>
        public string SystemMessage { get; set; } = "You are a helpful assistant.";

        /// <summary>
        /// Dark mode.
        /// </summary>
        public bool DarkMode { get; set; } = false;

        /// <summary>
        /// Settings version. Used to control migration of settings when there is a new version.
        /// </summary>
        public int SettingsVersion { get; set; } = CurrentSettingsVersion;

        public const int CurrentSettingsVersion = 1;
    }
}
