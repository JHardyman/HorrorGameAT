using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.PlatformToolkit.Editor;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Unity.PlatformToolkit.PS5.Editor
{
    [Serializable]
    internal class PS5Settings : AttributeSettings, IPS5Settings, INotifyBindablePropertyChanged
    {
        public override string PlatformKey => PS5PlatformToolkit.Key;
        public event EventHandler<BindablePropertyChangedEventArgs> propertyChanged;

        // Source of size limits: https://game.develop.playstation.net/resources/documents/SDK/10.000/SaveData-Overview/0002.html#__document_toc_00000009
        internal const int k_MinSaveSizeInKib = 3072;
        internal const int k_MaxSaveSizeInKib = 1049000;

        [SerializeField]
        private int allocatedSaveSize = k_MinSaveSizeInKib;

        [CreateProperty]
        public int AllocatedSaveSize
        {
            get => allocatedSaveSize;
            set => SetProperty(ref allocatedSaveSize, Math.Clamp(value, k_MinSaveSizeInKib, k_MaxSaveSizeInKib));
        }

        public IAttributeSettings AttributeSettings => this;

        protected void SetProperty<T>(ref T field, T value, [CallerMemberName] string property = "")
        {
            if (value == null && field == null || value != null && value.Equals(field))
                return;

            field = value;
            propertyChanged?.Invoke(this, new BindablePropertyChangedEventArgs(property));
        }

        protected override void InitializeAttributes(out IReadOnlyList<(string AttributeId, Type AttributeType, string AttributeName)> attributeDefinitions)
        {
            attributeDefinitions = new List<(string attributeId, Type attributeType, string attributeName)>
            {
                ("OnlineId", typeof(string), "OnlineId"),
                ("UserId", typeof(int), "UserId"),
                ("FirstName", typeof(string), "First Name"),
                ("MiddleName", typeof(string), "Middle Name"),
                ("LastName", typeof(string), "Last Name"),
                ("SmallAvatar", typeof(Texture2D), "Small Avatar"),
                ("MediumAvatar", typeof(Texture2D), "Medium Avatar"),
                ("LargeAvatar", typeof(Texture2D), "Large Avatar"),
                ("ExtraLargeAvatar", typeof(Texture2D), "Extra Large Avatar")
            };
        }
    }
}
