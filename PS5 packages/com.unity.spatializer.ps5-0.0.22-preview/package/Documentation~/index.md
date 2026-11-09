# PS5 spatializer plug-in

The PS5 Spatializer plug-in package contains a native audio plug-in to implement 3D object-based spatialized audio using Unity and PlayStation 5 (PS5).

<!-- This package provides offline HTML documentation. Open `HTML~/index.html` from the package directory to view it locally. -->

Use the Unity PS5 Spatializer plug-in to specify which audio sources to spatialize and send a portion of the audio from each source to a mixer channel where reverb can be applied. You can also use this plug-in to control the global volume of the spatialized sound.

The Unity PS5 Spatializer plug-in uses the [Unity Audio Spatializer SDK](https://docs.unity3d.com/Manual/AudioSpatializerSDK.html).

Refer to [Sony DevNet](https://game.develop.playstation.net/resources/documents/SDK/latest/AudioOut2-3DAudioConcepts/__toc.html) for more information on PS5 3D audio.

## Spatialization

Placing a sound in a 3D audio environment is called spatialization. It's comprised of two main components:

* **Directionality** - How the listener perceives direction inside a space.
* **Acoustics** - How the sound interacts with objects within a space.

Use spatialized audio in your title to let your player experience a greater sense of presence. It also lets them locate the source of sounds more accurately. For example, the player can use 3D audio to assist in locating approaching footsteps from behind, or the noise of something lurking in the distance. You can also use 3D audio to create increasingly complex soundscapes for the player, immersing them in your designed world.

## Using the Spatializer plug-in

You must run your project on a PS5 device to hear the full effect of the spatializer plug-in. While the editor version of this plug-in produces audio, it isn't an accurate reflection of the spatialization and audio routing of an actual PS5 device. Some configurations may produce a different output in the editor than on a PS5 device. For more information, refer to the [best practices and considerations](bestpractices.md) page.

|**Topic** |**Description** |
|:---|:---|
|[**Getting started**](gettingstarted.md)|Describes the steps to use the spatializer plug-in with your title.|
|[**Using the sample project**](sample.md)|Explains the included sample projects and how to navigate them.|
|[**Best practices and considerations**](bestpractices.md)|Describes best practices and any considerations to take when using the Unity PS5 spatializer plug-in.|
