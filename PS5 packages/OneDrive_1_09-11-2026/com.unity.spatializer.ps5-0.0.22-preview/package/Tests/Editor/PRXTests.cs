using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

public class PRXTests
{
    private const string k_PluginName = "PS5AudioSpatializer.prx";

    [Test, TestCaseSource(nameof(GetSpatializerPlugins))]
    public void PRXOnlyForPS5(PluginImporter plugin)
    {

        Assert.That(plugin.GetCompatibleWithAnyPlatform(), Is.False, $"{plugin.assetPath} should not be compatible with all platforms");

        foreach (BuildTarget target in Enum.GetValues(typeof(BuildTarget)))
        {
            //Skip PS5 as only valid platform
            if (target == BuildTarget.PS5)
            {
                Assert.That(plugin.GetCompatibleWithPlatform(target), Is.True, $"{plugin.assetPath} should be compatible with {target}");
                continue;
            }

            Assert.That(plugin.GetCompatibleWithPlatform(target), Is.False, $"{plugin.assetPath} should not be compatible with {target}");
        }
    }

    private static IEnumerable GetSpatializerPlugins()
    {
        var allPlugins = PluginImporter.GetAllImporters();
        foreach (var plugin in allPlugins)
        {
            if(plugin.assetPath.EndsWith(k_PluginName))
            {
                TestCaseData data;
                data = new TestCaseData(plugin).SetName(plugin.assetPath);
                yield return data;
            }
        }
    }
}
