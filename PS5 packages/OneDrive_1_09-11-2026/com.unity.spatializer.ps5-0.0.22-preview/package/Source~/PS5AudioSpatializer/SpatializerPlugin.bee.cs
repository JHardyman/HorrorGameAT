using System;
using System.Collections.Generic;
using System.Linq;
using Bee.Core;
using Bee.NativeProgramSupport;
using Bee.Toolchain.GNU;
using Bee.Toolchain.PS5;
using Bee.Toolchain.Windows;
using NiceIO;

class SpatializerPlugin
{
    static void Main()
    {
        using var _ = new Bee.Core.BuildProgramContext();

        var toolchains = GetToolchains();

        var codegens = new[] { CodeGen.Release, CodeGen.Debug };
        foreach (var codegen in codegens)
        {
            var program = new NativeProgram($"PS5AudioSpatializer")
            {
                Sources =
                {
                    "Plugin_Spatializer.cpp",
                    "Plugin_Spatializer_Mixer.cpp",
                    "AudioPluginUtil.cpp"
                }
            };

            program.RTTI.Set(false);
            program.Defines.Add(c => c.CodeGen == CodeGen.Debug, "_DEBUG");
            program.Defines.Add(c => c.Platform is PS5Platform,
                "PLATFORM_PLAYSTATION",
                "PLATFORM_ENDIAN_LITTLE",
                "PLATFORM_64BIT");

            program.Libraries.Add(c => c.Platform is PS5Platform , new SystemLibrary("SceAudioOut2_stub_weak"));

            program.CompilerSettingsForClang().Add(c => c.CodeGen != CodeGen.Debug && (c.Platform is PS5Platform || c.Platform is PS4Platform), c => c.WithLinkTimeOptimization(true));
            program.CompilerSettingsForClang().Add(c => (c.Platform is PS5Platform || c.Platform is PS4Platform), c => c.WithDebugMode(DebugMode.IncludeDebugSymbols));

            program.CompilerSettingsForClang().Add(s => s.WithWarningPolicies(new[] {
                new WarningAndPolicy("deprecated-register", WarningPolicy.Silent),
                new WarningAndPolicy("int-to-void-pointer-cast", WarningPolicy.Silent)
            }));

            foreach (var toolchain in toolchains)
            {
                var toolchainName = toolchain.ActionName.ToLowerInvariant();
                var cfg = new NativeProgramConfiguration(codegen, toolchain, true);

                NPath targetDir = $"_builds/unknown/{cfg.CodeGen}";
                if (toolchain is PS5Toolchain ps5Toolchain)
                    targetDir = $"_builds/ps5/{ps5Toolchain.SdkFolder}/{cfg.CodeGen}";

                var targetName = toolchainName;
                var target = program.SetupSpecificConfiguration(cfg, toolchain.DynamicLibraryFormat).DeployTo(targetDir);
                var targetAlias = $"{targetName}::{program.Name}";
                Backend.Current.AddAliasDependency(targetAlias, target.Path);
                Backend.Current.AddAliasToAliasDependency(targetName, targetAlias);

                Backend.Current.AddAliasToAliasDependency(toolchain.Platform.Name, targetAlias);    // allows us to use bee PLATFORMNAME to build everything for the platform
            }
        }

    }



    static IEnumerable<ToolChain> GetToolchains()
    {
        var toolchains = new ToolChain[]
        {
                    new WindowsToolchain(WindowsSdk.Locatorx64.UserDefaultOrLatest)
        }.Where(t => t != null);

        var ps5SdkArtifacts = PS5Artifacts.AddArtifactsFromManifestFile("ps5.manifest.stevedore") ?? PS5Artifacts.Artifacts;
        IEnumerable<ToolChain> ps5Toolchains = ps5SdkArtifacts.Select(artifact => new PS5Toolchain(new PS5Sdk(artifact.Value.SdkZip.Path, artifact.Value.SdkVersion, true)));
//        Console.WriteLine(string.Join("\n", ps5Toolchains.Select(t => $"{t.ActionName.ToLowerInvariant()} (SDK:{t.Sdk.Version}) {(t.CanBuild ? "" : " (can't build)")}")));
//        IEnumerable<ToolChain> toolchains = ps5Toolchains;

        toolchains = toolchains.Concat(ps5Toolchains);
        Console.WriteLine(string.Join("\n", toolchains.Select(t => $"{t.ActionName.ToLowerInvariant()} (SDK:{t.Sdk.Version}) {(t.CanBuild ? "" : " (can't build)")}")));

        return toolchains.Where(t => t.CanBuild);
    }

}

