using System.Threading.Tasks;
using Unity.SaveData.PS5.Mount;

namespace Unity.PlatformToolkit.PS5
{
    internal class PS5TaskWrapperMountResponse : Mounting.MountResponse
    {
        public TaskCompletionSource<PS5TaskWrapperMountResponse> TaskCompletionSource = new TaskCompletionSource<PS5TaskWrapperMountResponse>();
    }
}
