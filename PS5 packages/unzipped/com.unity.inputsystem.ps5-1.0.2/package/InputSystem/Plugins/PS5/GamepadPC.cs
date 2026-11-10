using System.Runtime.InteropServices;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

#if UNITY_EDITOR
using UnityEngine.InputSystem.DualShock.LowLevel;
#endif

namespace UnityEngine.InputSystem.PS5
{
#if UNITY_EDITOR
    /// <summary>
    /// PS5 DualSense controller that is interfaced to a HID backend.
    /// This is used when running games in the editor only with a controller plugged into the PC
    /// </summary>
    /// <remarks>Extends functionality from <see cref="DualSenseGamepadHID"/> with extra editor only features for PS5
    /// development</remarks>
    [InputControlLayout(stateType = typeof(DualSenseHIDInputReport), displayName = "PS5 DualSense (in Editor)")]
    [Scripting.Preserve]
    public class DualSenseGamepadPC : DualSenseGamepadHID
    {
        public new static ReadOnlyArray<DualSenseGamepadPC> all => new ReadOnlyArray<DualSenseGamepadPC>(s_Devices);

        public int slotIndex => m_SlotId;
        public int ps5UserId => m_PS5UserId;

        /// <summary>
        /// The last used/added DualSense controller.
        /// </summary>
        public new static DualSenseGamepadPC current { get; private set; }

        /// <summary>
        /// Color of the lightbar
        /// </summary>
        /// <remarks>Will return the default lightbar color for the user if it has not been set</remarks>
        public Color lightBarColor
        {
            get
            {
                m_LightBarColor ??= PS5ColorIdToColor(m_DefaultColorId);
                return m_LightBarColor.Value;
            }
        }

        /// <summary>
        /// Get the controller at the given slot index.
        /// This is simulated to mirror the concept of slots on PS5 and does not correspond to a physical slot on the device
        /// </summary>
        /// <param name="slotIndex">Index of the controller</param>
        /// <returns>DualSenseGamepadPC at the given slot in the controller array</returns>
        /// <exception cref="System.ArgumentException">Thrown if slot index is less than zero or if the <see cref="slotIndex"/>> is
        /// greater than the number of controllers connected</exception>
        public static DualSenseGamepadPC GetBySlotIndex(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= s_Devices.Length)
                throw new System.ArgumentException("Slot index out of range: " + slotIndex, "slotIndex");

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
        protected override void OnRemoved()
        {
            base.OnRemoved();
            if (current == this)
                current = null;
            RemoveDeviceFromList();
        }

        /// <inheritdoc />
        public override void MakeCurrent()
        {
            base.MakeCurrent();
            current = this;
        }

        private void AddDeviceToList()
        {
            var index = slotIndex;
            if (index >= 0 && index < s_Devices.Length)
            {
                if (s_Devices[index] != null)
                {
                    m_SlotId = GenerateSlotId();
                    s_Devices[m_SlotId] = this;

                }
                else
                {
                    s_Devices[index] = this;
                }
            }
        }

        private void RemoveDeviceFromList()
        {
            if (m_SlotId == -1)
                return;

            var index = slotIndex;
            if (index >= 0 && index < s_Devices.Length && s_Devices[index] == this)
                s_Devices[index] = null;
        }


        protected override void FinishSetup()
        {
            base.FinishSetup();

            var capabilities = description.capabilities;

            var deviceDescriptor = PS5InputDeviceDescriptor.FromJson(capabilities);

            if (deviceDescriptor != null)
            {
                m_SlotId = GenerateSlotId();
                m_DefaultColorId = (int)deviceDescriptor.defaultColorId;
                m_PS5UserId = (int)deviceDescriptor.userId;

                SetLightBarColor(PS5ColorIdToColor(m_DefaultColorId));
            }
        }

        // DualSenseGamepadPC is used in editor mode. We don't get a slotId from logged in users like we do
        // in the PS5Player so we need to generate a slotId
        private int GenerateSlotId()
        {
            for (int i = 0; i < s_Devices.Length; i++)
            {
                if (s_Devices[i] == null)
                {
                    return i;
                }
            }
            Debug.LogWarning("Too many gamepads connected, gamepad at slotIndex 0 will be overwritten.");
            return 0;
        }

        /// <summary>
        /// Set the trigger effect of the haptic triggers
        /// </summary>
        /// <param name="effect">Trigger effect data to set</param>
        public void SetTriggerEffect(TriggerEffectParam effect)
        {
             var command = DualSenseHIDOutputReport.Create();
             command.SetTriggerEffect(effect);
             ExecuteCommand(ref command);
        }

        // Slot id for the gamepad. Once set will never change.
        private int m_SlotId = -1;
        private int m_DefaultColorId = -1;
        private int m_PS5UserId = -1;

        private static DualSenseGamepadPC[] s_Devices = new DualSenseGamepadPC[4];

        private static Color PS5ColorIdToColor(int colorId)
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

    [StructLayout(LayoutKind.Explicit, Size = kSize)]
    internal unsafe struct DualSenseHIDOutputReport : IInputDeviceCommandInfo
    {
        public static FourCC Type => new FourCC('H', 'I', 'D', 'O');

         internal const int kSize = InputDeviceCommand.BaseCommandSize + 48;
        internal const int kReportId = 2;   //5;

 //     [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "No better term for underlying data.")]
 //     [Flags]
        public enum Flags
        {
            Rumble = 0x1,
            VibrationModeCompatible = 0x2
        }

        [FieldOffset(0)] public InputDeviceCommand baseCommand;

        [FieldOffset(InputDeviceCommand.BaseCommandSize + 0)] public byte reportId;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "flags", Justification = "No better term for underlying data.")]
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 1)] public byte flags;    // 0x04=triggerRight 0x08=triggerLeft   2=SCE_PAD_VIBRATION_MODE_COMPATIBLE
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 2)] public byte flags2;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 0x2d)] public byte redColor;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 0x2e)] public byte greenColor;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 0x2f)] public byte blueColor;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 9)] public fixed byte unknown2[2];

        [FieldOffset(InputDeviceCommand.BaseCommandSize + 0)] public fixed byte rawdata[48];
        const int kTriggerEffectSize = 11;


        const byte kVibration = 0x26;
        const byte kWeapon = 0x25;
        const byte kFeedback = 0x21;

        const byte kSlopeFeedback = 0x22;


        const byte kNone = 0x05;

        public FourCC typeStatic => Type;

        private void ProcessParameter(int index, byte value, ref int val, ref int val2)
        {
            if (value!=0)
            {
                val = val|(1<<index);
                val2 = val2|((value-1)<<(index*3));
            }
        }

        private void SetTriggerEffect(TriggerEffectCommand effect, int index)
        {
            for (int i=0;i<kTriggerEffectSize;i++)
                rawdata[index+i]=0;

            switch(effect.mode)
            {
                case TriggerEffectMode.Off:
                    rawdata[index]=kNone;
                    break;
                case TriggerEffectMode.Feedback:
                {
                    int val =(0x400)-(1<<effect.feedback.position);
                    long val2 = (((long)effect.feedback.strength-1)*(long)0x3fffffff)/7;

                    rawdata[index]=kFeedback;
                    rawdata[index+1]=(byte)(val&0xff);
                    rawdata[index+2]=(byte)((val&0xff00)>>8);
                    rawdata[index+3]=(byte)(val2&0xff);
                    rawdata[index+4]=(byte)((val2&0xff00)>>8);
                    rawdata[index+5]=(byte)((val2&0xff0000)>>16);
                    rawdata[index+6]=(byte)((val2&0xff000000)>>24);
                }
                    break;
                case TriggerEffectMode.Weapon:
                {
                    int val = (1<<effect.weapon.startPosition) | (1<<effect.weapon.endPosition);
                    rawdata[index]=kWeapon;
                    rawdata[index+1]=(byte)(val&0xff);
                    rawdata[index+2]=(byte)((val&0xff00)>>8);
                    rawdata[index+3]=(byte)(effect.weapon.strength-1);
                }
                    break;
                case TriggerEffectMode.Vibration:
                {
                    int val1 = 0x400-(1<<effect.vibration.position);
                    int val2 = 0x1000000  - (1<<(effect.vibration.position*3));
                    rawdata[index]=kVibration;
                    rawdata[index+1]=(byte)(val1&0xff);
                    rawdata[index+2]=(byte)((val1&0xff00)>>8);
                    rawdata[index+3]=(byte)(val2&0xff);
                    rawdata[index+4]=(byte)((val2&0xff00)>>8);
                    rawdata[index+5]=(byte)((val2&0xff0000)>>16);
                    rawdata[index+6]=(byte)((effect.vibration.amplitude-1)<<3) ;
                    rawdata[index+7]=0;
                    rawdata[index+8]=0;
                    rawdata[index+9]=(byte)effect.vibration.frequency;
                    rawdata[index+10]=0;
                }
                    break;
                 case TriggerEffectMode.SlopeFeedback:
                {
                    int val =(1<<effect.slopeFeedback.startPosition)|(1<<effect.slopeFeedback.endPosition);
                    int val2 = ((effect.slopeFeedback.endStrength-1)*8)| (effect.slopeFeedback.startStrength-1);

                    rawdata[index]=kSlopeFeedback;
                    rawdata[index+1]=(byte)(val&0xff);
                    rawdata[index+2]=(byte)((val&0xff00)>>8);
                    rawdata[index+3]=(byte)(val2&0xff);

                }
                    break;
                case TriggerEffectMode.MultiplePositionFeedback:
                {
                    int val =0x0;
                    int val2 =0x0;

                    ProcessParameter(0,effect.multiplePositionFeedback.strength0, ref val, ref val2);
                    ProcessParameter(1,effect.multiplePositionFeedback.strength1, ref val, ref val2);
                    ProcessParameter(2,effect.multiplePositionFeedback.strength2, ref val, ref val2);
                    ProcessParameter(3,effect.multiplePositionFeedback.strength3, ref val, ref val2);
                    ProcessParameter(4,effect.multiplePositionFeedback.strength4, ref val, ref val2);
                    ProcessParameter(5,effect.multiplePositionFeedback.strength5, ref val, ref val2);
                    ProcessParameter(6,effect.multiplePositionFeedback.strength6, ref val, ref val2);
                    ProcessParameter(7,effect.multiplePositionFeedback.strength7, ref val, ref val2);
                    ProcessParameter(8,effect.multiplePositionFeedback.strength8, ref val, ref val2);
                    ProcessParameter(9,effect.multiplePositionFeedback.strength9, ref val, ref val2);

                    rawdata[index]=kFeedback;
                    rawdata[index+1]=(byte)(val&0xff);
                    rawdata[index+2]=(byte)((val&0xff00)>>8);
                    rawdata[index+3]=(byte)(val2&0xff);
                    rawdata[index+4]=(byte)((val2&0xff00)>>8);
                    rawdata[index+5]=(byte)((val2&0xff0000)>>16);
                    rawdata[index+6]=(byte)((val2&0xff000000)>>24);
                }
                break;

                case TriggerEffectMode.MultiplePositionVibration:
                {
                    int val =0x0;
                    int val2 = 0x0;

                    ProcessParameter(0,effect.multiplePositionVibration.amplitude0, ref val, ref val2);
                    ProcessParameter(1,effect.multiplePositionVibration.amplitude1, ref val, ref val2);
                    ProcessParameter(2,effect.multiplePositionVibration.amplitude2, ref val, ref val2);
                    ProcessParameter(3,effect.multiplePositionVibration.amplitude3, ref val, ref val2);
                    ProcessParameter(4,effect.multiplePositionVibration.amplitude4, ref val, ref val2);
                    ProcessParameter(5,effect.multiplePositionVibration.amplitude5, ref val, ref val2);
                    ProcessParameter(6,effect.multiplePositionVibration.amplitude6, ref val, ref val2);
                    ProcessParameter(7,effect.multiplePositionVibration.amplitude7, ref val, ref val2);
                    ProcessParameter(8,effect.multiplePositionVibration.amplitude8, ref val, ref val2);
                    ProcessParameter(9,effect.multiplePositionVibration.amplitude9, ref val, ref val2);

                    rawdata[index]=kVibration;
                    rawdata[index+1]=(byte)(val&0xff);
                    rawdata[index+2]=(byte)((val&0xff00)>>8);
                    rawdata[index+3]=(byte)(val2&0xff);
                    rawdata[index+4]=(byte)((val2&0xff00)>>8);
                    rawdata[index+5]=(byte)((val2&0xff0000)>>16);
                    rawdata[index+6]=(byte)((val2&0xff000000)>>24);
                    rawdata[index+9]=(byte)(effect.multiplePositionVibration.frequency&0xff);

                }
                break;
            }
        }

        /// <summary>
        /// Set the trigger effect on the gamepad
        /// </summary>
        /// <param name="data">Effect parameter to set</param>
        public void SetTriggerEffect(TriggerEffectParam data)
        {
            const int kHidLeftTriggerDataOffset = 22;
            const int kHidRightTriggerDataOffset = 11;
            if ((data.triggerMask & TriggerEffectMask.L2) != 0)
            {
                flags|=8;
                SetTriggerEffect(data.left, kHidLeftTriggerDataOffset);
            }
            if ((data.triggerMask & TriggerEffectMask.R2) != 0)
            {
                flags|=4;
                SetTriggerEffect(data.right, kHidRightTriggerDataOffset);
            }
        }


        public static DualSenseHIDOutputReport Create()
        {
            return new DualSenseHIDOutputReport
            {
                baseCommand = new InputDeviceCommand(Type, kSize),
                reportId = kReportId,
            };
        }
    }
#endif
}
