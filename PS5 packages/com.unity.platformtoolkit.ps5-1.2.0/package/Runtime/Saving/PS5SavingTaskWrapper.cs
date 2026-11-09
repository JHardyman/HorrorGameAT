using System.Threading.Tasks;
using Unity.SaveData.PS5;
using Unity.SaveData.PS5.Core;
using Unity.SaveData.PS5.Delete;
using Unity.SaveData.PS5.Info;
using Unity.SaveData.PS5.Mount;
using Unity.SaveData.PS5.Search;

#if UNITY_6000_5_OR_NEWER
using Unity.Scripting.LifecycleManagement;
#endif

namespace Unity.PlatformToolkit.PS5
{
#if UNITY_6000_5_OR_NEWER
    [NoAutoStaticsCleanup]
#endif
    internal static class PS5SavingTaskWrapper
    {
        private static bool m_IsInitialized = false;

        public static Task<PS5TaskWrapperMountResponse> Mount(Mounting.MountRequest request)
        {
            SubscribeToAsyncFileOperationCallback();

            var response = new PS5TaskWrapperMountResponse();
            Mounting.Mount(request, response);
            return response.TaskCompletionSource.Task;
        }

        public static Task<PS5TaskWrapperWriteFileResponse> WriteFile(PS5TaskWrapperWriteFileRequest request)
        {
            SubscribeToAsyncFileOperationCallback();

            PS5TaskWrapperWriteFileResponse response = new PS5TaskWrapperWriteFileResponse();
            FileOps.CustomFileOp(request, response);
            return response.TaskCompletionSource.Task;
        }

        public static Task<PS5TaskWrapperEmptyResponse> DeleteDirectory(Deleting.DeleteRequest request)
        {
            SubscribeToAsyncFileOperationCallback();

            PS5TaskWrapperEmptyResponse response = new PS5TaskWrapperEmptyResponse();
            Deleting.Delete(request, response);
            return response.TaskCompletionSource.Task;
        }

        public static Task<PS5TaskWrapperReadFilesResponse> ReadFile(PS5TaskWrapperReadFileRequest request)
        {
            SubscribeToAsyncFileOperationCallback();

            PS5TaskWrapperReadFilesResponse response = new PS5TaskWrapperReadFilesResponse();
            FileOps.CustomFileOp(request, response);
            return response.TaskCompletionSource.Task;
        }

        public static Task<PS5TaskWrapperDeleteFileResponse> DeleteFile(PS5TaskWrapperDeleteFileRequest request)
        {
            SubscribeToAsyncFileOperationCallback();

            PS5TaskWrapperDeleteFileResponse response = new PS5TaskWrapperDeleteFileResponse();
            FileOps.CustomFileOp(request, response);
            return response.TaskCompletionSource.Task;
        }

        public static Task<PS5EnumerateFileResponse> EnumerateFiles(PS5TaskWrapperEnumerateFilesRequest request)
        {
            SubscribeToAsyncFileOperationCallback();

            PS5EnumerateFileResponse response = new PS5EnumerateFileResponse();
            FileOps.CustomFileOp(request, response);
            return response.TaskCompletionSource.Task;
        }

        public static Task<PS5TaskWrapperEnumerateArchivesResponse> EnumerateArchives(Searching.DirNameSearchRequest request)
        {
            SubscribeToAsyncFileOperationCallback();

            PS5TaskWrapperEnumerateArchivesResponse response = new PS5TaskWrapperEnumerateArchivesResponse();
            Searching.DirNameSearch(request, response);
            return response.TaskCompletionSource.Task;
        }

        public static Task<PS5TaskWrapperEmptyResponse> Unmount(Mounting.UnmountRequest request)
        {
            SubscribeToAsyncFileOperationCallback();

            PS5TaskWrapperEmptyResponse response = new PS5TaskWrapperEmptyResponse();
            Mounting.Unmount(request, response);
            return response.TaskCompletionSource.Task;
        }

        private static void SubscribeToAsyncFileOperationCallback()
        {
            if (m_IsInitialized) return;
            if (!m_IsInitialized)
            {
                Main.OnAsyncEvent += OnAsyncFileOperationEvent;
                m_IsInitialized = true;
            }
        }

        /// <summary>
        /// This method is required by PS5 and is invoked by "The system" when a file operation is completed.
        /// Optionally, it also contains the data that you've just written.
        ///
        /// The flow works as follows: You create a Request and a Response, then execute the request with the response as argument.
        /// Then this method is invoked (By the system) with the response containing the data (If any)
        /// </summary>
        /// <param name="callbackEvent">The response to the file request, containing the files</param>
        private static void OnAsyncFileOperationEvent(SaveDataCallbackEvent callbackEvent)
        {
            if (callbackEvent.Response is PS5TaskWrapperMountResponse mountingResponse)
            {
                mountingResponse.TaskCompletionSource.SetResult(mountingResponse);
                return;
            }

            if (callbackEvent.Response is PS5TaskWrapperWriteFileResponse fileResponse)
            {
                fileResponse.TaskCompletionSource.SetResult(fileResponse);
                return;
            }

            if (callbackEvent.Response is PS5TaskWrapperReadFilesResponse readFileResponse)
            {
                readFileResponse.TaskCompletionSource.SetResult(readFileResponse);
                return;
            }

            if (callbackEvent.Request is PS5TaskWrapperDeleteFileRequest &&
                callbackEvent.Response is PS5TaskWrapperDeleteFileResponse deleteFileEmptyResponse)
            {

                deleteFileEmptyResponse.TaskCompletionSource.SetResult(deleteFileEmptyResponse);
                return;
            }

            if (callbackEvent.Request is Deleting.DeleteRequest &&
                callbackEvent.Response is PS5TaskWrapperEmptyResponse response)
            {
                response.TaskCompletionSource.SetResult(response);
                return;
            }

            if (callbackEvent.Response is PS5EnumerateFileResponse enumerateFileResponse)
            {
                enumerateFileResponse.TaskCompletionSource.SetResult(enumerateFileResponse);
                return;
            }

            if (callbackEvent.Response is PS5TaskWrapperEnumerateArchivesResponse enumerateArchivesResponse)
            {
                enumerateArchivesResponse.TaskCompletionSource.SetResult(enumerateArchivesResponse);
                return;
            }

            if (callbackEvent.Request is Mounting.UnmountRequest &&
                callbackEvent.Response is PS5TaskWrapperEmptyResponse unmountEmptyResponse)
            {
                unmountEmptyResponse.TaskCompletionSource.SetResult(unmountEmptyResponse);
            }
        }
    }
}
