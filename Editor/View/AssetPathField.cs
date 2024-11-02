using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Unity;

namespace SettingsManagement.Editor
{

    [CustomInputView(typeof(AssetPath))]
    public class AssetPathField : InputView
    {
        ObjectField input;
        public override VisualElement CreateView()
        {
            input = new ObjectField();
            input.label = DisplayName;
            input.objectType = typeof(GameObject);
            input.RegisterValueChangedCallback(e =>
            {
                var prefab = e.newValue as GameObject;
                AssetPath newValue;
                newValue = new AssetPath(prefab);
                OnValueChanged(newValue);
            });

            return input;
        }

        public override void SetValue(object value)
        {
            GameObject prefab = null;
            if (value is AssetPath assetPath)
            {
                prefab = assetPath.Asset as GameObject;
            }
            input.SetValueWithoutNotify(prefab);
        }
    }

    [UnityEditor.CustomPropertyDrawer(typeof(AssetPath))]
    class AssetPathPropertyDrawer : PropertyDrawer
    {
        SerializedProperty targetProperty;

        //void OnEnable()
        //{
        //    targetProperty = serializedObject.FindProperty("target");
        //}

        //public override void OnInspectorGUI()
        //{
        //    serializedObject.Update();
        //    AssetPath assetPathValue = (AssetPath)target;
        //    //EditorGUILayout.ObjectField(assetPathValue.Target, typeof(UnityEngine.Object),);
        //    EditorGUILayout.PropertyField(targetProperty);
        //    serializedObject.ApplyModifiedProperties();
        //}

        //public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        //{

        //}

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            ObjectField objectField = new ObjectField();
            objectField.label = property.displayName;
            
            objectField.RegisterValueChangedCallback(e =>
            {
                Debug.Log("change: " + e.newValue);
                Debug.Log(e.newValue == null);
            });
            return objectField;
        }
    }

}