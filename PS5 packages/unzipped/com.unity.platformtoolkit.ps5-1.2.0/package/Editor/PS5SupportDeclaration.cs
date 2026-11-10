using System.Collections.Generic;
using Unity.PlatformToolkit.Editor;
using UnityEditor;

#if UNITY_6000_4_OR_NEWER
using GUID = UnityEngine.GUID;
#endif
#if UNITY_6000_5_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif

namespace Unity.PlatformToolkit.PS5.Editor
{
#if UNITY_6000_5_OR_NEWER
    [NoAutoStaticsCleanup]
#endif
    internal class PS5SupportDeclaration : IPlatformToolkitSupportDeclaration
    {
        private static readonly PS5SettingsProvider s_Provider = new PS5SettingsProvider();

        private static readonly BuildTarget[] k_SupportedBuildTargets = new[]
        {
            BuildTarget.PS5
        };

        public string DisplayName => "PlayStation®5";
        public string Key => PS5PlatformToolkit.Key;

        public IReadOnlyCollection<BuildTarget> SupportedPlatforms => k_SupportedBuildTargets;

        private static readonly GUID[] k_SupportedBuildProfileGuids = new[]
        {
            new GUID("e30a9c34166844499a56eaa4ed115c44"), // PlayStation®5
        };
        public IReadOnlyCollection<GUID> SupportedBuildProfileGuids => k_SupportedBuildProfileGuids;

        public IPlatformToolkitSettingsProvider SettingsProvider => s_Provider;

        public IPlatformToolkitBuilder CreateBuilder(IAchievementConfigurationContext achievementContext = null,
            ISettingsConfigurationContext settingsContext = null)
        {
            var settings = (PS5Settings)SettingsProvider.CreateSettingsConfiguration(settingsContext).Settings;
            return new PS5Builder(achievementContext, settings);
        }

        public bool AchievementsSupported => true;
        public IAchievementConfiguration CreateAchievementConfiguration(IAchievementConfigurationContext context)
        {
            return new AchievementConfigurationWithStringId(context, DisplayName, Key);
        }
    }
}
