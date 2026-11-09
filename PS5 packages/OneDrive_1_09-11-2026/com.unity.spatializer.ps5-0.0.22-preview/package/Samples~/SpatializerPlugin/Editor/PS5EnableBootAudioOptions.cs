using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

#if UNITY_PS5
// allow per-project amendments to the final build
public class PS5EnableBootAudioOptions : UnityEditor.PS5.IPostProcessPS5
{
    public void OnPostProcessPS5(string projectFolder, string outputFolder)
    {
        System.Console.WriteLine($"PS5EnableBootAudioOptions script. projectFolder:{projectFolder} outputFolder:{outputFolder}");

        // this way works with the new incremental build system.
        Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.PostprocessBuildPlayer").GetMethod("AddProjectBootConfigKeyValue", BindingFlags.NonPublic | BindingFlags.Static).Invoke(null, new object[] { "audio-output-type", "AUDIOOUT2" });

        // Add custom lines to the boot.config file (pre inc-build system)... adding them to the end should allow you to override any earlier lines in the file
        var bootConfigPath = Path.Combine(outputFolder, "Data\\boot.config");
        System.Console.WriteLine($"checking for existence of boot config file at :{bootConfigPath}");
        if (File.Exists(bootConfigPath))
        {
            System.Console.WriteLine($"file found at :{bootConfigPath}");
            var lines = new List<string>(File.ReadAllLines(bootConfigPath));
            lines.Add("audio-output-type=AUDIOOUT2");
            File.WriteAllLines(bootConfigPath, lines.ToArray());
        }
        else
        {
            System.Console.WriteLine($"file not found at :{bootConfigPath}");
        }
    }
    public int callbackOrder { get { return 1; } }
}

#if UNITY_2022_1_OR_NEWER  // IPostProcessPS5PrepareStagingArea is not in 2021
public class  PS5EnableBootAudioOptions2 : UnityEditor.PS5.IPostProcessPS5PrepareStagingArea
{
    public bool OnPrepareStagingArea(string projectFolder, string outputFolder, UnityEditor.PS5.PS5BuildSubtarget buildSubtarget, UnityEditor.BuildOptions buildOptions)
    {
        System.Console.WriteLine($"PS5EnableBootAudioOptions OnPrepareStagingArea(). projectFolder:{projectFolder} outputFolder:{outputFolder}");
        return true;
    }

    public int callbackOrder { get { return 9999; } }
}
#endif
#endif
