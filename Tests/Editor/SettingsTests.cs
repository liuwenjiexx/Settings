using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Unity.SettingsManagement.Tests
{

    public class SettingsTestsBase
    {
        static string PackageName = "com.unity.settings";
        static Settings settings;


        public static Settings Settings
        {
            get
            {
                if (settings == null)
                    settings = new Settings(
                        new PackageSettingRepository(PackageName, SettingsScope.EditorProject, "Test"),
                        new PackageSettingRepository(PackageName, SettingsScope.EditorUser, "Test"),
                        new PackageSettingRepository(PackageName, SettingsScope.RuntimeProject, "Test"),
                        new PackageSettingRepository(PackageName, SettingsScope.RuntimeUser, "Test"));
                return settings;
            }
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {

            foreach (var repo in Settings.Repositories)
            {
                if (File.Exists(repo.FilePath))
                {
                    File.Delete(repo.FilePath);
                }
            }

            foreach (var field in typeof(SettingsTestsBase).Assembly
                .GetTypes()
                .SelectMany(o => o.GetFields(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)))
            {
                Type valueType = field.FieldType;
                if (!valueType.IsGenericType)
                    continue;
                if (!(valueType.GetGenericTypeDefinition() == typeof(Setting<>)))// ||
                                                                                 //valueType.GetGenericTypeDefinition() == typeof(PlatformSetting<>)))
                {
                    continue;
                }

                var resetMethod = valueType.GetMethod("Reset", new Type[] { typeof(bool) });
                object obj;
                if (field.IsStatic)
                {
                    obj = field.GetValue(null);
                }
                else
                {
                    obj = field.GetValue(this);
                }
                resetMethod.Invoke(obj, new object[] { false });
            }


        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {




        }

    }

    public class SettingsTests : SettingsTestsBase
    {

        static Setting<string> staticStringEditorProject = new(Settings, "string", "abc123", SettingsScope.EditorProject);
        static Setting<string> staticStringRuntimeProject = new(Settings, "string", "abc123", SettingsScope.RuntimeProject);
        static Setting<string> staticStringEditorUser = new(Settings, "string", "abc123", SettingsScope.EditorUser);
        static Setting<string> staticStringRuntimeUser = new(Settings, "string", "abc123", SettingsScope.RuntimeUser);
        static Setting<bool> staticBoolEditorProject = new(Settings, "bool", true, SettingsScope.EditorProject);



        [Test]
        public void String()
        {
            Assert.AreEqual("abc123", staticStringEditorProject.Value);
            staticStringEditorProject.SetValue("abc", true);
            Assert.AreEqual("abc", staticStringEditorProject.Value);

        }


        [Test]
        public void Bool()
        {
            Assert.AreEqual(true, staticBoolEditorProject.Value);
            staticBoolEditorProject.SetValue(false, true);
            Assert.AreEqual(false, staticBoolEditorProject.Value);
        }



        [Test]
        public void Reset()
        {
            Assert.AreEqual("abc123", staticStringRuntimeProject.Value);
            staticStringRuntimeProject.SetValue("abc", true);
            Assert.AreEqual("abc", staticStringRuntimeProject.Value);
            staticStringRuntimeProject.Reset();
            Assert.AreEqual("abc123", staticStringRuntimeProject.Value);
        }


        [Test]
        public void TTT()
        {
            string json = "{\r\n    \"package\": \"com.unity.settings\",\r\n    \"name\": \"MySettings\",\r\n    \"scope\": 1,\r\n    \"items\": []\r\n}";

            PackageSettingRepository obj = new("aaa", SettingsScope.RuntimeProject, "Settings");
            JsonUtility.FromJsonOverwrite(json, obj);


        }

    }
}