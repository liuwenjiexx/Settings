using SettingsManagement.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace SettingsManagement.Editor
{
    [UnityEditor.CustomPropertyDrawer(typeof(AssetGuid))]
    class AssetGuidPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            ObjectField objectField = new ObjectField();
            objectField.label = property.displayName;

            objectField.RegisterValueChangedCallback(e =>
            {
                AssetGuid assetPath = new AssetGuid(objectField.value);
                SerializedPropertyUtility.SetObjectOfProperty(property, assetPath);
                property.serializedObject.Update();
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            });

            AssetGuid value = (AssetGuid)SerializedPropertyUtility.GetObjectOfProperty(property);
            objectField.SetValueWithoutNotify(value.Target);
            return objectField;
        }
    }
}
