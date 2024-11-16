using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SettingsManagement.Editor
{
    public class CustomInputViewAttribute : Attribute
    {

        public CustomInputViewAttribute(Type targetType)
        {
            TargetType = targetType;
        }

        public Type TargetType { get; set; }

      
    }
}
