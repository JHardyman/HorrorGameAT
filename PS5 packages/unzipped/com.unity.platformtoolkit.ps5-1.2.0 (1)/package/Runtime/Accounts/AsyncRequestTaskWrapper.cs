using System.Threading.Tasks;
using Unity.PSN.PS5;
using Unity.PSN.PS5.UDS;
using Unity.PSN.PS5.Trophies;
using Unity.PSN.PS5.Aysnc;
using Unity.PSN.PS5.Users;

namespace Unity.PlatformToolkit.PS5
{
    // A wrapper to communicate with PS5 internal system.
    // Can be used for User System, UDS (Universal Data System) and trophy requests
    static internal class AsyncRequestTaskWrapper
    {
        public static Task<AsyncRequest<T>> AsyncRequestUserSystem<T>(T request) where T : Request
        {
            var source = new TaskCompletionSource<AsyncRequest<T>>();
            var requestOperation = MakeRequestOperation(request, source);
            UserSystem.Schedule(requestOperation);
            return source.Task;
        }

        public static Task<AsyncRequest<T>> AsyncRequestUniversalDataSystem<T>(T request) where T : Request
        {
            var source = new TaskCompletionSource<AsyncRequest<T>>();
            var requestOperation = MakeRequestOperation(request, source);
            UniversalDataSystem.Schedule(requestOperation);
            return source.Task;
        }

        public static Task<AsyncRequest<T>> AsyncRequestTrophySystem<T>(T request) where T : Request
        {
            var source = new TaskCompletionSource<AsyncRequest<T>>();
            var requestOperation = MakeRequestOperation(request, source);
            TrophySystem.Schedule(requestOperation);
            return source.Task;
        }

        private static AsyncAction<AsyncRequest<T>> MakeRequestOperation<T>(T request, TaskCompletionSource<AsyncRequest<T>> source) where T : Request
        {
            var requestOp = new AsyncRequest<T>(request).ContinueWith((result) =>
            {
                if (result.Request.Result.RaiseException)
                {
                    source.SetException(new PSNException(result.Request.Result));
                }
                else
                {
                    source.SetResult(result);
                }
            });
            return requestOp;
        }
    }
}
