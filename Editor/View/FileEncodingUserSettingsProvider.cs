using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.SettingsManagement;
using Unity.SettingsManagement.Editor;

namespace Unity.Text.Editor
{
    class FileEncodingUserSettingsProvider : SettingsProvider
    {
        const string SettingsPath = "Unity/File Encoding";

        public FileEncodingUserSettingsProvider()
          : base(SettingsPath, UnityEditor.SettingsScope.User)
        {
        }


        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            var provider = new FileEncodingUserSettingsProvider();
            provider.keywords = new string[] { "encoding" };
            return provider;
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            VisualElement windowContent;
            string docFile = Path.GetFullPath(Path.Combine(EditorSettingsUtility.GetUnityPackageDirectory(SettingsUtility.GetPackageName(GetType())), "README.md"));
            windowContent = EditorSettingsUtility.CreateSettingsWindow(rootElement, "File Encoding"/*, helpLink: docFile*/);

            rootElement.styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(EditorSettingsUtility.GetEditorUSSPath(SettingsUtility.GetPackageName(GetType()), nameof(FileEncodingUserSettingsProvider))));
            EditorSettingsUtility.CreateSettingView(windowContent, typeof(FileEncodingUserSettings));
        }

    }
}