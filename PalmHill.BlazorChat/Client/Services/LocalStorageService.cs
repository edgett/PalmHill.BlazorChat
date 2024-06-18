using Blazored.LocalStorage;
using PalmHill.BlazorChat.Client.Models;

namespace PalmHill.BlazorChat.Client.Services
{
    /// <summary>
    /// Service to handle local storage.
    /// </summary>
    public class LocalStorageService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly ThemeService _themeControl;

        /// <summary>
        /// Use dependency injection to get the local storage service and the theme service.
        /// </summary>
        /// <param name="localStorage"></param>
        /// <param name="themeControl">Used to change the theme on settings get.</param>
        public LocalStorageService(ILocalStorageService localStorage, ThemeService themeControl)
        {
            _localStorage = localStorage;
            _themeControl = themeControl;
        }

        /// <summary>
        /// In memory settings.
        /// </summary>
        public LocalStorageSettings LocalStorageSettings { get; private set; } = new LocalStorageSettings();

        /// <summary>
        /// Get the settings from local storage.
        /// </summary>
        /// <returns></returns>
        public async Task<LocalStorageSettings> GetSettings()
        {
            LocalStorageSettings = await _getMigratedSettings();
            return LocalStorageSettings;
        }

        public async Task SyncTheme()
        {
            await _themeControl.ChangeTheme(LocalStorageSettings.DarkMode);
        }

        /// <summary>
        /// Save the settings to local storage.
        /// </summary>
        /// <param name="localStorageSettings"></param>
        /// <returns></returns>
        public async Task SaveSettings(LocalStorageSettings localStorageSettings)
        {
            await _localStorage.SetItemAsync("LocalStorageSettings", localStorageSettings);
        }

        /// <summary>
        /// Gets the settings from local storage and migrates them if necessary.
        /// </summary>
        private async Task<LocalStorageSettings> _getMigratedSettings()
        {
            var settingsExist = await _localStorage.ContainKeyAsync("LocalStorageSettings");

            if (settingsExist)
            {
                var localStorageSettings = await _localStorage.GetItemAsync<LocalStorageSettings>("LocalStorageSettings");

                if (localStorageSettings?.SettingsVersion == LocalStorageSettings.CurrentSettingsVersion)
                {
                    return localStorageSettings;
                }
                else
                {
                    //TODO: Migrate settings
                    return new LocalStorageSettings();
                }
            }

            return new LocalStorageSettings();
        }
    }
}
