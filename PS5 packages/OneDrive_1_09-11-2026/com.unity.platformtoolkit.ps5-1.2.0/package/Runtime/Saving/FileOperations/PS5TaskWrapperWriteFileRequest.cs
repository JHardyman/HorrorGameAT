using System;
using System.IO;
using System.Threading.Tasks;
using Unity.SaveData.PS5.Info;
using Unity.SaveData.PS5.Mount;


namespace Unity.PlatformToolkit.PS5
{
    internal class PS5TaskWrapperWriteFileRequest : FileOps.FileOperationRequest
    {
        private readonly string m_FileName;
        private readonly byte[] m_Data;

        public PS5TaskWrapperWriteFileRequest(int userID, Mounting.MountPointName mountPointName, string fileName, byte[] dataToWrite)
        {
            m_FileName = fileName;
            m_Data = dataToWrite;
            UserId = userID;
            MountPointName = mountPointName;
        }

        public override void DoFileOperations(Mounting.MountPoint mountPoint, FileOps.FileOperationResponse response)
        {
            var savePath = Path.Combine(mountPoint.PathName.Data, m_FileName);
            File.WriteAllBytes(savePath, m_Data);
        }
    }

    internal class PS5TaskWrapperWriteFileResponse : FileOps.FileOperationResponse
    {
        public readonly TaskCompletionSource<PS5TaskWrapperWriteFileResponse> TaskCompletionSource = new TaskCompletionSource<PS5TaskWrapperWriteFileResponse>();
    }
}
