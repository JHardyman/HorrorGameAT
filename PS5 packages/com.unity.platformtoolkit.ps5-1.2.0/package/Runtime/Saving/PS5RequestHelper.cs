using Unity.SaveData.PS5.Core;
using Unity.SaveData.PS5.Delete;
using Unity.SaveData.PS5.Mount;
using Unity.SaveData.PS5.Search;
using UnityEngine;

namespace Unity.PlatformToolkit.PS5
{
    internal class PS5RequestHelper
    {
        public static Mounting.MountRequest CreatePS5MountingRequest(int userID, string dirName, bool readWrite, ulong maxCapacityInBytes)
        {
            DirName PS5DirName = new DirName
            {
                Data = dirName,
            };

            return new Mounting.MountRequest
            {
                UserId = userID,
                Async = true,
                DirName = PS5DirName,
                MountMode = readWrite
                    ? Mounting.MountModeFlags.Create2 | Mounting.MountModeFlags.ReadWrite
                    : Mounting.MountModeFlags.ReadOnly,
                PrepareMode = Mounting.PrepareMode.EnableCancel,
                Blocks = CalculateBlocksSize(maxCapacityInBytes)
            };
        }

        private static ulong CalculateBlocksSize(ulong maxCapacityInBytes)
        {
            if (maxCapacityInBytes / Mounting.MountRequest.BLOCK_SIZE >=
                Mounting.MountRequest.BLOCKS_MAX)
            {
                return Mounting.MountRequest.BLOCKS_MAX;
            }

            if ((maxCapacityInBytes / Mounting.MountRequest.BLOCK_SIZE) < Mounting.MountRequest.BLOCKS_MIN)
            {
                return Mounting.MountRequest.BLOCKS_MIN;
            }

            return maxCapacityInBytes / Mounting.MountRequest.BLOCK_SIZE;
        }

        public static Deleting.DeleteRequest CreatePS5DeleteDirectoryRequest(int userID, string dirName)
        {
            DirName PS5DirName = new DirName
            {
                Data = dirName
            };

            return new Deleting.DeleteRequest
            {
                UserId = userID,
                Async = true,
                DirName = PS5DirName,
            };
        }

        public static Searching.DirNameSearchRequest CreatePS5SearchDirectoryRequest(int userID)
        {
            return new Searching.DirNameSearchRequest
            {
                UserId = userID,
                Key = Searching.SearchSortKey.Time,
                Order = Searching.SearchSortOrder.Ascending,
                IncludeBlockInfo = false,
                IncludeParams = false,
                MaxDirNameCount = Searching.DirNameSearchRequest.DIR_NAME_MAXSIZE
            };
        }

        public static Searching.DirNameSearchRequest CreatePS5AvailableSpaceRequest(int userID)
        {
            return new Searching.DirNameSearchRequest
            {
                UserId = userID,
                Key = Searching.SearchSortKey.Time,
                Order = Searching.SearchSortOrder.Ascending,
                IncludeBlockInfo = true,
                IncludeParams = false,
                MaxDirNameCount = Searching.DirNameSearchRequest.DIR_NAME_MAXSIZE
            };
        }

        public static Mounting.UnmountRequest CreatePS5UnmountRequest(int userID,
            Mounting.MountPointName mountPointName, bool commitData)
        {
            return new Mounting.UnmountRequest
            {
                UserId = userID,
                Async = true,
                MountPointName = mountPointName,
                UnmountMode = commitData ? Mounting.UnmountMode.Commit : Mounting.UnmountMode.Cancel
            };
        }
    }
}
