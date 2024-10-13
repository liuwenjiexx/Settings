//using System;
//using System.Collections;
//using System.Collections.Generic;
//using Unity.Plastic.Newtonsoft.Json.Linq;
//using Unity.VisualScripting.YamlDotNet.Core.Tokens;
//using UnityEditor.Build;
//using UnityEngine;


//namespace Unity.SettingsManagement
//{
//    public class PlatformSetting<T>
//    {
//        private bool initialized;
//        private Settings settings;
//        private string key;
//        private SettingsScope scope;
//        private string repositoryName;

//        private T defaultValue;

//        public PlatformSetting(Settings settings, string key, T value, SettingsScope scope = SettingsScope.RuntimeProject)
//            : this(settings, key, value, null, scope)
//        {
//        }

//        public PlatformSetting(Settings settings, string key, T value, string repository, SettingsScope scope = SettingsScope.RuntimeProject)
//        {
//            if (settings == null)
//                throw new ArgumentNullException(nameof(settings));
//            if (key == null)
//                throw new ArgumentNullException(nameof(key));

//            this.settings = settings;
//            this.key = key;
//            this.defaultValue = value;
//            this.repositoryName = repository;
//            this.scope = scope;
//            initialized = false;

//        }

//        public Settings Settings => settings;

//        public string Key => key;

//        public SettingsScope Scope => scope;

//        public string RepositoryName => repositoryName;

//        public T DefaultValue
//        {
//            get
//            {
//                return ValueWrapper<T>.Copy(defaultValue);
//            }
//        }


//        public T Value
//        {
//            get
//            {
//                Initialize();
//                return GetValue(CurrentPlatform);
//            }
//            set
//            {
//                SetValue(CurrentPlatform, value, true);
//            }
//        }

//        public Type ValueType = typeof(T);

//        public static SettingsPlatform CurrentPlatform
//        {
//            get
//            {

//#if UNITY_ANDROID
//                return  SettingsPlatform.Android;
//#elif UNITY_IOS
//                return SettingsPlatform.iOS;
//#elif UNITY_SERVER
//                return SettingsPlatform.Server;
//#else
//                return SettingsPlatform.Standalone;
//#endif
//            }
//        }

//        private void Initialize()
//        {
//            if (initialized) return;

//            initialized = true;

//            if (!settings.ContainsKey<T>(key, repositoryName, scope))
//            {
//                PlatformValue<T> platformValue = new PlatformValue<T>();
//                platformValue.items.Add(new(SettingsPlatform.Default, DefaultValue));
//                settings.Set(key, platformValue, repositoryName, scope);
//            }
//        }

//        public T GetValue(SettingsPlatform platform)
//        {
//            Initialize();

//            var platformValue = settings.Get<PlatformValue<T>>(key, repositoryName, scope);

//            T value;
//            if (platformValue == null)
//            {
//                value = DefaultValue;
//            }
//            else if (!platformValue.TryGetValue(platform, out value))
//            {
//                if (platform == SettingsPlatform.Server && platformValue.TryGetValue(SettingsPlatform.Standalone, out value))
//                {

//                }
//                else if (!platformValue.TryGetValue(SettingsPlatform.Default, out value))
//                {
//                    value = DefaultValue;
//                }
//            }

//            return value;
//        }

//        public void SetValue(T value, bool saveImmediate = false)
//        {
//            SetValue(CurrentPlatform, value, saveImmediate);
//        }


//        public void SetValue(SettingsPlatform platform, T value, bool saveImmediate = false)
//        {
//            Initialize();

//            var oldValue = GetValue(platform);

//            if (!object.Equals(oldValue, value))
//            {
//                var platformValue = settings.Get<PlatformValue<T>>(key, repositoryName, scope);
//                platformValue.SetValue(platform, value);
//                settings.Set(key, platformValue, repositoryName, scope);
//                if (saveImmediate)
//                {
//                    settings.Save();
//                }
//            }
//        }


//        public void Delete(bool saveImmediate = false)
//        {
//            Initialize();
//            if (settings.ContainsKey<PlatformValue<T>>(key, repositoryName, scope))
//            {
//                settings.DeleteKey<PlatformValue<T>>(key, repositoryName, scope);
//                if (saveImmediate)
//                {
//                    settings.Save();
//                }
//            }
//            initialized = false;
//        }

//        public void Delete(SettingsPlatform platform, bool saveImmediate = false)
//        {
//            Initialize();

//            var platformValue = settings.Get<PlatformValue<T>>(key, repositoryName, scope);

//            if (platformValue.Contains(platform))
//            {
//                platformValue.Delete(platform);
//                if (saveImmediate)
//                {
//                    settings.Save();
//                }
//            }
//        }

//        public void Reset(bool saveImmediate = false)
//        {
//            Reset(CurrentPlatform, saveImmediate);
//        }

//        public void Reset(SettingsPlatform platform, bool saveImmediate = false)
//        {
//            Initialize();
//            SetValue(platform, DefaultValue, saveImmediate);
//        }


//        public override string ToString()
//        {
//            return $"{scope} setting. Key: {key}  Value: {Value}";
//        }

//    }


//    [Serializable]
//    internal class PlatformValue<T>
//    {
//        [Serializable]
//        internal struct Entry : ISerializationCallbackReceiver
//        {
//            [NonSerialized]
//            public SettingsPlatform Platform;
//            [SerializeField]
//            private string platform;
//            public T value;


//            public Entry(SettingsPlatform platform, T value)
//            {
//                this.Platform = platform;
//                this.value = value;
//                this.platform = platform.ToString();
//            }

//            public void OnBeforeSerialize()
//            {

//            }

//            public void OnAfterDeserialize()
//            {
//                if (!string.IsNullOrEmpty(platform) && !Enum.TryParse(platform, out Platform))
//                {

//                }
//            }
//        }

//        [SerializeField]
//        internal List<Entry> items = new();

//        public bool Contains(SettingsPlatform platform)
//        {
//            foreach (var item in items)
//            {
//                if (item.Platform == platform)
//                {
//                    return true;
//                }
//            }
//            return false;
//        }

//        private int FindIndex(SettingsPlatform platform)
//        {
//            int index = -1;
//            for (int i = 0; i < items.Count; i++)
//            {
//                var item = items[i];
//                if (item.Platform == platform)
//                {
//                    index = i;
//                    break;
//                }
//            }
//            return index;
//        }

//        public bool TryGetValue(SettingsPlatform platform, out T value)
//        {
//            var index = FindIndex(platform);
//            if (index == -1)
//            {
//                value = default;
//                return false;
//            }
//            value = items[index].value;
//            return true;
//        }

//        public void SetValue(SettingsPlatform platform, T value)
//        {
//            var index = FindIndex(platform);
//            if (index >= 0)
//            {
//                items[index] = new Entry(platform, value);
//            }
//            else
//            {
//                items.Add(new Entry(platform, value));
//            }
//        }

//        public void Delete(SettingsPlatform platform)
//        {
//            var index = FindIndex(platform);
//            if (index >= 0)
//            {
//                items.RemoveAt(index);
//            }
            
//        }

//    }

//    public enum SettingsPlatform
//    {
//        Default,
//        Standalone,
//        Android,
//        iOS,
//        Server
//    }

//}



