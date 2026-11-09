using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.PSN.PS5;
using Unity.PSN.PS5.Users;
using UnityEngine;
using UnityEngine.Networking;

namespace Unity.PlatformToolkit.PS5
{
    class PS5Account : IAccount, IDoubleSignOut, IAsyncDisposable
    {
        private GenericLifetimeToken m_LifetimeToken = new GenericLifetimeToken();

        private PS5AchievementSystem m_AchievementSystem;
        private GenericSavingSystem m_SavingSystem;

        // Temporarily cache profiles to avoid multiple requests in a short timespan.
        private UserSystem.UserProfile m_CachedProfile;
        private int m_LastProfileRequestFrame;

        public AccountState State { get; internal set; }

        public int UserId { get; internal set; }
        public ulong AccountId { get; internal set; }

        private AccountAttributeProvider<PS5Account> m_AttributeProvider;

        public PS5Account(AccountAttributeProvider<PS5Account> provider)
        {
            m_AttributeProvider = provider;
        }

        internal void Initialize()
        {
            m_SavingSystem = new GenericSavingSystem(new PS5StorageSystem(UserId), parentLifetimeToken: m_LifetimeToken);
        }

        public Task<T> GetAttribute<T>(string attributeName)
        {
            return m_AttributeProvider.GetAttribute<T>(this, attributeName);
        }

        public bool HasAttribute<T>(string attributeName)
        {
            return m_AttributeProvider.HasAttribute<T>(attributeName);
        }

        private string CreateSafeURL(string url)
        {
            // without this we get an InvalidOperationException: Insecure connection not allowed
            if (url.StartsWith("http") && !url.StartsWith("https"))
            {
                return url.Replace("http", "https");
            }
            return url;
        }

        public static Task<int> GetUserId(PS5Account account)
        {
            return Task.FromResult(account.UserId);
        }

        public static async Task<Texture2D> SmallAvatar(PS5Account account)
        {
            return await account.GetAvatar(UserSystem.ProfilePicture.Sizes.Small);
        }

        public static async Task<Texture2D> MediumAvatar(PS5Account account)
        {
            return await account.GetAvatar(UserSystem.ProfilePicture.Sizes.Medium);
        }

        public static async Task<Texture2D> LargeAvatar(PS5Account account)
        {
            return await account.GetAvatar(UserSystem.ProfilePicture.Sizes.Large);
        }

        public static async Task<Texture2D> ExtraLargeAvatar(PS5Account account)
        {
            return await account.GetAvatar(UserSystem.ProfilePicture.Sizes.ExtraLarge);
        }

        private async Task<Texture2D> GetAvatar(UserSystem.ProfilePicture.Sizes size)
        {
            UserSystem.UserProfile userProfile = await GetUserProfile();
            if (userProfile == null)
            {
                throw new TemporarilyUnavailableException($"Unable to get user profile for user id '{UserId}'.");
            }

            var profile = userProfile.Avatars.FirstOrDefault(p => p.Size == size);
            if (profile == null)
            {
                throw new TemporarilyUnavailableException($"Unable to get avatar for user id '{UserId}'.");
            }
            UnityWebRequest avatarGetPictureRequest = UnityWebRequestTexture.GetTexture(CreateSafeURL(profile.Url));
            UnityWebRequestAsyncOperation avatarWebRequestOperation = avatarGetPictureRequest.SendWebRequest();
            await Awaitable.FromAsyncOperation(avatarWebRequestOperation);
            Texture2D avatarTexture = DownloadHandlerTexture.GetContent(avatarGetPictureRequest);

            return avatarTexture;
        }

        public static async Task<string> OnlineId(PS5Account account)
        {
            UserSystem.UserProfile userProfile = await account.GetUserProfile();
            if (userProfile == null)
            {
                throw new TemporarilyUnavailableException($"Unable to get online ID for user id '{account.UserId}'.");
            }

            return userProfile.OnlineId;
        }

        public static async Task<string> FirstName(PS5Account account)
        {
            UserSystem.UserProfile userProfile = await account.GetUserProfile();
            if (userProfile == null || userProfile.PersonalDetails == null)
            {
                throw new TemporarilyUnavailableException($"Unable to get first name for user id '{account.UserId}'.");
            }

            return userProfile.PersonalDetails.FirstName;
        }

        public static async Task<string> MiddleName(PS5Account account)
        {
            UserSystem.UserProfile userProfile = await account.GetUserProfile();
            if (userProfile == null || userProfile.PersonalDetails == null)
            {
                throw new TemporarilyUnavailableException($"Unable to get middle name for user id '{account.UserId}'.");
            }

            return userProfile.PersonalDetails.MiddleName;
        }

        public static async Task<string> LastName(PS5Account account)
        {
            UserSystem.UserProfile userProfile = await account.GetUserProfile();
            if (userProfile == null || userProfile.PersonalDetails == null)
            {
                throw new TemporarilyUnavailableException($"Unable to get last name for user id '{account.UserId}'.");
            }

            return userProfile.PersonalDetails.LastName;
        }

        private async Task<UserSystem.UserProfile> GetUserProfile()
        {
            // Get the last result if a batch of requests were dispatched in the same frame that the first one returned.
            if (Time.frameCount == m_LastProfileRequestFrame && m_LastProfileRequestFrame != -1)
            {
                return m_CachedProfile;
            }
            m_LastProfileRequestFrame = -1;
            m_CachedProfile = null;


            UserSystem.GetProfilesRequest request = new UserSystem.GetProfilesRequest()
            {
                UserId = UserId,
                AccountIds = null,
                RetrievedProfiles = new List<UserSystem.UserProfile>()
            };

            try
            {
                var result = await AsyncRequestTaskWrapper.AsyncRequestUserSystem(request);
                if (result.Request.RetrievedProfiles.Count == 1)
                {
                    m_LastProfileRequestFrame = Time.frameCount;
                    m_CachedProfile = result.Request.RetrievedProfiles[0];
                    return m_CachedProfile;
                }

                return null;
            }
            catch (PSNException e)
            {
               throw new PSNException($"Unable to get profile of user {UserId}. Check if this user is signed-in to PSN. PSN ExtendedMessage: {e.ExtendedMessage}");
            }
        }

        public  async Task<IAchievementSystem> GetAchievementSystem()
        {
            m_LifetimeToken.ThrowOnDisposedAccess();
            if (m_AchievementSystem != null)
            {
                return m_AchievementSystem;
            }
            m_AchievementSystem = new PS5AchievementSystem(UserId, PS5PlatformToolkit.Achievements);
            await m_AchievementSystem.Initialize();
            return m_AchievementSystem;
        }

        public Task<ISavingSystem> GetSavingSystem()
        {
            m_LifetimeToken.ThrowOnDisposedAccess();
            return Task.FromResult<ISavingSystem>(m_SavingSystem);
        }

        public Task<bool> SignOut()
        {
            // We can't sign out a native account on PS5. That is handled by the system.
            throw new NotSupportedException();
        }

        public async ValueTask DisposeAsync()
        {
            if (m_LifetimeToken.TryAtomicDispose(DisposeReason.InvalidAccount))
            {
                if (m_SavingSystem != null)
                {
                    await m_SavingSystem.DisposeAsync();
                }
            }
        }

        public bool TrySignOut()
        {
            if (!m_LifetimeToken.TryAtomicDispose(DisposeReason.InvalidAccount))
                return false;

            State = AccountState.SignedOut;
            return true;
        }

        public async Task CleanUpAfterSignOut()
        {
            var tasks = new List<Task>(2);
            if (m_SavingSystem != null)
            {
                tasks.Add(m_SavingSystem.DisposeAsync().AsTask());
                m_SavingSystem = null;
            }
            if (m_AchievementSystem != null)
            {
                tasks.Add(m_AchievementSystem.DisposeAsync().AsTask());
                m_AchievementSystem = null;
            }
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        public async Task<string> GetName()
        {
            try
            {
                return await OnlineId(this);
            }
            catch (Exception e)
            {
                return AccountErrorHandling.HandleGetNameException(e);
            }
        }

        public async Task<Texture2D> GetPicture()
        {
            try
            {
                return await GetAvatar(UserSystem.ProfilePicture.Sizes.Medium);
            }
            catch (Exception e)
            {
                return AccountErrorHandling.HandleGetPictureException(e);
            }
        }
    }
}
