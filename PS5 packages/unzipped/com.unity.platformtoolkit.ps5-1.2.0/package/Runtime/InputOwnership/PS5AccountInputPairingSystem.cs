using UnityEngine;
using UnityEngine.PS5;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Unity.PlatformToolkit.PS5
{
    internal class PS5AccountInputPairingSystem : AbstractInputOwnershipSystem
    {
        private readonly PS5AccountSystemManager m_PS5AccountSystemManager;
        public override IAccount GetOwner(IInputDevice inputDevice)
        {
            var accountsSignedIn = m_PS5AccountSystemManager.AccountSystem.SignedIn;

            foreach (var account in accountsSignedIn)
            {
                if (account is not PS5Account ps5Account)
                {
                    continue;
                }

                if (inputDevice.IdType == "SceUserServiceUserId" && int.Parse(inputDevice.Id) == ps5Account.UserId)
                    return ps5Account;
            }
            return null;
        }

        public PS5AccountInputPairingSystem(PS5AccountSystemManager pS5AccountSystemManager)
        {
            m_PS5AccountSystemManager = pS5AccountSystemManager;
#if ENABLE_INPUT_SYSTEM
            InputSystem.onDeviceChange += OnDeviceChange;
#else
            PS5Input.OnUserServiceEvent += OnUserServiceEvent;
#endif
        }

#if ENABLE_INPUT_SYSTEM
        private void OnDeviceChange(InputDevice inputDevice, InputDeviceChange changeType)
        {
            if (changeType == InputDeviceChange.Added || changeType == InputDeviceChange.Removed)
            {
                MarkPairingChanged();
            }
        }
#else
        private void OnUserServiceEvent(PS5Input.UserServiceEventType eventtype, uint userid)
        {
            MarkPairingChanged();
        }
#endif
    }
}
