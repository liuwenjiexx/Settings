using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Unity.SettingsManagement;

[assembly: AssemblyMetadata("Package.Name", "com.unity.settings")]

[assembly: Settings(typeof(PlayerSettings))]
[assembly: Settings(typeof(PlayerSettings.Android))]
[assembly: Settings(typeof(PlayerSettings.iOS))]
[assembly: Settings(typeof(PlayerSettings.EmbeddedLinux))]
[assembly: Settings(typeof(PlayerSettings.Lumin))]
[assembly: Settings(typeof(PlayerSettings.macOS))]
[assembly: Settings(typeof(PlayerSettings.PS4))]
[assembly: Settings(typeof(PlayerSettings.QNX))]
[assembly: Settings(typeof(PlayerSettings.SplashScreen))]
[assembly: Settings(typeof(PlayerSettings.Switch))]
[assembly: Settings(typeof(PlayerSettings.tvOS))]
[assembly: Settings(typeof(PlayerSettings.VisionOS))]
[assembly: Settings(typeof(PlayerSettings.WebGL))]
[assembly: Settings(typeof(PlayerSettings.WSA))]
[assembly: Settings(typeof(PlayerSettings.XboxOne))]
[assembly: Settings(typeof(EditorSettings))]
[assembly: Settings(typeof(EditorBuildSettings))]
[assembly: Settings(typeof(EditorUserBuildSettings))]
