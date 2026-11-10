using System.Runtime.InteropServices;

namespace Unity.PlatformToolkit.PS5.Native
{
    internal static class PS5Native
    {
        public const int SCE_USER_SERVICE_ERROR_OPERATION_NOT_SUPPORTED = unchecked((int)0x80960006);

        public const int SCE_NP_TROPHY2_ERROR_INVALID_TROPHY_ID = unchecked((int)0x8055390a);
        /// <summary>
        /// Get the initial user who started the application if InitialUserAlwaysLoggedIn is enabled in the settings. Will return an error if InitialUserAlwaysLoggedIn is disabled in the settings.
        /// </summary>
        /// <param name="userId">The initial user ID who started the application</param>
        /// <returns>When returning 0, it means OK. When it returns a negative number, it means error/InitialUserAlwaysLoggedIn is disabled. Docs:
        /// https://game.develop.playstation.net/resources/documents/SDK/11.000/UserService-Reference/sce-user-service-get-initial-user.html</returns>
        [DllImport("__Internal")]
        public static extern int sceUserServiceGetInitialUser(ref int userId);
    }
}
