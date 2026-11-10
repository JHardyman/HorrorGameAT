using System;
using Unity.PlatformToolkit.Editor;

namespace Unity.PlatformToolkit.PS5.Editor
{
    internal class PS5SettingsProvider : IPlatformToolkitSettingsProvider
    {
        public Type SettingsType => typeof(IPS5Settings);

        public ISettingsConfiguration CreateSettingsConfiguration(ISettingsConfigurationContext context)
        {
            var settingsConfiguration = new PS5SettingsConfiguration(context);
            return settingsConfiguration;
        }
    }
}
