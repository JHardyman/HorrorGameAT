using Unity.PlatformToolkit.Editor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Unity.PlatformToolkit.PS5.Editor
{
    internal class PS5Builder : IPlatformToolkitBuilder
    {
        private IAchievementConfigurationContext m_AchievementContext;
        private PS5Settings m_Settings;

        public PS5Builder(IAchievementConfigurationContext achievementContext, PS5Settings settings)
        {
            m_AchievementContext = achievementContext;
            m_Settings = settings;
        }

        public BaseRuntimeConfiguration PrepareBuild(BuildReport buildReport)
        {
#if UNITY_PS5
            var runtimeConfiguration = ScriptableObject.CreateInstance<PS5RuntimeConfiguration>();
            runtimeConfiguration.AchievementDefinitions = AchievementConfigurationWithStringId.BuildForRuntimeWithIntIds(m_AchievementContext);
            runtimeConfiguration.SaveSizeInKib = m_Settings.AllocatedSaveSize;
            runtimeConfiguration.AttributeStore = m_Settings.BuildAttributes();
            return runtimeConfiguration;
#else
            Debug.LogWarning($"{nameof(PrepareBuild)} called when UNITY_PS5 is not defined. This will result in PlatformToolkit for PS5 failing to initialize as a runtime configuration will not exist.");
            return null;
#endif
        }

        public void PostBuild(BuildReport buildReport) { }
    }
}
