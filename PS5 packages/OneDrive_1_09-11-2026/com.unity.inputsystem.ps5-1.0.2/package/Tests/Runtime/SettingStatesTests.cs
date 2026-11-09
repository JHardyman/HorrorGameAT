using System;
using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.InputSystem.PS5;

[UnityPlatform(RuntimePlatform.PS5)]
class SettingStatesTests : PS5InputTestFixture
{
    [Test]
    public void SetLightbarColour()
    {
        Gamepad.SetLightBarColor(Color.blue);
        Assert.That(Gamepad.lightBarColor, Is.EqualTo(Color.blue));
        LogAssert.NoUnexpectedReceived();
        Gamepad.ResetLightBarColor();
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest]
    public IEnumerator HapticsTest()
    {
        Gamepad.SetMotorSpeeds(1f, 1f);
        yield return new WaitForSeconds(0.5f);
        Gamepad.PauseHaptics();
        yield return new WaitForSeconds(0.5f);
        Gamepad.ResetHaptics();
        yield return new WaitForSeconds(0.5f);
        Gamepad.ResetHaptics();

        LogAssert.NoUnexpectedReceived();
    }

    [Test]
    public void ResetOrientation()
    {
        Gamepad.ResetOrientation();
        LogAssert.NoUnexpectedReceived();
    }

#if UNITY_PS5_ENHANCED_INPUT_SYSTEM
    [UnityTest]
    public IEnumerator SetMotionSensorState()
    {
        Gamepad.SetMotionSensorState(false);
        yield return new WaitForSeconds(0.5f);
        Gamepad.SetMotionSensorState(true);
        LogAssert.NoUnexpectedReceived();
    }

    [UnityTest]
    public IEnumerator SetTiltCorrectionSensorState()
    {
        Gamepad.SetTiltCorrectionState(false);
        yield return new WaitForSeconds(0.5f);
        Gamepad.SetTiltCorrectionState(true);
        LogAssert.NoUnexpectedReceived();
    }



    [Test]
    public void SetVibrationMode([Values] VibrationMode vibrationMode)
    {
        Gamepad.SetVibrationMode(vibrationMode);
        LogAssert.NoUnexpectedReceived();
    }
#endif

#if UNITY_SUPPORT_ANGVEL_DEADBAND_STATE
    [UnityTest, Description("Set Angular Velocity Deadband State")]
    public IEnumerator SetAVDBState()
    {
        Gamepad.SetAngularVelocityDeadbandState(false);
        yield return new WaitForSeconds(0.5f);
        Gamepad.SetAngularVelocityDeadbandState(true);
        LogAssert.NoUnexpectedReceived();
    }
#endif

}

