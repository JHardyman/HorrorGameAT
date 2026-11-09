using System.Collections;
using NUnit.Framework;
using UnityEngine.InputSystem.PS5;
using UnityEngine.TestTools;

internal class PS5InputTestFixture
{
    internal static DualSenseGamepad Gamepad => DualSenseGamepad.all[0];

    [UnitySetUp]
    public IEnumerator Setup()
    {
        yield return null; //Wait 1 frame for input system init
        AssertInconclusiveIfNoGamepad();
    }

    internal static void AssertInconclusiveIfNoGamepad()
    {
        if (Gamepad == null)
        {
            Assert.Inconclusive("No Gamepad Connected, these tests need a controller connected to work");
        }
    }
}
