using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.PS5;
using UnityEngine.InputSystem.PS5.LowLevel;
using UnityEngine.TestTools;

[UnityPlatform(RuntimePlatform.PS5)]
public class InputConversionTests
{
    DualSenseGamepad device;

    [SetUp]
    public void Setup()
    {
        device = InputSystem.AddDevice<DualSenseGamepad>();
        Assert.NotNull(device);
    }

    [TearDown]
    public void TearDown()
    {
        InputSystem.RemoveDevice(device);
    }

    [Test, Description("Check that the SDK Supplied Acceleration is Converted to Unity Coordinate Space")]
    public void CorrectsAccelerationSDKValue()
    {
        Vector3 sdkValue = new Vector3(1f, 2f, 3f);
        Vector3 expectedValue = new Vector3(sdkValue.x, sdkValue.y, -sdkValue.z);
        InputSystem.QueueStateEvent(device, new GamepadStatePS5()
        {
            acceleration = sdkValue
        });
        InputSystem.Update();

        Assert.That(device.acceleration.ReadUnprocessedValue(), Is.EqualTo(sdkValue));
        Assert.That(device.acceleration.ReadValue(), Is.EqualTo(expectedValue));
    }

    [Test, Description("Check that the SDK Supplied Angular Velocity is Converted to Unity Coordinate Space")]
    public void CorrectsAngularVelocitySDKValue()
    {
        Vector3 sdkValue = new Vector3(1f, 2f, 3f);
        //Angular velocity changes RH -> LH space and inverts the vector so it matches Unity expectations
        Vector3 expectedValue = new Vector3(-sdkValue.x, -sdkValue.y, sdkValue.z);
        InputSystem.QueueStateEvent(device, new GamepadStatePS5()
        {
            angularVelocity = sdkValue
        });
        InputSystem.Update();

        Assert.That(device.angularVelocity.ReadUnprocessedValue(), Is.EqualTo(sdkValue));
        Assert.That(device.angularVelocity.ReadValue(), Is.EqualTo(expectedValue));
    }

    [Test, Description("Check that the SDK Supplied Orientation is Converted to Unity Coordinate Space")]
    public void CorrectsOrientationSDKValue()
    {
        Quaternion sdkValue = new Quaternion(-0.0072109f, -0.0032049f, -0.0016024f, 0.9999676f); //Euler Angles of 45, 20, 10
        Quaternion expectedValue = new Quaternion(-0.0072109f, -0.0032049f, 0.0016024f, -0.9999676f);

        InputSystem.QueueStateEvent(device, new GamepadStatePS5()
        {
            orientation = sdkValue
        });
        InputSystem.Update();

        Assert.That(device.orientation.ReadUnprocessedValue(), Is.EqualTo(sdkValue));
        Assert.That(device.orientation.ReadValue(), Is.EqualTo(expectedValue));
    }
}


