
# About PS5 Spatializer plugin package

This package contains a native audio plugin that can be used to implement 3D object based audio using the PS5.
More details about the Unity native spatializer audio plugin can be found [here](https://docs.unity3d.com/Manual/AudioSpatializerSDK.html)
Further details about the PS5 3D audio can be found [here](https://game.develop.playstation.net/resources/documents/SDK/latest/AudioOut2-3DAudioConcepts/__toc.html)

# Installing PS5 Spatializer plugin package

This package needs to be installed using the "Add package from tarball..." option in the package manager.


# Configuring the PS5 Spatializer plugin

In the PlayerSettings->Audio->Spatializer Plugin, select the "PS5AudioSpatializer" dropdown option.
The PS5 native plugin (PS5AudioSpatializer) requires the "Load on startup" tickbox to be enabled, this should be enabled by default.
All AudioSources to be played using the spatializer plugin are required to have their outputAudioMixerGroup property assigned to a mixer group that has the "PS5SpatializerMixer" effect enabled (see PS5SpatializerMixer.mixer in the package).

# Technical details
## Requirements

This version of PS5 Spatializer plugin package is compatible with the following versions of the Unity Editor:

* 2020.3 and later
