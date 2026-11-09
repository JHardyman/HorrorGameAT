using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Unity.PlatformToolkit.PS5.Native;
using UnityEngine;
using UnityEngine.PS5;
using Unity.PSN.PS5.Users;

namespace Unity.PlatformToolkit.PS5
{

    class PS5AccountSystemManager : IPrimaryAccountSystemProvider
    {
        private GenericAccountSystem<PS5Account> m_AccountSystem;
        public IAccountSystem AccountSystem => m_AccountSystem;

        const int k_MaxNativeUsers = 4;

        private readonly IInputOwnershipSystem m_InputOwnershipSystem;
        public IInputOwnershipSystem InputOwnership => m_InputOwnershipSystem;
        private AccountAttributeProvider<PS5Account> m_AttributeProvider;
        private readonly int m_InitialUserId;

        public PS5AccountSystemManager(AttributeStore store, int initialUserId)
        {
            m_InputOwnershipSystem = new PS5AccountInputPairingSystem(this);

            m_AttributeProvider = new AccountAttributeProvider<PS5Account>(store.Attributes);

            m_AttributeProvider.RegisterAttributeGetter("OnlineId", PS5Account.OnlineId);
            m_AttributeProvider.RegisterAttributeGetter("UserId", PS5Account.GetUserId);
            m_AttributeProvider.RegisterAttributeGetter("FirstName", PS5Account.FirstName);
            m_AttributeProvider.RegisterAttributeGetter("MiddleName", PS5Account.MiddleName);
            m_AttributeProvider.RegisterAttributeGetter("LastName", PS5Account.LastName);
            m_AttributeProvider.RegisterAttributeGetter("SmallAvatar", PS5Account.SmallAvatar);
            m_AttributeProvider.RegisterAttributeGetter("MediumAvatar", PS5Account.MediumAvatar);
            m_AttributeProvider.RegisterAttributeGetter("LargeAvatar", PS5Account.LargeAvatar);
            m_AttributeProvider.RegisterAttributeGetter("ExtraLargeAvatar", PS5Account.ExtraLargeAvatar);

            m_AttributeProvider.FinalizeRegistration();

            m_InitialUserId = initialUserId;
        }

        internal async Task Initialize()
        {
            var accounts = await CreateLoggedInAccounts();
            if (m_InitialUserId > 0)
            {
                var primaryAccountIndex = accounts.FindIndex(account => account.UserId == m_InitialUserId);
                m_AccountSystem = new GenericAccountSystem<PS5Account>(
                    initialAccounts: accounts,
                    primaryAccountSystemProvider: this,
                    primaryAccountIndex: primaryAccountIndex,
                    inputOwnershipSystem: m_InputOwnershipSystem);
            }
            else
            {
                m_AccountSystem = new GenericAccountSystem<PS5Account>(
                    initialAccounts: accounts,
                    primaryAccountSystemProvider: null,
                    inputOwnershipSystem: m_InputOwnershipSystem);
            }

            UnityEngine.PS5.PS5Input.OnUserServiceEvent += OnUserServiceEvent;
        }

        private async Task<List<PS5Account>> CreateLoggedInAccounts()
        {
            await Awaitable.MainThreadAsync();
            List<PS5Account> accounts = new List<PS5Account>();
            for (int i = 0; i < k_MaxNativeUsers; ++i)
            {
                PS5Input.LoggedInUser loggedInUser = PS5Input.GetUsersDetails(i);

                if (loggedInUser.userId > 0)
                {
                    var ps5Account = new PS5Account(m_AttributeProvider)
                    {
                        UserId = loggedInUser.userId,
                        AccountId = loggedInUser.accountId,
                        State = AccountState.SignedIn,
                    };
                    accounts.Add(ps5Account);

                    UserSystem.AddUserRequest addUserRequest = new UserSystem.AddUserRequest()
                    {
                        UserId = loggedInUser.userId
                    };
                    await AsyncRequestTaskWrapper.AsyncRequestUserSystem(addUserRequest);
                    ps5Account.Initialize();
                }
            }

            return accounts;
        }

        private async void OnUserServiceEvent(PS5Input.UserServiceEventType eventtype, uint userid)
        {
            if (eventtype == PS5Input.UserServiceEventType.Login)
            {
                await using var accountModifier = await m_AccountSystem.BeginAccountSystemModification();

                // This event can fire for users that had already been added during initialization, if this was a pending event from boot.
                if (accountModifier.SignedInAccounts.Any(a => a.UserId == userid))
                {
                    // The user is already tracked as signed-in.
                    return;
                }

                for (int i = 0; i < k_MaxNativeUsers; ++i)
                {
                    PS5Input.LoggedInUser loggedInUser = PS5Input.GetUsersDetails(i);
                    bool foundUserLocally = loggedInUser.userId == userid;
                    if (foundUserLocally)
                    {
                        var PS5Account = new PS5Account(m_AttributeProvider)
                        {
                            UserId = loggedInUser.userId,
                            AccountId = loggedInUser.accountId,
                            State = AccountState.SignedIn,
                        };

                        accountModifier.Add(PS5Account);
                        UserSystem.AddUserRequest addUserRequest = new UserSystem.AddUserRequest()
                        {
                            UserId = PS5Account.UserId
                        };

                        await AsyncRequestTaskWrapper.AsyncRequestUserSystem(addUserRequest);
                        PS5Account.Initialize();
                    }
                }
            }

            if (eventtype == PS5Input.UserServiceEventType.Logout)
            {
                await using var accountModifier = await m_AccountSystem.BeginAccountSystemModification().ConfigureAwait(false);

                var signedOutAccount = accountModifier.SignedInAccounts.FirstOrDefault(account => account.UserId == userid);
                if (signedOutAccount != null)
                {
                    UserSystem.RemoveUserRequest removeUserRequest = new UserSystem.RemoveUserRequest()
                    {
                        UserId = (int)userid
                    };
                    await AsyncRequestTaskWrapper.AsyncRequestUserSystem(removeUserRequest);
                    accountModifier.Remove(signedOutAccount);
                }
            }
        }

        public Task<IAccount> Establish()
        {
            return Task.FromResult(m_AccountSystem.PrimaryCurrentUser);
        }
    }
}
