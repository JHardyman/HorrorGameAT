using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Unity.SaveData.PS5.Info;
using Unity.SaveData.PS5.Mount;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;

namespace Unity.PlatformToolkit.PS5
{
    internal class PS5TaskWrapperEnumerateFilesRequest : FileOps.FileOperationRequest
    {
        // This pattern searches for every file within the folder, with any extension.
        private readonly string m_SearchPattern = "*.*";
        public PS5TaskWrapperEnumerateFilesRequest(int userID , Mounting.MountPoint mountPoint)
        {
            UserId = userID;
            MountPointName = mountPoint.PathName;
        }

        public override void DoFileOperations(Mounting.MountPoint mountPoint, FileOps.FileOperationResponse response)
        {
            PS5EnumerateFileResponse PS5Response = response as PS5EnumerateFileResponse;
            // We expect only the file name, not folder name + file name. So we get the file name only.
            var fullPathFiles = Directory.GetFiles(mountPoint.PathName.Data, m_SearchPattern, SearchOption.AllDirectories);
            PS5Response.Result = fullPathFiles.Select(file => Path.GetFileName(file)).ToArray();
        }
    }

    internal class PS5EnumerateFileResponse : FileOps.FileOperationResponse
    {
        public readonly TaskCompletionSource<PS5EnumerateFileResponse> TaskCompletionSource =
            new TaskCompletionSource<PS5EnumerateFileResponse>();
        public string[] Result;
    }
}
