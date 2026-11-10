using System.IO;
using System.Threading.Tasks;
using Unity.SaveData.PS5.Core;
using Unity.SaveData.PS5.Info;
using Unity.SaveData.PS5.Mount;

namespace Unity.PlatformToolkit.PS5
{
    internal class PS5TaskWrapperDeleteFileRequest : FileOps.FileOperationRequest
    {
        private readonly string m_FileName;

        public PS5TaskWrapperDeleteFileRequest(int userID, Mounting.MountPointName mountPointName, string fileName)
        {
            m_FileName = fileName;
            UserId = userID;
            MountPointName = mountPointName;
        }

        public override void DoFileOperations(Mounting.MountPoint mountPoint, FileOps.FileOperationResponse response)
        {
            var fullPath = Path.Combine(mountPoint.PathName.Data, m_FileName);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }

    internal class PS5TaskWrapperDeleteFileResponse : FileOps.FileOperationResponse
    {
        public readonly TaskCompletionSource<PS5TaskWrapperDeleteFileResponse> TaskCompletionSource = new TaskCompletionSource<PS5TaskWrapperDeleteFileResponse>();
    }
}
