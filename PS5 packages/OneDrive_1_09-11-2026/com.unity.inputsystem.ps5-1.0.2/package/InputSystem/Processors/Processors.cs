using System.ComponentModel;
using UnityEditor;
using UnityEngine;

namespace UnityEngine.InputSystem.PS5.Processors
{

    /// <summary>
    /// Processor to convert a Quaternion from Right Handed to Left Handed Coordinates Space
    /// </summary>
    #if UNITY_EDITOR
    [DesignTimeVisible(false)] //Hide in inspector UI
    [InitializeOnLoad]
    #endif
    internal class ConvertRightToLeftHandQuaternion : InputProcessor<Quaternion>
    {
        #if UNITY_EDITOR
        static ConvertRightToLeftHandQuaternion()
        {
            Initialize();
        }

        static void Initialize()
        {
            InputSystem.RegisterProcessor<ConvertRightToLeftHandQuaternion>();
        }

        #endif

        public override Quaternion Process(Quaternion value, InputControl control)
        {
            return new Quaternion(value.x, value.y, -value.z, -value.w);
        }
    }

    /// <summary>
    /// Processor to convert a Vector3 from Right Handed to Left Handed Coordinates Space
    /// </summary>
    #if UNITY_EDITOR
    [DesignTimeVisible(false)] //Hide in inspector UI
    [InitializeOnLoad]
    #endif
    internal class ConvertRightToLeftHandVector3 : InputProcessor<Vector3>
    {
        #if UNITY_EDITOR
        static ConvertRightToLeftHandVector3()
        {
            Initialize();
        }

        static void Initialize()
        {
            InputSystem.RegisterProcessor<ConvertRightToLeftHandVector3>();
        }
        #endif



        public override Vector3 Process(Vector3 value, InputControl control)
        {
            return new Vector3(value.x, value.y, -value.z);
        }
    }
}
