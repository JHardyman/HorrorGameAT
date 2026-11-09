# Motion sensors

The DualSense wireless controller has a 3-axis accelerometer and 3-axis angular velocity sensor to detect the movement and orientation of the controller.

For a detailed view of the motion sensor system, refer to [Using the DualSense® Wireless Controller Motion Sensors](https://game.develop.playstation.net/resources/documents/SDK/latest/Pad-Overview/using-motion-sensors.html) (PlayStation®5 DevNet).

The sensors make it possible to detect the following elements of the controllers movement.

> [!NOTE]
> Data from the following APIs are converted from Right to Left-handed coordinate space to match Unity's coordinate space. To read the uncorrected value from the SDK, use `ReadValueUnprocessed()`.

##  Orientation
To retrieve the orientation of the controller, use [`DualSenseGamepad.orientation`](xref:UnityEngine.InputSystem.PS5.DualSenseGamepad.orientation). The returned data is the orientation of the controller relative to the orientation of the controller when it was first connected. 

Use [`DualSenseGamepad.ResetOrientation`](xref:UnityEngine.InputSystem.PS5.DualSenseGamepad.ResetOrientation) at any time to reset the controller to register its default position from the current controller state.

## Angular velocity

Use [`DualSenseGamepad.angularVelocity`](xref:UnityEngine.InputSystem.PS5.DualSenseGamepad.angularVelocity) to return the angular velocity of the controller. The returned values are in radians per second, allowing you to calculate how fast the controller is rotating in the user's hands.

## Acceleration

Use [`DualSenseGamepad.acceleration`](xref:UnityEngine.InputSystem.PS5.DualSenseGamepad.acceleration) to return the acceleration data from the controller. The acceleration value returned is always affected by gravitational acceleration. For example, 1 g of gravitational acceleration is returned when the controller is stationary.

## Additional resources

* [Using the DualSense® Wireless Controller Motion Sensors](https://game.develop.playstation.net/resources/documents/SDK/latest/Pad-Overview/using-motion-sensors.html) (PlayStation®5 DevNet).

