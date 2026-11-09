using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem.PS5;
using UnityEngine.TestTools;

[UnityPlatform(RuntimePlatform.PS5)]
class GettingInfoTests : PS5InputTestFixture
{
    [Test]
    public void GetUserID()
    {
        Assert.That(Gamepad.ps5UserId, Is.Not.EqualTo(-1));
    }

    [Test]
    public void GetSlotID()
    {
        Assert.That(Gamepad.slotIndex, Is.InRange(0, 3));
    }

#if UNITY_PS5_ENHANCED_INPUT_SYSTEM
    [Test]
    public void GetTouchpadInfo()
    {
        Assert.That(Gamepad.touchPadInformation, Is.Not.EqualTo(default(TouchPadInformation)));
    }

    [Test]
    public void GetStickInfo()
    {
        Assert.That(Gamepad.stickInfo, Is.Not.EqualTo(default(StickInformation)));
    }

    [Test]
    public void GetDeviceClass()
    {
        Assert.That(Gamepad.deviceClass, Is.InRange(DeviceClass.Standard, DeviceClass.Gun));
    }

    [Test]
    public void GetConnectionMode()
    {
        Assert.That(Gamepad.connectionType, Is.InRange(ConnectionType.Local, ConnectionType.RemoteDualsense));
    }
#endif
}
