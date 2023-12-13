using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components.DesignTokens;

namespace PalmHill.BlazorChat.Client.Services
{

    /// <summary>
    /// Service to handle theme changes.
    /// </summary>
    public class ThemeService
    {
        /// <summary>
        /// For DI.
        /// </summary>
        /// <param name="baseLayerLuminance">From FluentUI</param>
        /// <param name="accentBaseColor">From FluentUI</param>
        public ThemeService(BaseLayerLuminance baseLayerLuminance, AccentBaseColor accentBaseColor)
        {
            _baseLayerLuminance = baseLayerLuminance;
            _accentBaseColor = accentBaseColor;
        }

        private BaseLayerLuminance? _baseLayerLuminance { get; }

        private AccentBaseColor? _accentBaseColor { get;  }

        /// <summary>
        /// Changes the theme.
        /// </summary>
        /// <param name="darkMode">Dark Mode</param>
        public async Task ChangeTheme(bool darkMode)
        {

            if (darkMode)
            {
                await _baseLayerLuminance!.WithDefault(0f);
                await _accentBaseColor!.WithDefault(new Swatch(255, 105, 180));

            }
            else
            {
                await _baseLayerLuminance!.WithDefault(1f);
                await _accentBaseColor!.WithDefault(new Swatch(255, 105, 180));
            }

        }

    }
}
