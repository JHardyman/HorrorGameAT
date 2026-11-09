using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System;
using Unity.SaveData.PS5.Core;
using Unity.SaveData.PS5.Mount;
using UnityEngine;

namespace Unity.PlatformToolkit.PS5
{
    internal class PS5Archive : AbstractArchive
    {
        private readonly int m_UserID;
        private bool m_SaveExists;

        private readonly string m_SaveName;
        private bool m_TriedToCommit = false;
        private Mounting.MountPoint m_MountPoint;
        private bool m_IsMountedForWritingAndDeleting = false;
        private IReadOnlyList<string> m_Files = null;

        public PS5Archive(int userID, string archiveName)
             : base(doDataBuffering: false)
        {
            m_UserID = userID;
            m_SaveName = archiveName;
        }

        public async Task Initialize(bool saveExists, bool isReadWrite)
        {
            m_SaveExists = saveExists;
            await Mount(isReadWrite);

            if (saveExists)
            {
                var files = await EnumerateFiles();
                if (files.Count == 0)
                {
                    await UnmountNoCommit();
                    throw new CorruptedSaveException("Corrupted save found, no files are in the save");
                }
            }
        }

        private async Task UnmountNoCommit()
        {
            var unMountRequest =
                PS5RequestHelper.CreatePS5UnmountRequest(m_UserID,m_MountPoint.PathName,commitData: false);
            var unmountResponse = await PS5SavingTaskWrapper.Unmount(unMountRequest);
            if (unmountResponse.ReturnCode != ReturnCodes.SUCCESS)
            {
                throw new IOException($"Unable to unmount. Error code: {unmountResponse.ReturnCode:X}");
            }
        }

        public override async Task<IReadOnlyList<string>> EnumerateFiles()
        {
            if (m_MountPoint != null)
            {
                var enumerateFileRequest = new PS5TaskWrapperEnumerateFilesRequest(m_UserID, m_MountPoint);
                var enumerateResponse = await PS5SavingTaskWrapper.EnumerateFiles(enumerateFileRequest);
                if (enumerateResponse.ReturnCode != ReturnCodes.SUCCESS)
                {
                    throw new IOException($"Unable to enumerate files. Error code:{enumerateResponse.ReturnCode}");
                }
                m_Files = enumerateResponse.Result;
            }
            return m_Files;
        }

        protected override async Task<byte[]> GetDataFromStorage(string name)
        {
            if (!m_SaveExists)
            {
                throw new FileNotFoundException($"Save does not exist: {name}");
            }

            var request =
                new PS5TaskWrapperReadFileRequest(m_UserID, m_MountPoint.PathName, name);
            var data = await PS5SavingTaskWrapper.ReadFile(request);

            if (data.fileData == null)
            {
                throw new FileNotFoundException($"File not found: {name}");
            }

            return data.fileData;
        }

        protected override async Task CommitDataToStorage(IReadOnlyCollection<(string name, byte[] data)> writtenFiles,
            IReadOnlyCollection<string> deletedFiles)
        {
            m_TriedToCommit = true;
            var files = await EnumerateFiles();
            if (files.Count == 0 && !m_SaveExists)
            {
                await UnmountNoCommit();
                await DeleteSave();
            }
            if (files.Count == 0)
            {
                throw new InvalidOperationException("Cannot commit empty save");
            }

            var unmountCommitRequest =
                PS5RequestHelper.CreatePS5UnmountRequest(m_UserID, m_MountPoint.PathName, commitData: true);
            var unmountResponse = await PS5SavingTaskWrapper.Unmount(unmountCommitRequest);
            if (unmountResponse.ReturnCode != ReturnCodes.SUCCESS)
            {
                throw new IOException($"Unable to unmount. Error code: {unmountResponse.ReturnCode:X}");
            }

            //Dispose method could be called after this so we should not have a mount point available.
            m_MountPoint = null;
        }

        protected override async Task WriteDataToStorage(string name, byte[] data)
        {
            var request = new PS5TaskWrapperWriteFileRequest(m_UserID, m_MountPoint.PathName, name,
                data);
            var response = await PS5SavingTaskWrapper.WriteFile(request);
            if (response.ReturnCode == ReturnCodes.DATA_ERROR_NO_SPACE_FS)
            {
                throw new NotEnoughSpaceException(
                    $"Not enough disk space to save files into save. Error code: {ReturnCodes.DATA_ERROR_NO_SPACE_FS}.");
            }

            if (response.ReturnCode != ReturnCodes.SUCCESS)
            {
                await UnmountNoCommit();

                m_MountPoint = null;
                throw new IOException(
                    $"Error trying to write file. Error code: {response.ReturnCode:X}.");
            }
        }

        protected override async Task RemoveDataFromStorage(string name)
        {
            var request = new PS5TaskWrapperDeleteFileRequest(m_UserID, m_MountPoint.PathName, name);
            var response = await PS5SavingTaskWrapper.DeleteFile(request);
            if (response.ReturnCode != ReturnCodes.SUCCESS)
            {
                await UnmountNoCommit();

                m_MountPoint = null;
                throw new IOException(
                    $"Error trying to delete file. Error code: {response.ReturnCode:X}.");
            }
        }

        public override async ValueTask DisposeInternalAsync(bool explicitDispose)
        {
            if (m_MountPoint != null && m_MountPoint.IsMounted)
            {
                await UnmountNoCommit();
            }

            if (m_IsMountedForWritingAndDeleting)
            {
                // This means we've opened a save writable, potentially wrote a file to it and closed the save without committing.
                // If we commit, m_triedToCommit would be true
                if (m_MountPoint != null && !m_SaveExists && !m_TriedToCommit)
                {
                    await DeleteSave();
                }
            }

            m_MountPoint = null;
            await base.DisposeInternalAsync(explicitDispose);
        }

        public override void DisposeInternal(bool explicitDispose)
        {
            if (m_MountPoint != null && m_MountPoint.IsMounted)
            {
                UnmountNoCommit().GetAwaiter().GetResult();
            }

            if (m_IsMountedForWritingAndDeleting)
            {
                if (m_MountPoint != null && !m_SaveExists && !m_TriedToCommit)
                {
                    DeleteSave().GetAwaiter().GetResult();
                }
            }

            m_MountPoint = null;
            base.DisposeInternal(explicitDispose);
        }

        private async Task DeleteSave()
        {
            var directoryDeletionRequest = PS5RequestHelper.CreatePS5DeleteDirectoryRequest(m_UserID, m_MountPoint.DirName.Data);
            await PS5SavingTaskWrapper.DeleteDirectory(directoryDeletionRequest);
        }

        private async Task Mount(bool isReadWrite)
        {
           var mountRequest =
               PS5RequestHelper.CreatePS5MountingRequest(m_UserID, m_SaveName, readWrite: isReadWrite, PS5PlatformToolkit.MaxSaveSizeInBytes);
           var response = await WaitForMountingWhileBusy(mountRequest);
           if (response.ReturnCode != ReturnCodes.SUCCESS)
           {
               throw new IOException($"Unable to mount. Error code: {response.ReturnCode:X}");
           }
           m_MountPoint = response.MountPoint;
           if (isReadWrite)
           {
               m_IsMountedForWritingAndDeleting = true;
           }
        }

        private async Task<PS5TaskWrapperMountResponse> WaitForMountingWhileBusy(Mounting.MountRequest mountRequest)
        {
            PS5TaskWrapperMountResponse mountResponse;
            // We could be busy writing files, in which case we cannot write to disk yet.
            do
            {
                mountResponse = await PS5SavingTaskWrapper.Mount(mountRequest);
                await Task.Yield();
            } while (mountResponse.ReturnCode == ReturnCodes.SAVE_DATA_ERROR_BUSY);

            return mountResponse;
        }
    }
}
