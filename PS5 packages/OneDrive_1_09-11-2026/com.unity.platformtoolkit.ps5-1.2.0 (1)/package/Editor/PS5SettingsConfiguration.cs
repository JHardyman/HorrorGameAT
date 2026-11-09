using Unity.PlatformToolkit.Editor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.PlatformToolkit.PS5.Editor
{
    internal class PS5SettingsConfiguration : ISettingsConfiguration
    {
        private ISettingsConfigurationContext m_Context;
        private PS5Settings m_Settings;
        private VisualTreeAsset m_ViewAsset;

        public object Settings => m_Settings;

        public PS5SettingsConfiguration(ISettingsConfigurationContext context)
        {
            m_Context = context;
            if (m_Context.TryGetSerializedSettings(out var serializedSettings))
                m_Settings = JsonUtility.FromJson<PS5Settings>(serializedSettings);
            else
                m_Settings = new PS5Settings();

            m_Settings.propertyChanged += (_, _) => SettingsChanged();
            m_Settings.SettingsChanged += SettingsChanged;
        }

        private void SettingsChanged()
        {
            m_Context.SetSerializedSettings(JsonUtility.ToJson(m_Settings));
        }

        public void CreateSettingsUI(VisualElement settingsContainer, VisualElement attributeContainer)
        {
            settingsContainer.Add(new PS5ConfigurationPanel(m_Settings));
            attributeContainer.Add(new AttributeSettingsField(m_Settings));
        }
    }
}
