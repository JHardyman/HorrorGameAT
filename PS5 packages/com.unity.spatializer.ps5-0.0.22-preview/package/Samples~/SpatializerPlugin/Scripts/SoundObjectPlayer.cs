using UnityEngine;
#if INPUTSYSTEM_PS5_PACKAGE_DETECTED || UNITY_PS5
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System;
using UnityEngine.Audio;

// In the sample code this object is attached as a component of the PlayerCapsule
public class SoundObjectPlayer : MonoBehaviour
{
    public GameObject soundObjectPrefab;

    static internal bool spatialize = true;
    static internal bool spatializePostEffects = false;
    static internal float spatialBlend = 1.0f;

    static internal float globalGain = 1.0f;

    static private bool InputInitialised = false;
    static internal bool mixerSfxReverb = false;

    public AudioMixerSnapshot mixerSnapshotReverbEnabled;
    public AudioMixerSnapshot mixerSnapshotReverbDisabled;

    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        if (InputInitialised==false)
        {
            InputInit();
        }
#else
        // ps5 can sometime start without gamepad, especially on devkits with remote viewer, so we initialise the input only when a gamepad has been added
        InputSystem.onDeviceChange +=
            (device, change) =>
            {
                switch (change)
                {
                    case InputDeviceChange.Added:
                        Debug.Log($"New device added: {device}  isgamepad:{device is Gamepad} " );
                        // specifically for ps5 when we don't have a controller initialized at the start
                        if ((device is Gamepad) && (InputInitialised==false))
                        {
                            InputInit();
                        }
                        break;

                    case InputDeviceChange.Removed:
                        Debug.Log("Device removed: " + device);
                        break;
                }
            };
#endif

    }

    void InputInit()
    {
        GameObject sheep;

        Queue<GameObject> liveSheep = new Queue<GameObject>();
        var actionMap = new InputActionMap("SourceObjectInputActionMap");

        System.Console.WriteLine($"SoundObjectPlayer::Start() InputActionMap created\n");

        // e.g. buttonSouth, dpadSouth, triggerLeft, shoulderLeft,
        var delaction = actionMap.AddAction("delSoundObject", binding: "<Gamepad>/leftShoulder");
        var addaction = actionMap.AddAction("addSoundObject", binding: "<Gamepad>/rightShoulder");
        var decreaseSpatialBlend = actionMap.AddAction("decreaseSpatialBlend", binding: "<Gamepad>/dpad/left");
        var increaseSpatialBlend = actionMap.AddAction("increaseSpatialBlend", binding: "<Gamepad>/dpad/right");
        var toggleSpatialize = actionMap.AddAction("toggleSpatialize", binding: "<Gamepad>/buttonNorth");
        var toggleSpatializePostEffects = actionMap.AddAction("toggleSpatializePostEffects", binding: "<Gamepad>/buttonSouth");
        var toggleAudioListenerVolume = actionMap.AddAction("toggleAudioListenerVolume", binding: "<Gamepad>/buttonWest");
        var toggleMixerSfxReverb = actionMap.AddAction("toggleMixerSfxReverb", binding: "<Gamepad>/buttonEast");
        var decreaseGlobalGain = actionMap.AddAction("decreaseGlobalGain", binding: "<Gamepad>/dpad/down");
        var increaseGlobalGain = actionMap.AddAction("increaseGlobalGain", binding: "<Gamepad>/dpad/up");

        addaction.AddBinding("<Keyboard>/1");   // for editor
        delaction.AddBinding("<Keyboard>/2");   // for editor
        toggleAudioListenerVolume.AddBinding("<Keyboard>/3");   // for editor
        toggleMixerSfxReverb.AddBinding("<Keyboard>/4");   // for editor
        addaction.Enable();
        delaction.Enable();
        toggleSpatialize.Enable();
        increaseSpatialBlend.Enable();
        decreaseSpatialBlend.Enable();
        decreaseGlobalGain.Enable();
        increaseGlobalGain.Enable();
        toggleSpatializePostEffects.Enable();
        toggleAudioListenerVolume.Enable();
        toggleMixerSfxReverb.Enable();


        // Debug.LogError($"SoundObjectPlayer::Start() input action created.\n");
        // foreach( var a in actionMap.actions)
        //     Debug.LogError($"action:{a.ToString()}");

        actionMap.actionTriggered +=
            context =>
            {
                System.Console.WriteLine($"SoundObjectPlayer::Start() action triggered:{context.action.name}");
                if (context.performed)
                {
                    switch (context.action.name)
                    {
                        case "addSoundObject":
                            sheep = Instantiate(soundObjectPrefab, new Vector3(UnityEngine.Random.Range(-10.0f, 10.0f),5.0f,UnityEngine.Random.Range(0.0f, 20.0f)), Quaternion.identity);
                            liveSheep.Enqueue(sheep);
                            Debug.LogError($"sheepies summoned:{liveSheep.Count}");
                            break;

                        case "delSoundObject":
                            {
                                var exSheepie = liveSheep.Dequeue();
                                if (exSheepie != null)
                                {
                                    Destroy(exSheepie);
                                    Debug.LogError($"sheepies summoned:{liveSheep.Count}");
                                }
                            }
                            break;

                        case "toggleSpatialize":
                            spatialize=!spatialize;
                            Debug.LogError($"Spatialize:{spatialize}");
                            break;

                        case "toggleSpatializePostEffects":
                            spatializePostEffects=!spatializePostEffects;
                            Debug.LogError($"spatializePostEffects:{spatializePostEffects}");
                            break;

                        case "toggleAudioListenerVolume":
                            AudioListener.volume=(AudioListener.volume<1.0f)?1.0f:0.0f;
                            Debug.LogError($"AudioListener.volume:{AudioListener.volume}");
                            break;

                        case "toggleMixerSfxReverb":
                            mixerSfxReverb=!mixerSfxReverb;
                            if (mixerSfxReverb)
                                mixerSnapshotReverbEnabled.TransitionTo(0.01f);
                            else
                                mixerSnapshotReverbDisabled.TransitionTo(0.01f);

                            Debug.LogError($"toggleMixerSfxReverb:{mixerSfxReverb}");
                            break;

                       case "decreaseSpatialBlend":
                            spatialBlend=System.Math.Max(spatialBlend-0.05f,0.0f);
                            Debug.LogError($"SpatialBlend:{spatialBlend}");
                            break;
                       case "increaseSpatialBlend":
                            spatialBlend=System.Math.Min(spatialBlend+0.05f,1.0f);
                            Debug.LogError($"SpatialBlend:{spatialBlend}");
                            break;

                      case "decreaseGlobalGain":
                            globalGain=System.Math.Max(globalGain-0.5f,0.0f);
                            Debug.LogError($"globalGain:{globalGain}");
                            break;
                       case "increaseGlobalGain":
                            globalGain=System.Math.Min(globalGain+0.5f,10.0f);
                            Debug.LogError($"globalGain:{globalGain}");
                            break;
                    }
                }

            };

        InputInitialised = true;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
#else
public class SoundObjectPlayer : MonoBehaviour
{
    static internal bool spatialize = true;
    static internal bool spatializePostEffects = false;
    static internal float spatialBlend = 1.0f;
    void Start()
    {
        Debug.LogError("Enable \"Input System Package\" and install com.unity.inputsystem.ps5 package");
    }
}
#endif
