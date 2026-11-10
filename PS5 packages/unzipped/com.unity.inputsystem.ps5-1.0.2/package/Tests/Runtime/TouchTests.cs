using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.PS5;
using UnityEngine.InputSystem.PS5.LowLevel;
using UnityEngine.TestTools;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

[UnityPlatform(RuntimePlatform.PS5)]
public class TouchTests
{
    const float k_Epsilon = float.Epsilon;

    DualSenseGamepad m_Pad;

    [SetUp]
    public void Setup()
    {
        m_Pad = InputSystem.AddDevice<DualSenseGamepad>();
    }

    [TearDown]
    public void Teardown()
    {
        InputSystem.RemoveDevice(m_Pad);
    }

    [Test, Description("When no updates have been supplied to the pad the touch must be valid")]
    public void TouchIsValidWhenNoEvents()
    {
        foreach (var touch in m_Pad.touches)
        {
            Assert.NotNull(touch);
            Assert.DoesNotThrow(() => _ = touch.position);
            Assert.DoesNotThrow(() => _ = touch.delta);
        }
    }

    [Test, Description("Phase should be ended for touches with an invalid position")]
    public void TouchPhaseIsCorrect_Ended()
    {
        SetTouchPositionOnPadAndUpdate(m_Pad, new Vector2(-1f, -1f));

        Assert.AreEqual(TouchPhase.Ended, m_Pad.touches[0].phase);
    }

    [Test, Description("Touches that just started should have the began state")]
    public void TouchPhaseIsCorrect_Began()
    {
        SetTouchPositionOnPadAndUpdate(m_Pad, new Vector2(-1f, -1f));
        SetTouchPositionOnPadAndUpdate(m_Pad, new Vector2(0.5f, 0.5f));

        Assert.AreEqual(TouchPhase.Began, m_Pad.touches[0].phase);
    }

    [Test, Description("Touches that have a delta less than epslion should be stationary")]
    public void TouchPhaseIsCorrect_Stationary()
    {
        SetTouchPositionOnPadAndUpdate(m_Pad, new Vector2(0.5f, 0.5f));
        SetTouchPositionOnPadAndUpdate(m_Pad, new Vector2(0.5f, 0.5f));

        Assert.AreEqual(TouchPhase.Stationary, m_Pad.touches[0].phase);
    }

    [Test, Description("Touches that have differences in position between updates should be moved")]
    public void TouchPhaseIsCorrect_Moving()
    {
        SetTouchPositionOnPadAndUpdate(m_Pad, new Vector2(0f, 0f));
        SetTouchPositionOnPadAndUpdate(m_Pad, new Vector2(0.5f, 0.5f));

        Assert.AreEqual(TouchPhase.Moved, m_Pad.touches[0].phase);
    }

    [Test, Description("Delta for a touch that just began should be 0")]
    public void DeltaIsCorrect_Began()
    {
        SetTouchPositionOnPadAndUpdate(m_Pad, new Vector2(-1f, -1f));
        SetTouchPositionOnPadAndUpdate(m_Pad, new Vector2(0.5f, 0.5f));

        Assert.That(m_Pad.touches[0].delta.x, Is.EqualTo(0f).Within(k_Epsilon));
        Assert.That(m_Pad.touches[0].delta.y, Is.EqualTo(0f).Within(k_Epsilon));
    }

    [Test, Description("Delta for a stationary touch should be 0")]
    public void DeltaIsCorrect_Stationary()
    {
        SetTouchPositionOnPadAndUpdate(m_Pad, new Vector2(0.5f,0.5f));
        SetTouchPositionOnPadAndUpdate(m_Pad, new Vector2(0.5f,0.5f));

        Assert.That(m_Pad.touches[0].delta.x, Is.EqualTo(0f).Within(k_Epsilon));
        Assert.That(m_Pad.touches[0].delta.y, Is.EqualTo(0f).Within(k_Epsilon));
    }

    [Test, Description("Delta for a moving touch should be equvilent to the moved amount")]
    public void DeltaIsCorrect_Moving()
    {
        //Away from top corner
        SetTouchPositionOnPadAndUpdate(m_Pad, new Vector2(0f,0f));
        SetTouchPositionOnPadAndUpdate(m_Pad, new Vector2(0.5f, 0.5f));
        Assert.That(m_Pad.touches[0].delta.x, Is.EqualTo(0.5f).Within(k_Epsilon));
        Assert.That(m_Pad.touches[0].delta.y, Is.EqualTo(0.5f).Within(k_Epsilon));

        //Back towards top corner
        SetTouchPositionOnPadAndUpdate(m_Pad, new Vector2(0.25f, 0.25f));
        Assert.That(m_Pad.touches[0].delta.x, Is.EqualTo(-0.25f).Within(k_Epsilon));
        Assert.That(m_Pad.touches[0].delta.y, Is.EqualTo(-0.25f).Within(k_Epsilon));
    }

    [Test, Description("Touch ended should have 0 delta")]
    public void DeltaIsCorrect_Ended()
    {
        SetTouchPositionOnPadAndUpdate(m_Pad, new Vector2(0,0));
        SetTouchPositionOnPadAndUpdate(m_Pad, new Vector2(-1,-1));

        Assert.That(m_Pad.touches[0].delta.x, Is.EqualTo(0f).Within(k_Epsilon));
        Assert.That(m_Pad.touches[0].delta.y, Is.EqualTo(0f).Within(k_Epsilon));
    }

    static void SetTouchPositionOnPadAndUpdate(DualSenseGamepad pad, Vector2 pos)
    {
        InputSystem.QueueStateEvent(pad, new GamepadStatePS5()
        {
            touch0 = new PS5Touch()
            {
                position = pos,
                touchId = 1
            }
        });
        InputSystem.Update();
    }
}

