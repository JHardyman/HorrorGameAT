using System.Threading.Tasks;
using Unity.SaveData.PS5.Core;

namespace Unity.PlatformToolkit.PS5
{
    internal class PS5TaskWrapperEmptyResponse : EmptyResponse
    {
        public readonly TaskCompletionSource<PS5TaskWrapperEmptyResponse> TaskCompletionSource = new TaskCompletionSource<PS5TaskWrapperEmptyResponse>();
    }
}
