using System;
using Unity.SaveData.PS5.Core;
using UnityEngine;

namespace Unity.PlatformToolkit.PS5
{
    internal class PS5SaveDataException : Exception
    {
        public ReturnCodes ReturnCode { get; }

        public PS5SaveDataException(ReturnCodes returnCode) : base($"PS5 Saving Data failed with error: {returnCode.ToString()}")
        {
            ReturnCode = returnCode;
        }
    }
}
