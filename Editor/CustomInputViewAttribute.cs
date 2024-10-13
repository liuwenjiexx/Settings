using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Unity.SettingsManagement.Editor
{
    public class CustomInputViewAttribute : Attribute
    {
        private static Dictionary<Type, Type> typeMapViewTypes;

        public CustomInputViewAttribute(Type targetType)
        {
            TargetType = targetType;
        }

        public Type TargetType { get; set; }

        internal static Type GetInputViewType(Type valueType)
        {
            Type viewType = null;
            if (typeMapViewTypes == null)
            {
                typeMapViewTypes = new();
                foreach (var type in TypeCache.GetTypesWithAttribute(typeof(CustomInputViewAttribute)))
                {
                    if (!type.IsClass || type.IsAbstract)
                        continue;
                    if (!typeof(InputView).IsAssignableFrom(type))
                    {
                        Debug.LogError($"{nameof(CustomInputViewAttribute)} Type '{type.Name}' not interit '{typeof(InputView).Name}'");
                        continue;
                    }
                    var viewAttr = type.GetCustomAttribute<CustomInputViewAttribute>();
                    var targetType = viewAttr.TargetType;
                    if (targetType == null) continue;
                    typeMapViewTypes[targetType] = type;
                }
            }

            if (!typeMapViewTypes.TryGetValue(valueType, out viewType))
            {
                if (BaseInputView.IsBaseField(valueType))
                {
                    viewType = typeof(BaseInputView);
                    typeMapViewTypes[valueType] = viewType;
                }
                else if (valueType.IsEnum)
                {
                    if (typeMapViewTypes.TryGetValue(typeof(Enum), out viewType))
                    {
                        typeMapViewTypes[valueType] = viewType;
                    }
                }

            }
            return viewType;
        }
    }
}
