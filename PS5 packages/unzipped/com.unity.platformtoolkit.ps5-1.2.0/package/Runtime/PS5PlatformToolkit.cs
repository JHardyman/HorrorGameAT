using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.PlatformToolkit.PS5.Native;
using UnityEngine;
using Unity.PSN.PS5;
using Unity.PSN.PS5.Initialization;
using Unity.PSN.PS5.Trophies;
using Unity.PSN.PS5.UDS;
using UnityEngine.Assertions;

#if UNITY_6000_5_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif

namespace Unity.PlatformToolkit.PS5
{
#if UNITY_6000_5_OR_NEWER
    [NoAutoStaticsCleanup]
#endif
    internal class PS5PlatformToolkit : IPlatformToolkit
    {
        private AttributeStore m_AttributeStore;
        public ICapabilities Capabilities { get; }

        private PS5AccountSystemManager m_AccountSystemManager;
        public IAccountSystem Accounts => m_AccountSystemManager.AccountSystem;

        public static IReadOnlyList<AchievementDefinitionWithNativeId<int>> Achievements { get; private set; }
        internal static ulong MaxSaveSizeInBytes { get; private set; }
        private readonly int m_InitialUserId;
        private bool m_Initialized = false;

        public PS5PlatformToolkit(PS5RuntimeConfiguration runtimeConfiguration)
        {
            var capabilityBuilder = new CapabilityBuilder
            {
                AccountSupport = true,
                PrimaryAccount = CheckInitialUserEnabled(out m_InitialUserId),
                PrimaryAccountEstablishLimited = false,
                AccountName = true,
                AccountPicture = true,
                AccountAchievementSystem = true,
                AccountSavingSystem = true,
                AccountManualSignOut = false,
                AccountInputPairingSystem = true,
                AdditionalAccountSystem = false,
                LocalSavingSystem = false
            };

            m_AttributeStore = runtimeConfiguration.AttributeStore;
            Achievements = runtimeConfiguration.AchievementDefinitions;
            MaxSaveSizeInBytes = (ulong) runtimeConfiguration.SaveSizeInKib * 1024;
            Capabilities = capabilityBuilder.ToCapabilities();
        }

        private bool CheckInitialUserEnabled(out int userId)
        {
            int ps5UserId = 0;
            var result = PS5Native.sceUserServiceGetInitialUser(ref ps5UserId);
            if (result == 0)
            {
                userId = ps5UserId;
                return true;
            }
            userId = -1;
            return false;
        }

        public async Task Initialize()
        {
            Assert.IsFalse(m_Initialized);
            m_Initialized = true;

            try
            {
                // Initialize PSN systems.
                InitResult psnResult = Unity.PSN.PS5.Main.Initialize();
                if (!psnResult.Initialized)
                {
                    // Something went wrong.
                    Debug.LogError($@"[PS5Implementation::Initialize] PSN failed to Initialize.
                        Sce SDK Version Value: {psnResult.SceSDKVersionValue}
                        DLL Version: {psnResult.DllVersion}
                        Sce SDK Version: {psnResult.SceSDKVersion}.");
                }

                Unity.SaveData.PS5.Initialization.InitSettings initSettings = new Unity.SaveData.PS5.Initialization.InitSettings();
                // Initialize save data systems.
                Unity.SaveData.PS5.Initialization.InitResult savedataResult = Unity.SaveData.PS5.Main.Initialize(initSettings);
                if (!savedataResult.Initialized)
                {
                    // Something went wrong.
                    Debug.LogError($@"[PS5Implementation::Initialize] SaveData failed to Initialize.
                        Sce SDK Version Value: {savedataResult.SceSDKVersionValue}
                        DLL Version: {savedataResult.DllVersion}
                        Sce SDK Version: {savedataResult.SceSDKVersion}.");
                }
            }
            catch (PSNException e)
            {
                throw new InvalidSystemException(e.ExtendedMessage);
            }
            catch (SaveData.PS5.Core.SaveDataException e)
            {
                throw new InvalidSystemException(e.Message);
            }

            Sony.PS5.Dialog.Main.Initialise();
            m_AccountSystemManager = new PS5AccountSystemManager(m_AttributeStore, m_InitialUserId);
            // We can only initialize this after the PSN system is initialized because
            // we need to add users to the PSN system.
            await m_AccountSystemManager.Initialize();
            // We first need to connect to Universal Data Storage (UDS)
            await ConnectToUDS();
            // Then we can connect to the trophy system
            await ConnectToTrophySystem();
        }

        private static async Task ConnectToTrophySystem()
        {
            TrophySystem.StartSystemRequest trophySystemRequest = new TrophySystem.StartSystemRequest();
            await AsyncRequestTaskWrapper.AsyncRequestTrophySystem(trophySystemRequest);
        }

        private static async Task ConnectToUDS()
        {
            UniversalDataSystem.StartSystemRequest UDSrequest = new UniversalDataSystem.StartSystemRequest();
            UDSrequest.PoolSize = 256 * 1024;
            await AsyncRequestTaskWrapper.AsyncRequestUniversalDataSystem(UDSrequest);
        }
    }
}
