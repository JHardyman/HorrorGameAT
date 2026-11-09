using System.Threading.Tasks;
using Unity.SaveData.PS5.Search;

namespace Unity.PlatformToolkit.PS5
{
    internal class PS5TaskWrapperEnumerateArchivesResponse : Searching.DirNameSearchResponse
    {
        public readonly TaskCompletionSource<PS5TaskWrapperEnumerateArchivesResponse> TaskCompletionSource = new TaskCompletionSource<PS5TaskWrapperEnumerateArchivesResponse>();
    }
}
