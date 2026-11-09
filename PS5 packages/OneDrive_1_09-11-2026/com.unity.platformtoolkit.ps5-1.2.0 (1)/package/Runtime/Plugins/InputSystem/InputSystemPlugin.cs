using UnityEngine;
using UnityEngine.InputSystem.PS5;
using UnityEngine.Scripting;

[assembly: AlwaysLinkAssembly]

namespace Unity.PlatformToolkit.PS5.InputSystem
{
    internal static class InputSystemPlugin
    {
        [RuntimeInitializeOnLoadMethod]
        private static void PrepareRegistration()
        {
            PlatformToolkit.Initialized += Initialize;
        }

        private static void Initialize()
        {
            if (!PlatformToolkit.Capabilities.InputOwnership)
                return;

            PlatformToolkit.Accounts.InputOwnership.RegisterInputDeviceConverter<DualSenseGamepad>(ConvertDualSenseGamepad);
        }

        private static IInputDevice ConvertDualSenseGamepad(DualSenseGamepad dualSenseGamepad)
        {
            return new GenericInputDevice("SceUserServiceUserId", dualSenseGamepad.ps5UserId.ToString());
        }
    }
}
