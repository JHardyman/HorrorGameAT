using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.PS5;

class DeviceRecognitionTests {

    [Test, Description("Check that Dualsense controller is recognised as a DualsenseGamepad on device")]
    [UnityPlatform(RuntimePlatform.PS5)]
    public void Devices_SupportsDeviceDualsense_AsDualsense() {
        var device = InputSystem.AddDevice(new InputDeviceDescription
        {
            interfaceName = "PS5",
            deviceClass = "PS5DualShockGamepad"
        });

        try
        {
            Assert.That(device, Is.AssignableTo<DualSenseGamepad>());
        }
        finally
        {
            InputSystem.RemoveDevice(device);
        }
    }

}
