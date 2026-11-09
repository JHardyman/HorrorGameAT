# Trigger effects

The adaptive triggers on the DualSense® Wireless controller use motors to provide the user with tactile feedback in response to gameplay. The **L2** and **R2** triggers use internal feedback module motors to add specific haptic feedback to your project, such as resistance or vibration.

> [!NOTE]
> The PS5 Input system package allows you to test and update trigger effects whilst in [Play mode](https://docs.unity3d.com/Manual/GameView.html).

Trigger effects can be used to simulate resistance such as pulling back a bow string, or the release of a gun trigger. The effect can start at a specific trigger position, adding resistance on the trigger, and then set to release at another trigger position. There are 10 trigger positions that can be used (positions 0 to 9), with the option to add different effects for each position.

Refer to [Using the DualSense® Wireless Controller Trigger Effect Feature](https://game.develop.playstation.net/resources/documents/SDK/latest/Pad-Overview/using-the-trigger-effect-feature.html) (PlayStation®5 DevNet) for more information.

## Trigger effect types

There are six effects that you can use with the triggers. For more information on each effect, refer to the following table.


|**Effect**|**Description**|
|---|---|
|**[TriggerEffectFeedbackParam](xref:UnityEngine.InputSystem.PS5.TriggerEffectFeedbackParam)**|Allows you to set the start position and strength of the trigger effect. The trigger will provide the same degree of feedback once activated.|
|**[TriggerEffectMultiplePositionFeedbackParam](xref:UnityEngine.InputSystem.PS5.TriggerEffectMultiplePositionFeedbackParam)**|Set the strength of the trigger feedback for each specific trigger position.|
|**[TriggerEffectSlopeFeedbackParam](xref:UnityEngine.InputSystem.PS5.TriggerEffectSlopeFeedbackParam)**|Change the strength of the feedback on a linear scale between a desired start and end trigger position.|
|**[TriggerEffectWeaponParam](xref:UnityEngine.InputSystem.PS5.TriggerEffectWeaponParam)**|This control mode allows you to set the start and end trigger position with a desired strength, but providing no feedback when releasing the trigger after exceeding the end position. This can be used to emulate the trigger of a gun.|
|**[TriggerEffectVibrationParam](xref:UnityEngine.InputSystem.PS5.TriggerEffectVibrationParam)**|Set the start and end position of the trigger feature, its strength, and the vibrational frequency of the feedback.|
|**[TriggerEffectMultiplePositionVibrationParam](xref:UnityEngine.InputSystem.PS5.TriggerEffectMultiplePositionVibrationParam)**|Similar to SetGamepadTriggerEffectVibration, this control mode allows you to set the strength and vibration of the feedback for multiple trigger positions.|
|**[SetGamepadTriggerEffectOff]()**| Disable the trigger effect feature. It’s possible to transition from any other trigger control mode into off mode as required.|

For information about these effects, refer to [Trigger Effect Feature Configuration and Control Modes](https://game.develop.playstation.net/resources/documents/SDK/latest/Pad-Overview/control-schemes-for-the-trigger-effect-fea.html) (PlayStation®5 DevNet).

> [!NOTE] 
> Multi-position feedback mode, slope feedback mode, and multi-position vibration mode are enabled only if the version of the DualSense® wireless controller is `0224` or later. For DualSense Edge™ wireless controllers, all trigger control modes are available regardless of the controller's version.

## Trigger effect example

The following example details the steps to add a weapon trigger effect to a controller with a set start position, end position, and strength.

```csharp
var pad = UnityEngine.InputSystem.PS5.DualSenseGamepad.GetBySlotIndex(0);
var effect = new UnityEngine.InputSystem.PS5.TriggerEffectParam(UnityEngine.InputSystem.PS5.TriggerEffectMask.L2 | UnityEngine.InputSystem.PS5.TriggerEffectMask.R2);

effect.right.mode = UnityEngine.InputSystem.PS5.TriggerEffectMode.Weapon;
effect.right.weapon.startPosition = 2;
effect.right.weapon.endPosition = 8;
effect.right.weapon.strength = 8;

pad.SetTriggerEffect(effect);
```

> [!NOTE]
> In the provided sample project, refer to the `Scripts/Gamepad/GamepadTriggerEffects.cs` file for more examples of working with trigger effects in a project.

## Additional resources

* [Using the DualSense® Wireless Controller Trigger Effect Feature](https://game.develop.playstation.net/resources/documents/SDK/latest/Pad-Overview/control-schemes-for-the-trigger-effect-fea.html)
* [Use the sample project](sample-project.md)
