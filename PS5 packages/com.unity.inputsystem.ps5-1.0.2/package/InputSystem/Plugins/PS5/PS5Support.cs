using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.DualShock;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.PS5.Processors;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.InputSystem.DualShock.LowLevel;
#endif

[assembly: AlwaysLinkAssembly]

namespace UnityEngine.InputSystem.PS5
{
    /// <summary>
    /// Adds support for PS5 controllers.
    /// </summary>
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
#if UNITY_DISABLE_DEFAULT_INPUT_PLUGIN_INITIALIZATION
    public
#else
    internal
#endif
        static class PS5Support
    {
        static PS5Support()
        {

            //Register processors before any devices that use them are registered
            InputSystem.RegisterProcessor<ConvertRightToLeftHandQuaternion>();
            InputSystem.RegisterProcessor<ConvertRightToLeftHandVector3>();

#if UNITY_EDITOR || !UNITY_PS5

            // through the editor
            InputSystem.RegisterLayout<DualSenseGamepadPC>(
                matches: new InputDeviceMatcher()
                    .WithInterface("HID")

                    // by having these extra values we choose this layout in preference with the DualSenseGamepadHID.
                    // the DualSenseGamepadPC inherits from DualSenseGamepadHID but adds extra NDA functionality
                    .WithManufacturer("Sony.+Entertainment.*")
                    .WithProduct(".*Wireless Controller")
                    .WithCapability("vendorId", 0x54C) // Sony Entertainment.
                    .WithCapability("productId", 0xCE6));

            //Dualsense Edge
            InputSystem.RegisterLayout<DualSenseGamepadPC>(
                matches: new InputDeviceMatcher()
                    .WithInterface("HID")

                    .WithManufacturer("Sony.+Entertainment.*")
                    .WithProduct(".*Dualsense Edge.*")
                    .WithCapability("vendorId", 0x54C) // Sony Entertainment.
                    .WithCapability("productId", 0xDF2)); // Dual Sense Edge
#endif
#if UNITY_EDITOR || UNITY_PS5
#if UNITY_INPUT_SYSTEM_CAN_SET_RUN_IN_BACKGROUND
            InputSystem.runInBackground = true;
#endif

            // on the device itself
            InputSystem.RegisterLayout<PS5TouchControl>("PS5Touch");

            // old naming. will be deprecated
            InputSystem.RegisterLayout<DualSenseGamepad>("PS5DualSenseGamepad",
                matches: new InputDeviceMatcher()
                    .WithInterface("PS5")
                    .WithDeviceClass("PS5DualShockGamepad"));
#endif

        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void InitializeInPlayer() { }
    }
}

