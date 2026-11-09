using System.IO;
using System.Threading.Tasks;
using Unity.SaveData.PS5.Info;
using Unity.SaveData.PS5.Mount;
using UnityEngine;

namespace Unity.PlatformToolkit.PS5
{
    internal class PS5TaskWrapperReadFileRequest : FileOps.FileOperationRequest
    {
        private readonly string m_FileName;

        public PS5TaskWrapperReadFileRequest(int userID, Mounting.MountPointName mountPointName, string fileName)
        {
            UserId = userID;
            m_FileName = fileName;
            MountPointName = mountPointName;
        }

        public override void DoFileOperations(Mounting.MountPoint mountPoint, FileOps.FileOperationResponse response)
        {
            PS5TaskWrapperReadFilesResponse pS5TaskWrapperReadFilesResponse =
                response as PS5TaskWrapperReadFilesResponse;
            string fullPath = Path.Combine(mountPoint.PathName.Data, m_FileName);
            if (File.Exists(fullPath))
            {
                pS5TaskWrapperReadFilesResponse.fileData = File.ReadAllBytes(fullPath);
            }
            else
            {
                pS5TaskWrapperReadFilesResponse.fileData = null;
            }
        }
    }

    internal class PS5TaskWrapperReadFilesResponse : FileOps.FileOperationResponse
    {
        public TaskCompletionSource<PS5TaskWrapperReadFilesResponse> TaskCompletionSource = new TaskCompletionSource<PS5TaskWrapperReadFilesResponse>();
        public byte[] fileData;
    }
}
