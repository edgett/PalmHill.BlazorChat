using Microsoft.AspNetCore.Components;
using Microsoft.FluentUI.AspNetCore.Components.DesignTokens;

namespace PlamHill.BlazorChat.Client
{
    public class ThemeControl
    {

        public ThemeControl(BaseLayerLuminance baseLayerLuminance, AccentBaseColor accentBaseColor) 
        { 
            BaseLayerLuminance = baseLayerLuminance;
            AccentBaseColor = accentBaseColor;
        }

        private BaseLayerLuminance? BaseLayerLuminance { get; set; }

        private AccentBaseColor? AccentBaseColor { get; set; }


        public async Task ChangeTheme(bool darkMode)
        {

            if (darkMode)
            {
                await BaseLayerLuminance!.WithDefault(0f);
                await AccentBaseColor!.WithDefault(new Swatch(255, 105, 180));

            }
            else
            {
                await BaseLayerLuminance!.WithDefault(1f);
                await AccentBaseColor!.WithDefault(new Swatch(255, 105, 180));
            }

        }

    }
}
