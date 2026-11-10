using Unity.PlatformToolkit.Editor;

namespace Unity.PlatformToolkit.PS5.Editor
{
    /// <summary>Platform Toolkit settings for PS5.</summary>
    /// <remarks>Can be retrieved via <see cref="PlatformToolkitEditor.TryGetSettings{TSettings}"/></remarks>
    public interface IPS5Settings
    {
        /// <summary>Size of saves.</summary>
        /// <remarks>Every save created via <see cref="ISavingSystem.OpenSaveWritable"/> will be allocated to this size on PS5. There is no way currently to set the save size at runtime.</remarks>
        int AllocatedSaveSize { get; set; }

        /// <summary>Attribute settings.</summary>
        IAttributeSettings AttributeSettings { get; }
    }
}
