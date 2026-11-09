# Vibration

Details the types of vibration that can be used with the DualSense® Wireless controller and how to use them in your project.

## Vibration types

There are three types of vibration that can be used with the DualSense® Wireless controller:

* **Advanced** vibration is specific to the DualSense® Wireless controller and uses Audio data to create vibration. 
* **Compatible2** and **Compatible** vibration emulates the vibration of a DualShock® 4 controller on the DualSense® Wireless controller. Compatible creates simple vibration effects based on sine waves, whereas Compatible2 creates effects that feel closer to those of a DualShock® 4 controller. This can be useful if you are remastering a PS4 title for PS5. Use `Gamepad.SetMotorSpeeds` with **Compatible2** and **Compatible** modes.

For a full overview of the vibration feature, refer to [Vibration Feature Overview](https://game.develop.playstation.net/resources/documents/SDK/latest/Pad-Overview/using-the-vibration-feature.html) (PlayStation®5 DevNet).


### Set the vibration type
To set the vibration mode to use in your project for each controller, use `UnityEngine.InputSystem.PS5.VibrationMode.

```csharp
//Set the vibration mode to Compatible on the Gamepad at slot 0
var pad = DualSenseGamepad.GetBySlotIndex(0);
pad.SetVibrationMode(VibrationMode.Compatible);
```
> [!NOTE]
> When working with vibration, **Advanced** vibration is the default option.

## Using Audio sources

When using the **Advanced** vibration mode, vibration data can be created from audio data (PCM data). When creating this audio data, the valid range for vibration data is `0` to `500` Hz. You can also use the **Vibration Designer** to create and edit your audio sources. For more information, refer to the [Vibration Designer User's Guide](https://game.develop.playstation.net/resources/documents/SDK/latest/Vibration_Designer-Users_Guide/__document_toc.html) (PlayStation®5 DevNet).

Audio examples for use as vibration data can be found in the [sample project](sample-project.md).

## Use advanced vibration

To use vibration in your project, use the following steps:

1) Select the controller you wish to target.
1) Set the vibration type to use.
1) Create or choose an AudioSource to use for the vibration if using advanced mode.
1) Set the gamepad audio output type for the AudioSource as **Vibration** with the property `AudioSource.GamepadSpeakerOutputType`.
1) Play the AudioSource to the controller with the declaration `AudioSource.PlayOnGamepad`.

Your script might look like the following:

```csharp
public class AdvancedAudioHapticsExample : MonoBehaviour
    {
        [SerializeField] AudioSource vibrationSource;
        DualSenseGamepad m_Gamepad;

        void Awake()
        {
            m_Gamepad = DualSenseGamepad.GetBySlotIndex(0);

            //Vibration mode only needs to be set once
            m_Gamepad.SetVibrationMode(VibrationMode.Advanced);

            vibrationSource.gamepadSpeakerOutputType = GamepadSpeakerOutputType.Vibration;
            
            //panStereo will effect the motor left (-1f) or right (1f) that will play the audio effect
            //0 will play on both motors equally
            vibrationSource.panStereo = 0f;
        }

        void Start() //or some other function to trigger the effect
        {
            vibrationSource.PlayOnGamepad(m_Gamepad.slotIndex);
        }
    }
```

## Use compatible vibration

To use compatible vibration in your project, use the following steps:

1) Select the controller you wish to target.
1) Set the vibration type to use.
1) Use `Gamepad.SetMotorSpeeds` to set the frequency strength of each motor.

Your script might look like the following:

```csharp
    public class BasicHapticsExample : MonoBehaviour
    {
        DualSenseGamepad m_Gamepad;

        void Awake()
        {
            m_Gamepad = DualSenseGamepad.GetBySlotIndex(0);

            //Vibration mode only needs to be set once
            m_Gamepad.SetVibrationMode(VibrationMode.Compatible /* or Compatible2*/);
        }

        void Start() //or some other function to trigger the effect
        {
            m_Gamepad.SetMotorSpeeds(1f, 1f);
        }
    }
```

## Additional resources

* [Vibration Feature Overview](https://game.develop.playstation.net/resources/documents/SDK/latest/Pad-Overview/using-the-vibration-feature.html)
* [Trigger Effects](trigger-effects.md)



