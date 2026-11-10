using UnityEditor;
using UnityEngine;

public class EditorScript
{

    [InitializeOnLoad]
    public class Startup {
        static Startup()
        {
#if !INPUTSYSTEM_PS5_PACKAGE_DETECTED
            Debug.LogError("No PS5 input package detected. Use package manager \"Add Package from tarball...\" option to add com.unity.inputsystem.ps5.tgz");
#endif
 #if !CINEMACHINE_PACKAGE_DETECTED
            Debug.LogError("No Cinemachine package detected. Use package manager to add Cinemachine package");
#endif
            if(AudioSettings.GetSpatializerPluginName() == string.Empty)
            {
                Debug.LogWarning("No spatializer plugin enabled. Set PlayerSettings->Audio->SpatializerPlugin to \"PS5AudioSpatializer\"");
            }
        }
    }



    [MenuItem("Tools/Spatializer Sample Configuration Test")]
    public static void Execute()
    {
#if !INPUTSYSTEM_PS5_PACKAGE_DETECTED
        Debug.LogError("No PS5 input package detected. Use package manager \"Add Package from tarball...\" option to add com.unity.inputsystem.ps5.tgz");
#endif
#if !CINEMACHINE_PACKAGE_DETECTED
        Debug.LogError("No Cinemachine package detected. Use package manager to add Cinemachine package");
#endif
#if INPUTSYSTEM_PS5_PACKAGE_DETECTED && CINEMACHINE_PACKAGE_DETECTED
        Debug.Log("All packages detected");
#endif
        if(AudioSettings.GetSpatializerPluginName() == string.Empty)
        {
            Debug.LogWarning("No spatializer plugin enabled. Set PlayerSettings->Audio->SpatializerPlugin to \"PS5AudioSpatializer\"");
        }

    }


}
