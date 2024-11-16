using System;
using UnityEngine;

namespace SettingsManagement
{

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class MultiPlatformAttribute : Attribute
    {
        public MultiPlatformAttribute()
        {
        }

        private string[] includePlatforms;

        public string IncludePlatform
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                    includePlatforms = null;
                else
                    includePlatforms = new string[] { value };
            }

            get => null;
        }

        public string[] IncludePlatforms { get => includePlatforms; set => includePlatforms = value; }

        private string[] excludePlatforms;

        public string ExcludePlatform
        {
            set
            {
                if (string.IsNullOrEmpty(value))
                    excludePlatforms = null;
                else
                    excludePlatforms = new string[] { value };
            }
            get => null;
        }

        public string[] ExcludePlatforms { get => excludePlatforms; set => excludePlatforms = value; }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InputFieldAttribue : Attribute
    {
        public bool IsElement { get; set; }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class PathFieldAttribute : InputFieldAttribue
    {
        public bool IsFolder { get; set; }

    }


    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class CombineAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = true)]
    public class SettingsAttribute : Attribute
    {
        public SettingsAttribute() { }
        public SettingsAttribute(Type settingsType)
        {
            SettingsType = settingsType;
        }

        public Type SettingsType { get; set; }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class NullableValueAttribute : PropertyAttribute
    {

    }
}
