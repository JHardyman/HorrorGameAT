using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.PS5;
using UnityEngine.TestTools;

class DeviceRecognitionTests
{
    const int sieVendorID = 0x54C; //Sony Vendor ID
    const int dualsenseProductID = 0xCE6; //Dualsense
    const int dualsenseEdgeProductID = 0xDF2; //Dualsense Edge

    [TestCase("Wireless Controller", "Sony Interactive Entertainment", sieVendorID, dualsenseProductID)]
    [TestCase("Dualsense Wireless Controller", "Sony Interactive Entertainment", sieVendorID, dualsenseProductID)]
    [TestCase("Dualsense Edge", "Sony Interactive Entertainment", sieVendorID, dualsenseEdgeProductID)]
    public void Devices_RecognisedAsDualsensePC(string name, string manufacturer, int vendorId, int productId)
    {
        var device = InputSystem.AddDevice(new InputDeviceDescription
        {
            product = name,
            manufacturer = manufacturer,
            interfaceName = "HID",
            capabilities = new HID.HIDDeviceDescriptor
            {
                vendorId = vendorId,
                productId = productId,
            }.ToJson()
        });

        Assert.That(device, Is.AssignableTo<DualSenseGamepadPC>());
    }
}
