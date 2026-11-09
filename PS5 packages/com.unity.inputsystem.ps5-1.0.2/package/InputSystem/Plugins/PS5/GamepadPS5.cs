#if UNITY_EDITOR || UNITY_PS5 || PACKAGE_DOCS_GENERATION
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using System;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.PS5.LowLevel;

namespace UnityEngine.InputSystem.PS5
{

/* example of how to set controller trigger effect

            var pad = UnityEngine.InputSystem.PS5.DualSenseGamepad.GetBySlotIndex(0);
            var effect = new  UnityEngine.InputSystem.PS5.TriggerEffectParam( UnityEngine.InputSystem.PS5.TriggerEffectMask.L2 | UnityEngine.InputSystem.PS5.TriggerEffectMask.R2);

            effect.left.mode =UnityEngine.InputSystem.PS5.TriggerEffectMode.Feedback;
            effect.left.feedback.position=2;
            effect.left.feedback.strength=8;

            effect.right.mode = UnityEngine.InputSystem.PS5.TriggerEffectMode.Weapon;
            effect.right.weapon.startPosition= 2;
            effect.right.weapon.endPosition= 8;
            effect.right.weapon.strength= 8;

            pad.SetTriggerEffect(effect);
*/

    /// <summary>
    /// The mode of the trigger effect.
    /// </summary>
    public enum TriggerEffectMode
    {
        /// <summary>
        /// Disables the current effect
        /// </summary>
        Off,

        /// <summary>
        /// Feedback effect.
        /// <seealso cref="TriggerEffectFeedbackParam"/>
        /// </summary>
        Feedback,

        /// <summary>
        /// Weapon effect.
        /// </summary>
        /// <seealso cref="TriggerEffectWeaponParam"/>
        Weapon,

        /// <summary>
        /// Vibration effect.
        /// </summary>
        /// <seealso cref="TriggerEffectVibrationParam"/>
        Vibration,

        /// <summary>
        /// Feedback at multiple positions along the trigger.
        /// </summary>
        /// <seealso cref="TriggerEffectMultiplePositionFeedbackParam"/>
        MultiplePositionFeedback,

        /// <summary>
        /// Sloped feedback.
        /// </summary>
        /// <seealso cref="TriggerEffectSlopeFeedbackParam"/>
        SlopeFeedback,

        /// <summary>
        /// Vibration at multiple positions along the trigger.
        /// </summary>
        /// <seealso cref="TriggerEffectMultiplePositionVibrationParam"/>
        MultiplePositionVibration
    }

    /// <summary>
    /// Mask of the left and right triggers.
    /// Use TriggerEffectMask to select the trigger to apply an effect.
    /// </summary>
    [Flags]
    public enum TriggerEffectMask : byte
    {
        L2 = 1,
        R2 = 2
    }


    /// <summary>
    /// Parameter for setting a feedback trigger effect on a DualSense trigger.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 48)]
    public struct TriggerEffectFeedbackParam
    {
        /// <summary>
        /// Position to start the trigger effect. Use a value between `0` and `9`.
        /// </summary>
        [FieldOffset(0)] public byte position;

        /// <summary>
        /// The strength of the feedback.
        /// </summary>
        /// <remarks>
        ///   Use a value `0` to turn off feedback.  Use a value between `1 ` and  `8` to set the feedback strength.
       /// </remarks>
        [FieldOffset(1)] public byte strength;
    }


    /// <summary>
    /// Parameter for setting a vibration trigger effect of the DualSense triggers.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 48)]
    public struct TriggerEffectVibrationParam
    {
        /// <summary>
        /// The position to start the trigger effect.
        /// </summary>
        /// <remarks>
        ///  Use a value between `0` and  `9` to set the trigger effect start position.
        /// </remarks>
        [FieldOffset(0)] public byte position;

        /// <summary>
        /// Strength of the trigger feedback.
        /// </summary>
        /// <remarks>
        /// Use a value between `1 ` and  `8` to set the feedback strength. Use '0' to turn off trigger feedback.
        /// </remarks>
        [FieldOffset(1)] public byte amplitude;

        /// <summary>
        /// Frequency of the trigger feedback, (0 = off, 1 to 255 = vibrations)
        /// </summary>
        /// <remarks>
        /// Use a value between `1 ` and  `255` to set the frequency of vibrations. Use '0' to turn off vibrations.
        /// </remarks>
        [FieldOffset(2)] public byte frequency;
    }


    /// <summary>
    /// Parameter for setting a weapon trigger effect on a DualSense trigger.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 48)]
    public struct TriggerEffectWeaponParam
    {
        /// <summary>
        /// Position to start the trigger effect (2 - 7)
        /// </summary>
        [FieldOffset(0)] public byte startPosition;

        /// <summary>
        /// Position to end the trigger effect ([<see cref="startPosition"/> + 1] - 8)
        /// </summary>
        [FieldOffset(1)] public byte endPosition;

        /// <summary>
        /// Strength of the effect.
        /// </summary>
        /// <remarks>
        ///  Use a value between `1 ` and  `8` to set the effect strength. Use `0` to turn the effect off.
        /// </remarks>
        [FieldOffset(2)] public byte strength;
    }

    /// <summary>
    /// Parameter for setting trigger feedback at different positions of the DualSense trigger.
    /// </summary>
    /// <remarks>
    /// Each field `strengthX (X: 0-9)` represents a different strength value.
    /// Use values `1` to `8` to set the strength at the desired trigger position. Use `0` to turn off the effect.
    /// </remarks>
    [StructLayout(LayoutKind.Explicit, Size = 48)]
    public struct TriggerEffectMultiplePositionFeedbackParam
    {
        [FieldOffset(0)] public byte strength0;
        [FieldOffset(1)] public byte strength1;
        [FieldOffset(2)] public byte strength2;
        [FieldOffset(3)] public byte strength3;
        [FieldOffset(4)] public byte strength4;
        [FieldOffset(5)] public byte strength5;
        [FieldOffset(6)] public byte strength6;
        [FieldOffset(7)] public byte strength7;
        [FieldOffset(8)] public byte strength8;
        [FieldOffset(9)] public byte strength9;
    }

    /// <summary>
    /// Parameter for setting a Slope of feedback on the Dualsense Triggers
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 48)]
    public struct TriggerEffectSlopeFeedbackParam
    {
        /// <summary>
        /// Position to start the trigger effect (0-8)
        /// </summary>
        [FieldOffset(0)] public byte startPosition;

        /// <summary>
        /// Position to end the trigger effect ([<see cref="startPosition"/> + 1] - 8)
        /// </summary>
        [FieldOffset(1)] public byte endPosition;

        /// <summary>
        /// Strength of the effect at the <see cref="startPosition"/>
        /// </summary>
        [FieldOffset(2)] public byte startStrength;

        /// <summary>
        /// Strength of the effect at the <see cref="endPosition"/>
        /// </summary>
        [FieldOffset(3)] public byte endStrength;
    }

    /// <summary>
    /// Parameter for setting vibration at different positions of the DualSense trigger.
    /// </summary>
    /// <remarks>
    /// Each field `strengthX (X: 0-9)` represents a different vibration value.
    /// Use values `1` to `8` to set the vibration strength at the desired trigger position. Use `0` to turn off the vibration.
    /// </remarks>
    [StructLayout(LayoutKind.Explicit, Size = 48)]
    public struct TriggerEffectMultiplePositionVibrationParam
    {
        /// <summary>
        /// The frequency of the trigger feedback.
        /// </summary>
        /// <remarks>
        /// Use values `1` to `255` to set the vibration frequency. Use `0` to turn off the vibration.
        /// </remarks>
        [FieldOffset(0)] public byte frequency;
        [FieldOffset(1)] public byte amplitude0;
        [FieldOffset(2)] public byte amplitude1;
        [FieldOffset(3)] public byte amplitude2;
        [FieldOffset(4)] public byte amplitude3;
        [FieldOffset(5)] public byte amplitude4;
        [FieldOffset(6)] public byte amplitude5;
        [FieldOffset(7)] public byte amplitude6;
        [FieldOffset(8)] public byte amplitude7;
        [FieldOffset(9)] public byte amplitude8;
        [FieldOffset(10)] public byte amplitude9;
    }

    /// <summary>
    /// Command to set the trigger effect mode and parameters.
    /// **Note**: Set the mode and the matching parameter. Only one parameter should be set.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 56)]
    public struct TriggerEffectCommand
    {
        /// <summary>
        /// Mode of the trigger effect.
        /// </summary>
        [FieldOffset(0)] public TriggerEffectMode mode;

        /// <summary>
        /// Weapon Effect parameter. Use with the mode set to <see cref="TriggerEffectMode.Feedback"/>.
        /// </summary>
        [FieldOffset(8)] public TriggerEffectWeaponParam weapon;

        /// <summary>
        /// Vibration Effect parameter. Use with the mode set to  <see cref="TriggerEffectMode.Vibration"/>.
        /// </summary>
        [FieldOffset(8)] public TriggerEffectVibrationParam vibration;

        /// <summary>
        /// Feedback Effect parameter. Use with the mode set to <see cref="TriggerEffectMode.Feedback"/>.
        /// </summary>
        [FieldOffset(8)] public TriggerEffectFeedbackParam feedback;

        /// <summary>
        /// Multiple Position Feedback parameter. Use with the mode set to <see cref="TriggerEffectMode.MultiplePositionFeedback"/>
        /// </summary>
        [FieldOffset(8)] public TriggerEffectMultiplePositionFeedbackParam multiplePositionFeedback;

        /// <summary>
        /// Slope Feedback parameter. Use with the mode set to <see cref="TriggerEffectMode.SlopeFeedback"/>.
        /// </summary>
        [FieldOffset(8)] public TriggerEffectSlopeFeedbackParam slopeFeedback;

        /// <summary>
        /// Multiple Position Vibration parameter. Use with the Mode set to <see cref="TriggerEffectMode.MultiplePositionVibration"/>.
        /// </summary>
        [FieldOffset(8)] public TriggerEffectMultiplePositionVibrationParam multiplePositionVibration;
    }


    /// <summary>
    /// Parameter for setting a trigger effect for a specific DualSense trigger.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = kSize)]
    public struct TriggerEffectParam : IInputDeviceCommandInfo
    {
        public static FourCC Type { get { return new FourCC('T', 'R', 'I', 'G'); } }


        internal const int kSize = InputDeviceCommand.BaseCommandSize + 8+56+56;

        [FieldOffset(0)] public InputDeviceCommand baseCommand;

        /// <summary>
        /// Mask of triggers to apply the effect to
        /// </summary>
        [FieldOffset(InputDeviceCommand.BaseCommandSize +0)]  public TriggerEffectMask triggerMask;

        /// <summary>
        /// Effect to set for the left trigger
        /// </summary>
        [FieldOffset(InputDeviceCommand.BaseCommandSize +8)]  public TriggerEffectCommand left;

        /// <summary>
        /// Effect to set for the right trigger
        /// </summary>
        [FieldOffset(InputDeviceCommand.BaseCommandSize +8+56)]  public TriggerEffectCommand right;


        public FourCC typeStatic
        {
            get { return Type; }
        }

        public TriggerEffectParam(TriggerEffectMask mask)
        {
             this= default(TriggerEffectParam);
             baseCommand = new InputDeviceCommand(Type, kSize);
             triggerMask=mask;
        }

        public static TriggerEffectParam Create()
        {
            return new TriggerEffectParam
            {
                baseCommand = new InputDeviceCommand(Type, kSize)
            };
        }
    }

    /// <summary>
    /// Type of Device used
    /// </summary>
    public enum DeviceClass
    {
        Standard     = 0,
        Guitar       = 1,
        Drum         = 2,
        DjTurntable  = 3,
        Dancemat     = 4,
        Navigation   = 5,
        SteeringWheel = 6,
        Stick        = 7,
        FlightStick  = 8,
        Gun          = 9
    }

    /// <summary>
    /// Deadzones of the left and right sticks
    /// </summary>
    public struct StickInformation
    {
        /// <summary>
        /// Stick deadzone relative to the magnitude of the stick axis
        /// </summary>
        /// <remarks>This value is set in the System Software by an end user for a DualSense Edge controller</remarks>
        public float leftStickDeadzone, rightStickDeadzone;
    }

    /// <summary>
    /// Infomation about the touchpad
    /// </summary>
    public struct TouchPadInformation
    {
        /// <summary>
        /// Density of the touchpad in dots/mm
        /// </summary>
        public float pixelDensity;
        /// <summary>
        /// Size of the touchpad in pixels
        /// </summary>
        public ushort resolutionX, resolutionY;
    }

    /// <summary>
    /// Mode in which the gamepad is connected to the device
    /// </summary>
    public enum ConnectionType
    {
        /// <summary>
        /// Connected to the device locally
        /// </summary>
        Local = 0,

        //1 is unused

        /// <summary>
        /// Connected as a DualSense controller remotely
        /// </summary>
        RemoteDualsense = 2,
    }

    /// <summary>
    /// Vibration mode of the controller
    /// </summary>
    public enum VibrationMode
    {
        /// <summary>
        /// Mode to control vibration via an AudioClip
        /// </summary>
        /// <remarks>Play vibration audio via AudioSource.PlayOnGamepad</remarks>
        /// <remarks>Vibration played via <see cref="DualSenseGamepad.SetMotorSpeeds"/> will not function in this mode</remarks>
        Advanced = 0x1,
        /// <summary>
        /// Mode to control vibration via a sine wave
        /// </summary>
        /// <remarks>Use <see cref="DualSenseGamepad.SetMotorSpeeds"/> to set vibration in this mode</remarks>
        Compatible = 0x2,
        /// <summary>
        /// Mode to control vibration via a sine wave that is closer to a Dualshock 4 Controller
        /// </summary>
        /// <remarks>Use <see cref="DualSenseGamepad.SetMotorSpeeds"/> to set vibration in this mode</remarks>
        Compatible2 = 0x3,
        /// <summary>
        /// Mode to allow vibration with <see cref="Compatible"/> and <see cref="Advanced"/>> together without
        /// having to switch between the two modes
        /// </summary>
        AdvancedAndCompatible = 0x4,
        /// <summary>
        /// Mode to allow vibration with <see cref="Compatible2"/> and <see cref="Advanced"/>> together without
        /// having to switch between the two modes
        /// </summary>
        AdvancedAndCompatible2 = 0x5
    }
}

#pragma warning disable 0649
namespace UnityEngine.InputSystem.PS5.LowLevel
{
    // IMPORTANT: State layout must match with GamepadInputStatePS5 in native.
    [StructLayout(LayoutKind.Explicit, Size = 92)]
    internal struct GamepadStatePS5 : IInputStateTypeInfo
    {
        public static FourCC kFormat => new FourCC('P', '4', 'G', 'P');

        private enum Button
        {
            L3 = 1,
            R3 = 2,
            Options = 3,
            DpadUp = 4,
            DpadRight = 5,
            DpadDown = 6,
            DpadLeft = 7,
            L2 = 8,
            R2 = 9,
            L1 = 10,
            R1 = 11,
            Triangle = 12,
            Circle = 13,
            Cross = 14,
            Square = 15,
            TouchPad = 20,
        }

        [InputControl(name = "leftStickPress", bit = (uint)Button.L3, displayName = "L3")]
        [InputControl(name = "rightStickPress", bit = (uint)Button.R3, displayName = "R3")]
        [InputControl(name = "start", layout = "Button", bit = (uint)Button.Options)]
        [InputControl(name = "dpad", layout = "Dpad", sizeInBits = 4, bit = 4)]
        [InputControl(name = "dpad/up", bit = (uint)Button.DpadUp)]
        [InputControl(name = "dpad/right", bit = (uint)Button.DpadRight)]
        [InputControl(name = "dpad/down", bit = (uint)Button.DpadDown)]
        [InputControl(name = "dpad/left", bit = (uint)Button.DpadLeft)]
        [InputControl(name = "leftTriggerButton", layout = "Button", bit = (uint)Button.L2, displayName = "L2")]
        [InputControl(name = "rightTriggerButton", layout = "Button", bit = (uint)Button.R2, displayName = "R2")]
        [InputControl(name = "leftShoulder", bit = (uint)Button.L1, displayName = "L1")]
        [InputControl(name = "rightShoulder", bit = (uint)Button.R1, displayName = "R1")]
        [InputControl(name = "buttonWest", bit = (uint)Button.Square, displayName = "Square")]
        [InputControl(name = "buttonSouth", bit = (uint)Button.Cross, displayName = "Cross")]
        [InputControl(name = "buttonEast", bit = (uint)Button.Circle, displayName = "Circle")]
        [InputControl(name = "buttonNorth", bit = (uint)Button.Triangle, displayName = "Triangle")]
        [InputControl(name = "touchpadButton", layout = "Button", bit = (uint)Button.TouchPad, displayName = "TouchPad")]
        [InputControl(name = "select", layout = "Button", bit = 21, displayName = "Select")]	// dummy value to override base layout "select" value in struct GamepadState in Gamepad.cs. Failure to have this results in a "Button West" InputAction.performed delegate triggering an additional "Select" InputAction.performed delegate due to both being configured as bit15
        [FieldOffset(0)]
        public uint buttons;

        /// <summary>
        /// Left stick position.
        /// </summary>
        [InputControl(layout = "Stick")]
        [FieldOffset(4)]
        public Vector2 leftStick;

        /// <summary>
        /// Right stick position.
        /// </summary>
        [InputControl(layout = "Stick")]
        [FieldOffset(12)]
        public Vector2 rightStick;

        /// <summary>
        /// Position of the left trigger.
        /// </summary>
        [InputControl]
        [FieldOffset(20)]
        public float leftTrigger;

        /// <summary>
        /// Position of the right trigger.
        /// </summary>
        [InputControl]
        [FieldOffset(24)]
        public float rightTrigger;

        /// <summary>
        /// The acceleration of the controller in the x, y, and z directions.
        /// </summary>
        [InputControl(name = "acceleration", displayName = "Acceleration", noisy = true, processors = "ConvertRightToLeftHandVector3()")]
        [FieldOffset(28)]
        public Vector3 acceleration;

        /// <summary>
        /// The rotation of the controller.
        /// </summary>
        [InputControl(name = "orientation", displayName = "Orientation", noisy = true, processors = "ConvertRightToLeftHandQuaternion()")]
        [FieldOffset(40)]
        public Quaternion orientation;

        /// <summary>
        /// The angular velocity of the controller in the x, y, and z directions.
        /// </summary>
        [InputControl(name = "angularVelocity", displayName = "Angular Velocity", noisy = true, processors = "ConvertRightToLeftHandVector3(), InvertVector3()")]
        [FieldOffset(56)]
        public Vector3 angularVelocity;

        /// <summary>
        /// Returns the position of the first touch on the touchpad.
        /// </summary>
        [InputControl(displayName = "Touch #0", aliases = new[] { "Primary Touch", "Touch 0", "Touchpad Touch 0" })]
        [FieldOffset(68)]
        public PS5Touch touch0;

        /// <summary>
        /// Returns the position of the second touch on the touchpad.
        /// </summary>
        [InputControl(displayName = "Touch #1", aliases = new[] { "Secondary Touch", "Touch 1", "Touchpad Touch 1" })]
        [FieldOffset(80)]
        public PS5Touch touch1;

        public FourCC format
        {
            get { return kFormat; }
        }
    }

    /// <summary>
    /// PS5 output report sent as command to backend.
    /// </summary>
    // IMPORTANT: Struct must match the DualShockPS5OutputReport2 in native
    [StructLayout(LayoutKind.Explicit, Size = kSize)]
    internal struct PS5GamepadOuputCommand : IInputDeviceCommandInfo
    {
        public static FourCC Type { get { return new FourCC('P', 'S', 'G', 'O'); } }

#if UNITY_PS5_ENHANCED_INPUT_SYSTEM
        internal const int kSize = InputDeviceCommand.BaseCommandSize + 12;
#else
        internal const int kSize = InputDeviceCommand.BaseCommandSize + 6;
#endif

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "No better term for underlying data.")]
        [Flags]
        public enum Flags : byte
        {
            Rumble = 0x1,
            Color = 0x2,
            ResetColor = 0x4,
            ResetOrientation = 0x8,
#if UNITY_PS5_ENHANCED_INPUT_SYSTEM
            VibrationMode = 0x10,
            MotionSensorState = 0x20,
            TiltCorrectionState = 0x40,
#endif
        }

        [FieldOffset(0)] public InputDeviceCommand baseCommand;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "flags", Justification = "No better term for underlying data.")]
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 0)] public byte flags;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 1)] public byte largeMotorSpeed;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 2)] public byte smallMotorSpeed;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 3)] public byte redColor;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 4)] public byte greenColor;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 5)] public byte blueColor;
#if UNITY_PS5_ENHANCED_INPUT_SYSTEM
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 6)] public byte motionSensor;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 7)] public byte tiltCorrection;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 8)] public VibrationMode vibrationMode;
#endif

        public FourCC typeStatic
        {
            get { return Type; }
        }


        public  void SetMotorSpeeds(float largeMotor, float smallMotor)
        {
            flags |= (byte)Flags.Rumble;
            largeMotorSpeed = (byte)Mathf.Clamp(largeMotor * 255, 0, 255);
            smallMotorSpeed = (byte)Mathf.Clamp(smallMotor * 255, 0, 255);
        }

        public void SetColor(Color color)
        {
            flags |= (byte)Flags.Color;
            redColor = (byte)Mathf.Clamp(color.r * 255, 0, 255);
            greenColor = (byte)Mathf.Clamp(color.g * 255, 0, 255);
            blueColor = (byte)Mathf.Clamp(color.b * 255, 0, 255);
        }

        public void ResetColor()
        {
            flags |= (byte)Flags.ResetColor;
        }


        public void ResetOrientation()
        {
            flags |= (byte)Flags.ResetOrientation;
        }

#if UNITY_PS5_ENHANCED_INPUT_SYSTEM
        public void SetVibrationMode(VibrationMode mode)
        {
            flags |= (byte)Flags.VibrationMode;
            vibrationMode = mode;
        }

        public unsafe void SetMotionSensorState(bool enabled)
        {
            flags |= (byte)Flags.MotionSensorState;
            motionSensor = *((byte*)(&enabled));
        }

        internal unsafe void SetTiltCorrectionState(bool enabled)
        {
            flags |= (byte)Flags.TiltCorrectionState;
            tiltCorrection = *((byte*)(&enabled));
        }
#endif

        internal static PS5GamepadOuputCommand Create()
        {
            return new PS5GamepadOuputCommand
            {
                baseCommand = new InputDeviceCommand(Type, kSize)
            };
        }
    }

#if UNITY_SUPPORT_ANGVEL_DEADBAND_STATE
    [StructLayout(LayoutKind.Explicit, Size = kSize)]
    internal struct PS5GamepadSetAngularVelocityDeadbandStateReport : IInputDeviceCommandInfo
    {
        internal static FourCC Type { get { return new FourCC('P', 'S', 'D', 'B'); } }
        internal const int kSize = InputDeviceCommand.BaseCommandSize + 1;

        public FourCC typeStatic => Type;

        [FieldOffset(0)] public InputDeviceCommand baseCommand;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 0)] byte enabled;

        public unsafe void SetEnabled(bool enabled)
        {
            this.enabled = *((byte*)(&enabled));
        }

        internal static PS5GamepadSetAngularVelocityDeadbandStateReport Create()
        {
            return new PS5GamepadSetAngularVelocityDeadbandStateReport
            {
                baseCommand = new InputDeviceCommand(Type, kSize)
            };
        }
    }
#endif

     /// <summary>
    /// The layout of a touch from the touchpad.
    /// </summary>
    // IMPORTANT: State layout must match with GamepadInputTouchStatePS5 in native.
    [StructLayout(LayoutKind.Explicit, Size = 12)]
    public struct PS5Touch : IInputStateTypeInfo
    {
        public static FourCC kFormat => new FourCC('P', '4', 'T', 'C');

        public FourCC format
        {
            get { return kFormat; }
        }

        [FieldOffset(0)] public int touchId;
        [FieldOffset(4)] public Vector2 position;
    }

    /// <summary>
    /// Returns information about the deadzone of the left and right thumbsticks.
    /// </summary>
    public struct StickInfo
    {
        public float leftStickDeadzone, rightStickDeadzone;
    }

#if UNITY_PS5_ENHANCED_INPUT_SYSTEM
    /// <summary>
    /// Command to request information about the controller
    /// </summary>
    // IMPORTANT: MUST match the layout of ControllerInformation in Native
    [StructLayout(LayoutKind.Explicit, Size = kSize)]
    internal struct QueryControllerInfo : IInputDeviceCommandInfo
    {
        private static FourCC Type => new('P', 'S', 'C', 'I');
        const int kSize = InputDeviceCommand.BaseCommandSize + 24;
        public FourCC typeStatic => Type;

        [FieldOffset(0)] public InputDeviceCommand baseCommand;
        [FieldOffset(InputDeviceCommand.BaseCommandSize)] public TouchPadInformation touchPadInformation;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 8)] public StickInfo stickInfo;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 16)] public DeviceClass deviceClass;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 20)] public byte connectionType;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 21)] public byte connectedCount;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 22)] public byte padding0, padding1;

        internal static QueryControllerInfo Create()
        {
            return new QueryControllerInfo
            {
                baseCommand = new InputDeviceCommand(Type, kSize)
            };
        }
    }
#endif
}

namespace UnityEngine.InputSystem.PS5
{
    //Sync to PS5InputDeviceDefinition in sixaxis.cpp
    [Serializable]
    class PS5InputDeviceDescriptor
    {
        public uint slotId;
        public bool isAimController;
        public uint defaultColorId;
        public uint userId;

        internal string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        internal static PS5InputDeviceDescriptor FromJson(string json)
        {
            return JsonUtility.FromJson<PS5InputDeviceDescriptor>(json);
        }
    }

    /// <summary>
    /// Control that contains information about a touch on the PS5 touchpad.
    /// </summary>
    [InputControlLayout(hideInUI = true)]
    [Scripting.Preserve]
    public class PS5TouchControl : InputControl<PS5Touch>
    {
        /// <summary>
        /// The ID of the touch contact as reported by the SDK
        /// </summary>
        [Scripting.Preserve]
        [InputControl(displayName = "ID", alias = "touchId", offset = 0)]
        public IntegerControl touchId { get; private set; }

        /// <summary>
        /// Position of the touch
        /// </summary>
        /// <remarks>In the range 0 to 1, normalized to the size of the touchpad</remarks>
        [Scripting.Preserve]
        [InputControl(displayName = "Position",usage = "Position", offset = 4)]
        public Vector2Control position { get; private set; }

        /// <summary>
        /// Change in position since the last touchpad update
        /// </summary>
        /// <remarks>In the range 0 to 1, normalized to the size of the touchpad</remarks>
        public Vector2 delta
        {
            get
            {
                var prevFramePosition = ReadValueFromPreviousFrame().position;
                if(prevFramePosition == k_InvalidPositionVector || position.value == k_InvalidPositionVector)
                    return Vector2.zero;
                Vector2 deltaComp = position.value - prevFramePosition;
                return deltaComp;
            }
        }

        /// <summary>
        /// Phase of the touch
        /// </summary>
        public TouchPhase phase => DetermineTouchPhaseFromPrevFrame();

        private static readonly Vector2 k_InvalidPositionVector = new Vector2(-1f, -1f);

        public PS5TouchControl()
        {
            m_StateBlock.format = new FourCC('P', '4', 'T', 'C');
        }

        protected override void FinishSetup()
        {
            touchId = GetChildControl<IntegerControl>("touchId");
            position = GetChildControl<Vector2Control>("position");
            base.FinishSetup();
        }

        private TouchPhase DetermineTouchPhaseFromPrevFrame()
        {
            Vector2 prevFramePosition;

            unsafe
            {
                if (previousFrameStatePtr == null)
                {
                    return TouchPhase.None;
                }
                var preFrameVal = ReadValueFromPreviousFrame();
                prevFramePosition = preFrameVal.position;
            }

            //Did the touch start this frame?
            if (prevFramePosition == k_InvalidPositionVector && position.value != k_InvalidPositionVector)
            {
                return TouchPhase.Began;
            }

            //Did the touch end this frame
            if (prevFramePosition != k_InvalidPositionVector && position.value == k_InvalidPositionVector)
            {
                //There is no way to determine ended vs canceled, always assume that touch was ended gracefully
                return TouchPhase.Ended;
            }

            if (position.value == k_InvalidPositionVector)
            {
                return TouchPhase.None;
            }

            if ((prevFramePosition - position.value).sqrMagnitude < Vector2.kEpsilon * Vector2.kEpsilon)
            {
                return TouchPhase.Stationary;
            }

            return TouchPhase.Moved;
        }

        public override string ToString()
        {
            return $"{phase}, {position.value}, {delta}";
        }

        ////FIXME: this suffers from the same problems that TouchControl has in that state layout is hardcoded
        public override unsafe PS5Touch ReadUnprocessedValueFromState(void* statePtr)
        {
            var valuePtr = (PS5Touch*)((byte*)statePtr + (int)m_StateBlock.byteOffset);
            return *valuePtr;
        }

        public override unsafe void WriteValueIntoState(PS5Touch value, void* statePtr)
        {
            var valuePtr = (PS5Touch*)(byte*)statePtr + (int)m_StateBlock.byteOffset;
            UnsafeUtility.MemCpy(valuePtr, UnsafeUtility.AddressOf(ref value), UnsafeUtility.SizeOf<PS5Touch>());
        }
    }

    /// <summary>
    /// Gamepad that represents a DualSense Controller running on a PS5
    /// </summary>
    [InputControlLayout(stateType = typeof(GamepadStatePS5), displayName = "PS5 DualSense (on PS5)")]
    [Scripting.Preserve]
    public class DualSenseGamepad : DualShockGamepad
    {
        /// <summary>
        /// The last used/added DualSense controller.
        /// </summary>
        public new static DualSenseGamepad current { get; private set; }

        /// <summary>
        /// Acceleration of the Controller
        /// </summary>
        /// <remarks>Data from this control is converted from Right to Left-handed coordinates space to match Unity's coordinates space.
        /// To read the raw (uncorrected) value from the SDK use ReadValueUnprocessed()</remarks>
        public Vector3Control acceleration { get; private set; }

        /// <summary>
        /// Orientation of the Controller
        /// </summary>
        /// <remarks>Data from this control is converted from Right to Left-handed coordinates space to match Unity's coordinates space.
        /// To read the raw (uncorrected) value from the SDK use ReadValueUnprocessed()</remarks>
        public QuaternionControl orientation { get; private set; }

        /// <summary>
        /// Angular Velocity of the Controller with the axis in radians per second
        /// </summary>
        /// <remarks>Data from this control is converted from Right to Left-handed coordinates space to match Unity's coordinates space
        /// and inverted so that positive rotation matches the positive direction of the rotation axis
        /// To read the raw (uncorrected) value from the SDK use ReadValueUnprocessed()</remarks>
        public Vector3Control angularVelocity { get; private set; }

        /// <summary>
        /// Array of all of the touches on the touchpad
        /// </summary>
        public ReadOnlyArray<PS5TouchControl> touches { get; private set; }

        /// <summary>
        /// All connected DualSense devices
        /// </summary>
        public new static ReadOnlyArray<DualSenseGamepad> all => new ReadOnlyArray<DualSenseGamepad>(s_Devices);

        public static ReadOnlyArray<DualSenseGamepad> allAimDevices => new ReadOnlyArray<DualSenseGamepad>(s_AimDevices);

        /// <summary>
        /// Slot that this controller is assigned to
        /// </summary>
        public int slotIndex => m_SlotId;

        /// <summary>
        /// The user ID currently assigned to this controller
        /// </summary>
        public int ps5UserId => m_PS5UserId;

        /// <summary>
        /// If this controller is an aim controller
        /// </summary>
        public bool isAimController => m_IsAimController;

        /// <summary>
        /// The current color of the lightbar
        /// </summary>
        /// <remarks>If the lightbar colour has not been set during the running of the application this property will return the
        /// default colour of the controller</remarks>
        public Color lightBarColor => m_LightBarColor.HasValue == false ? PS4ColorIdToColor(m_DefaultColorId) : m_LightBarColor.Value;

#if UNITY_PS5_ENHANCED_INPUT_SYSTEM
        /// <summary>
        /// Information about the touchpad (resolution and pixel density)
        /// </summary>
        public TouchPadInformation touchPadInformation
        {
            get
            {
                RefreshControllerInformation();
                return m_TouchPadInformation;
            }
        }

        /// <summary>
        /// Information about the deadzones set for each analog stick
        /// </summary>
        public StickInfo stickInfo
        {
            get
            {
                RefreshControllerInformation();
                return m_StickInformation;
            }
        }

        /// <summary>
        /// The type of device that this gamepad is
        /// </summary>
        public DeviceClass deviceClass
        {
            get
            {
                RefreshControllerInformation();
                return m_DeviceClass;
            }
        }

        /// <summary>
        /// How this controller is connected to the system (local or remote play)
        /// </summary>
        public ConnectionType connectionType
        {
            get
            {
                RefreshControllerInformation();
                return m_ConnectionType;
            }
        }
#endif

        /// <summary>
        /// Get a controller at a given slot
        /// </summary>
        /// <param name="slotIndex">Slot to get the controller</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Thrown if a slot outside of the number of devices is passed</exception>
        public static DualSenseGamepad GetBySlotIndex(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= s_Devices.Length)
                throw new ArgumentException("Slot index out of range: " + slotIndex, "slotIndex");

            if (s_Devices[slotIndex] != null && s_Devices[slotIndex].slotIndex == slotIndex)
            {
                return s_Devices[slotIndex];
            }

            return null;
        }

        /// <inheritdoc />
        protected override void OnAdded()
        {
            base.OnAdded();
            AddDeviceToList();
        }

        /// <inheritdoc />
        public override void MakeCurrent()
        {
            base.MakeCurrent();
            current = this;
        }

        /// <inheritdoc />
        protected override void OnRemoved()
        {
            base.OnRemoved();
            if (current == this)
                current = null;
            RemoveDeviceFromList();
        }

        private void AddDeviceToList()
        {
            DualSenseGamepad[] deviceList;

            if (m_IsAimController == true)
            {
                deviceList = s_AimDevices;
            }
            else
            {
                deviceList = s_Devices;
            }

            var index = slotIndex;
            if (index >= 0 && index < deviceList.Length)
            {
                Debug.Assert(deviceList[index] == null, "PS5 gamepad with same slotIndex already added");
                deviceList[index] = this;
            }
        }

        private void RemoveDeviceFromList()
        {
            if (m_SlotId == -1)
                return;

            DualSenseGamepad[] deviceList;

            if (m_IsAimController == true)
            {
                deviceList = s_AimDevices;
            }
            else
            {
                deviceList = s_Devices;
            }

            var index = slotIndex;
            if (index >= 0 && index < deviceList.Length && deviceList[index] == this)
                deviceList[index] = null;
        }

        /// <inheritdoc />
        protected override void FinishSetup()
        {
            base.FinishSetup();

            acceleration = GetChildControl<Vector3Control>("acceleration");
            orientation = GetChildControl<QuaternionControl>("orientation");
            angularVelocity = GetChildControl<Vector3Control>("angularVelocity");

            var touchArray = new PS5TouchControl[2];

            touchArray[0] = GetChildControl<PS5TouchControl>("touch0");
            touchArray[1] = GetChildControl<PS5TouchControl>("touch1");

            touches = new ReadOnlyArray<PS5TouchControl>(touchArray);

            var capabilities = description.capabilities;
            var deviceDescriptor = PS5InputDeviceDescriptor.FromJson(capabilities);

            if (deviceDescriptor != null)
            {
                m_SlotId = (int)deviceDescriptor.slotId;
                m_DefaultColorId = (int)deviceDescriptor.defaultColorId;
                m_PS5UserId = (int)deviceDescriptor.userId;

                m_IsAimController = deviceDescriptor.isAimController;

                if (!m_LightBarColor.HasValue)
                {
                    m_LightBarColor = PS4ColorIdToColor(m_DefaultColorId);
                }
            }
        }

        /// <inheritdoc/>
        public override void PauseHaptics()
        {
            if (!m_LargeMotor.HasValue && !m_SmallMotor.HasValue && !m_LightBarColor.HasValue)
                return;

            var command = PS5GamepadOuputCommand.Create();
            command.SetMotorSpeeds(0f, 0f);
            if (m_LightBarColor.HasValue)
                command.SetColor(Color.black);

            var ret = ExecuteCommand(ref command);
            if (ret < 0)
            {
                Debug.LogError($"Failed to set PauseHaptics 0x{(int)ret:x8}");
            }
        }

        /// <inheritdoc/>
        public override void ResetHaptics()
        {
            if (!m_LargeMotor.HasValue && !m_SmallMotor.HasValue && !m_LightBarColor.HasValue)
                return;

            var command = PS5GamepadOuputCommand.Create();
            command.SetMotorSpeeds(0f, 0f);

            if (m_LightBarColor.HasValue)
                command.ResetColor();

            var ret = ExecuteCommand(ref command);
            if (ret < 0)
            {
                Debug.LogError($"Failed to set ResetHaptics 0x{(int)ret:x8}");
                return;
            }

            m_LargeMotor = null;
            m_SmallMotor = null;
            m_LightBarColor = null;
        }

        /// <inheritdoc/>
        public override void ResumeHaptics()
        {
            if (!m_LargeMotor.HasValue && !m_SmallMotor.HasValue && !m_LightBarColor.HasValue)
                return;

            var command = PS5GamepadOuputCommand.Create();

            if (m_LargeMotor.HasValue || m_SmallMotor.HasValue)
                command.SetMotorSpeeds(m_LargeMotor.Value, m_SmallMotor.Value);
            if (m_LightBarColor.HasValue)
                command.SetColor(m_LightBarColor.Value);

            var ret = ExecuteCommand(ref command);

            if (ret < 0)
            {
                Debug.LogError($"Failed to set ResumeHaptics 0x{(int)ret:x8}");
            }
        }

        /// <inheritdoc/>
        public override void SetLightBarColor(Color color)
        {
            var command = PS5GamepadOuputCommand.Create();
            command.SetColor(color);

            var ret = ExecuteCommand(ref command);

            if (ret < 0)
            {
                Debug.LogError($"Failed to set SetLightBarColor 0x{(int)ret:x8}");
                return;
            }

            m_LightBarColor = color;
        }

       /// <summary>
       /// Reset the lightbar color
       /// </summary>
        public void ResetLightBarColor()
        {
            var command = PS5GamepadOuputCommand.Create();
            command.ResetColor();

            var ret = ExecuteCommand(ref command);

            if (ret < 0)
            {
                Debug.LogError($"Failed to set ResetLightBarColor 0x{(int)ret:x8}");
                return;
            }

            m_LightBarColor = null;
        }

        /// <inheritdoc/>
        /// <remarks>This method will only set vibration when the <see cref="VibrationMode"/> is set to either
        /// <see cref="VibrationMode.Compatible"/> or <see cref="VibrationMode.Compatible2"/></remarks>
        public override void SetMotorSpeeds(float lowFrequency, float highFrequency)
        {
            var command = PS5GamepadOuputCommand.Create();
            command.SetMotorSpeeds(lowFrequency, highFrequency);

            var ret = ExecuteCommand(ref command);

            if (ret < 0)
            {
                Debug.LogError($"Failed to set SetMotorSpeeds 0x{(int)ret:x8}");
                return;
            }

            m_LargeMotor = lowFrequency;
            m_SmallMotor = highFrequency;
        }

        /// <summary>
        /// Reset the orientation sensors of the Gamepad
        /// </summary>
        public void ResetOrientation()
        {
            var command = PS5GamepadOuputCommand.Create();
            command.ResetOrientation();

            var ret = ExecuteCommand(ref command);

            if (ret < 0)
            {
                Debug.LogError($"Failed to set ResetOrientation 0x{(int)ret:X}");
            }
        }

#if UNITY_PS5_ENHANCED_INPUT_SYSTEM

        /// <summary>
        /// Set the vibration mode from the controller.
        /// </summary>
        /// <param name="mode">Mode for vibration</param>
        public void SetVibrationMode(VibrationMode mode)
        {
            var command = PS5GamepadOuputCommand.Create();
            command.SetVibrationMode(mode);

            var ret = ExecuteCommand(ref command);

            if (ret < 0)
            {
                Debug.LogError($"Failed to set SetVibrationMode 0x{(int)ret:x8}");
            }
        }

        /// <summary>
        /// Set if the tilt correction sensor is enabled or disabled
        /// </summary>
        /// <param name="enabled"></param>
        public void SetTiltCorrectionState(bool enabled)
        {
            var command = PS5GamepadOuputCommand.Create();
            command.SetTiltCorrectionState(enabled);

            long ret = ExecuteCommand(ref command);

            if (ret < 0)
            {
                Debug.LogError($"Failed to set SetTiltCorrectionState 0x{(int)ret:x8}");
            }
        }

        /// <summary>
        /// Enable or disable the motion sensor.
        /// </summary>
        /// <param name="enabled"></param>
        public void SetMotionSensorState(bool enabled)
        {
            var command = PS5GamepadOuputCommand.Create();
            command.SetMotionSensorState(enabled);

            long ret = ExecuteCommand(ref command);

            if (ret < 0)
            {
                Debug.LogError($"Failed to set SetMotionSensorState 0x{(int)ret:x8}");
            }
        }

        private void RefreshControllerInformation()
        {
            var command = QueryControllerInfo.Create();
            var ret = ExecuteCommand(ref command);
            if (ret < 0)
            {
                Debug.LogError($"Failed to RefreshControllerInformation 0x{(int)ret:x8}");
                return;
            }

            m_TouchPadInformation = command.touchPadInformation;
            m_StickInformation = command.stickInfo;
            m_DeviceClass = command.deviceClass;
            m_ConnectionType = (ConnectionType)command.connectionType;
        }
#endif

#if UNITY_SUPPORT_ANGVEL_DEADBAND_STATE
        /// <summary>
        /// Set the deadband filter of the angular velocity sensor
        /// </summary>
        /// <param name="enabled"></param>
        public unsafe void SetAngularVelocityDeadbandState(bool enabled)
        {
            var command = PS5GamepadSetAngularVelocityDeadbandStateReport.Create();
            command.SetEnabled(enabled);

            long ret = ExecuteCommand(ref command);

            if (ret < 0)
            {
                Debug.LogError($"Failed to set SetAngularVelocityDeadbandState 0x{(int)ret:x8}");
            }
        }
#endif

        /// <summary>
        /// Set the trigger effect of the haptic triggers
        /// </summary>
        /// <param name="data">Trigger effect data to set</param>
        public void SetTriggerEffect(TriggerEffectParam data)
        {
            ExecuteCommand(ref data);
        }

        private float? m_LargeMotor;
        private float? m_SmallMotor;
        private Color? m_LightBarColor;

        // Slot id for the gamepad. Once set will never change.
        private int m_SlotId = -1;
        private int m_DefaultColorId = -1;
        private int m_PS5UserId = -1;
        private bool m_IsAimController;

#if UNITY_PS5_ENHANCED_INPUT_SYSTEM
        private TouchPadInformation m_TouchPadInformation;
        private StickInfo m_StickInformation;
        private DeviceClass m_DeviceClass;
        private ConnectionType m_ConnectionType;
#endif

        private static DualSenseGamepad[] s_Devices = new DualSenseGamepad[4];
        private static DualSenseGamepad[] s_AimDevices = new DualSenseGamepad[4];


        private static Color PS4ColorIdToColor(int colorId)
        {
            switch (colorId)
            {
                case 0:
                    return Color.blue;
                case 1:
                    return Color.red;
                case 2:
                    return Color.green;
                case 3:
                    return Color.magenta;
                default:
                    return Color.black;
            }
        }
    }
}
#endif // UNITY_EDITOR || UNITY_PS5
