# Troubleshooting

Lists troubleshooting information for common issues when using the PS5 Input System package.

## No vibration on the connected DualSense controller

When implementing controller vibration in your title, the default vibration mode is set to **Advanced**. Check that you have the vibration mode set correctly to the mode you intend to use. For more information, refer to [Vibration](vibration.md).

## The Unity Editor and Windows Runtime detect different devices

This behavior is expected. The Unity Editor has a device that's used to emulate a DualSense Controller and enable its features in the Editor as if it's running on a PS5. However, these features aren’t available in a Windows Standalone Player.

[`DualsenseGamepadPC`](xref:UnityEngine.InputSystem.PS5.DualSenseGamepadPC) inherits from [`DualsenseGamepadHIDInputReport`](xref:UnityEngine.InputSystem.PS5.DualsenseGamepadHIDInputReport), with `DualsenseGamepadHID` used as the controller type in a Windows Standalone Player. Only use the features of `DualsenseGamepadHID` in a non-PS5 Player. 

