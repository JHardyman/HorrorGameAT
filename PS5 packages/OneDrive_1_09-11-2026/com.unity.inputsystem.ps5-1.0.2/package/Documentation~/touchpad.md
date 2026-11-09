# Touchpad

The DualSense® wireless controller has a capacitive touchpad that a player can interact with in your title. The touchpad can detect a maximum of two points of contact at any one time. The touchpad can also be used as a button.

For more information on using the touchpad, refer to [Using the DualSense® Wireless Controller Touch Pad](https://game.develop.playstation.net/resources/documents/SDK/latest/Pad-Overview/using-the-touch-pad.html) (PlayStation®5 DevNet).

## Retrieving touchpad data

The touchpad uses a coordinate system to track any touches on the surface. This requires the touchpad to have a set resolution and pixel density. For the DualSense® wireless controller, the resolution is defined as 1920 × 1080, and the pixel density as 44.86 dots/mm. As other controllers can be used with the PlayStation 5 (PS5), it’s recommended to use [`TouchPadInformation`](xref:UnityEngine.InputSystem.PS5.TouchPadInformation) to retrieve the resolution and pixel density of each connected controller.

> [!NOTE] 
> `TouchPadInformation` is available only in Unity 6000.0 and later.

The value of a touch on a touchpad touch is normalized in the range of 0 to 1 , based on the expected resolution of 1920 × 1080. When working with controllers that have different touchpad resolutions, multiply these values by the difference in resolutions to get the correct value.

> [!NOTE]
> (&minus;1,&minus;1) is the value supplied from the pad when the touchpad doesn’t have a valid touch position.

To retrieve information about how and where the user is interacting with the touchpad, use the following example:

```csharp
DualSenseGamepad pad = DualSenseGamepad.GetBySlotIndex(0);

            //Get the positions of the touches - there are always 2 touches reported
            PS5TouchControl touch0 = pad.touches[0];
            PS5TouchControl touch1 = pad.touches[1];

            //Get the phase of the touch
            TouchPhase phase = touch0.phase;
            //Touch is not valid when it's state is ended or none and will report position as (-1,-1)
            if (phase == TouchPhase.Ended || phase == TouchPhase.None)
            {
                return;
            }

            int id = touch0.touchId.ReadValue();
            Vector2 position = touch0.position.ReadValue();
            Vector2 delta = touch0.delta;
```

> [!NOTE] 
> If the distance between two points is less than 1 cm, the two points might be recognized as one point.

Additional resources

* [Using the DualSense® Wireless Controller Touch Pad](https://game.develop.playstation.net/resources/documents/SDK/latest/Pad-Overview/using-the-touch-pad.html) (PlayStation®5 DevNet)




