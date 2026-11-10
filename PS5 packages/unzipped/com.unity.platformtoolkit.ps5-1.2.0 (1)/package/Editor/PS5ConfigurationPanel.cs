using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.PlatformToolkit.PS5.Editor
{
    internal class PS5ConfigurationPanel : VisualElement
    {
        public PS5ConfigurationPanel(PS5Settings settings)
        {
            var uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.unity.platformtoolkit.ps5/Editor Resources/UI/PS5Settings.uxml");
            uxml.CloneTree(this);
            dataSource = settings;
        }
    }
}
