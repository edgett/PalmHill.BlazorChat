using Blazored.LocalStorage;
using Markdig.Syntax.Inlines;
using PalmHill.BlazorChat.Client.Models;

namespace PalmHill.BlazorChat.Client.Services
{
    public class LocalStorageService
    {
        private ILocalStorageService _localStorage;
        private ThemeControl _themeControl;

        public LocalStorageService(ILocalStorageService localStorage, ThemeControl themeControl)
        {
            _localStorage = localStorage;
            _themeControl = themeControl;
        }

        public LocalStorageSettings LocalStorageSettings { get; private set; } = new LocalStorageSettings();

        public async Task<LocalStorageSettings> GetSettings()
        {
            LocalStorageSettings = await GetStorageSettings();
            await _themeControl.ChangeTheme(LocalStorageSettings.DarkMode);
            return LocalStorageSettings;
        }

        public async Task SaveSettings(LocalStorageSettings localStorageSettings)
        {
            await _localStorage.SetItemAsync("LocalStorageSettings", localStorageSettings);
        }

        private async Task<LocalStorageSettings> GetStorageSettings()
        {
            var settingsExist = await _localStorage.ContainKeyAsync("LocalStorageSettings");

            if (settingsExist)
            {
                var localStorageSettings = await _localStorage.GetItemAsync<LocalStorageSettings>("LocalStorageSettings");

                if (localStorageSettings.SettingsVersion == LocalStorageSettings.CurrentSettingsVersion)
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
