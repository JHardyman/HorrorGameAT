using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;


namespace Unity.PlatformToolkit.PS5
{
    internal class PS5StorageSystem : AbstractStorageSystem
    {
        private readonly int m_UserID;

        public PS5StorageSystem(int userID)
        {
            m_UserID = userID;
        }

        public override async Task DeleteArchive(string name)
        {
            var deleteDirectoryRequest = PS5RequestHelper.CreatePS5DeleteDirectoryRequest(m_UserID, name);
            await PS5SavingTaskWrapper.DeleteDirectory(deleteDirectoryRequest);
        }

        public override async Task<IReadOnlyList<string>> EnumerateArchives()
        {
            var enumerateArchivesRequest = PS5RequestHelper.CreatePS5SearchDirectoryRequest(m_UserID);
            var archives = await PS5SavingTaskWrapper.EnumerateArchives(enumerateArchivesRequest);
            return archives.SaveDataItems?.Select(item => item.DirName.Data).ToArray() ?? Array.Empty<string>();
        }

        public override async Task<IGenericArchive> GetReadOnlyArchive(string name)
        {
            return await CreateArchive(name, isReadWrite: false);
        }

        public override async Task<IGenericArchive> GetWriteOnlyArchive(string name)
        {
            return await CreateArchive(name, isReadWrite: true);
        }

        private async Task<PS5Archive> CreateArchive(string name, bool isReadWrite)
        {
            var enumerateArchivesRequest = PS5RequestHelper.CreatePS5SearchDirectoryRequest(m_UserID);
            var enumerateResponse = await PS5SavingTaskWrapper.EnumerateArchives(enumerateArchivesRequest);
            bool archiveExists = enumerateResponse?.SaveDataItems?.Any(item => item.DirName.Data == name) ?? false;
            // If its read only and nothing has been found, we need to throw file not found exception.
            if (!isReadWrite && !archiveExists)
            {
                throw new FileNotFoundException($"Archive not found: {name}");
            }

            var archive = new PS5Archive(m_UserID, name);
            await archive.Initialize(archiveExists, isReadWrite);
            return archive;
        }
    }
}
