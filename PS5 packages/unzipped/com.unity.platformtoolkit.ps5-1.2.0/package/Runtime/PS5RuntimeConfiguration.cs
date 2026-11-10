namespace Unity.PlatformToolkit.PS5
{
    internal class PS5RuntimeConfiguration : BaseRuntimeConfiguration
    {
        public AchievementDefinitionWithNativeId<int>[] AchievementDefinitions;
        public int SaveSizeInKib;
        public AttributeStore AttributeStore;

        public override IPlatformToolkit InstantiatePlatformToolkit()
        {
            return new PS5PlatformToolkit(this);
        }
    }
}
