using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using SettingsManagement;
using SettingsManagement.Editor;

namespace Unity.Text.Editor
{
    public class FileEncodingUserSettings
    {
        private static Settings settings;

        public static Settings Settings
        => settings ??= new Settings(
            new EditorSettingsRepository(SettingsUtility.GetPackageName(typeof(FileEncodingUserSettings)), "FileEncodingSettings"),
            new PackageSettingRepository(SettingsUtility.GetPackageName(typeof(FileEncodingUserSettings)), SettingsScope.EditorUser, "FileEncodingSettings"));


        [InspectorName("Global Enabled")]
        private static Setting<bool> globalEnabled = new(Settings, nameof(GlobalEnabled), false, SettingsScope.UnityEditor);
        public static bool GlobalEnabled
        {
            get => globalEnabled.Value;
            set => globalEnabled.SetValue(value, true);
        }

        [InspectorName("Global Rules")]
        private static Setting<List<EncodingRule>> globalRules = new(Settings, nameof(GlobalRules), new(), SettingsScope.UnityEditor);
        public static List<EncodingRule> GlobalRules
        {
            get => globalRules.Value;
            set => globalRules.SetValue(value, true);
        }
         

        [InspectorName("Project Enabled")]
        private static Setting<bool> enabled = new(Settings, nameof(Enabled), false, SettingsScope.EditorUser);
        public static bool Enabled
        {
            get => enabled.Value;
            set => enabled.SetValue(value, true);
        }
    }
 


    [Serializable]
    public class EncodingRule
    {
        [SerializeField]
        private string encodingName;
        [SerializeField]
        public int codePage;
        [SerializeField]
        private string extension;
        [SerializeField]
        private string filter;

        private Encoding encoding;
        private Encoding testEncoding;

        public string EncodingName
        {
            get => encodingName;
            set
            {
                encodingName = value;
                encoding = null;
                testEncoding = null;
            }
        }

        public string Filter
        {
            get => filter;
            set
            {
                filter = value;
                filterRegex = null;
            }
        }
        public string Extension
        {
            get => extension;
            set
            {
                extension = value;
                extensionList = null;
            }
        }



        private Regex filterRegex;
        private string[] extensionList;
        public bool IsMatchPath(string path)
        {
            if (!string.IsNullOrEmpty(Filter) && filterRegex == null)
            {
                filterRegex = new Regex(Filter, RegexOptions.IgnoreCase);
            }

            if (!string.IsNullOrEmpty(Extension) && extensionList == null)
            {
                extensionList = Extension.Split(new char[] { '|', ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
            }

            if (filterRegex != null)
            {
                if (!filterRegex.IsMatch(path))
                {
                    return false;
                }
            }
            if (extensionList != null && extensionList.Length > 0)
            {
                if (!extensionList.Any(o => path.EndsWith(o, StringComparison.InvariantCultureIgnoreCase)))
                {
                    return false;
                }
            }

            return true;
        }

        public Encoding Encoding
        {
            get
            {
                if (encoding != null)
                {
                    return encoding;
                }
                if (!string.IsNullOrEmpty(encodingName))
                {
                    encoding = EncodingNames.GetEncoding(encodingName);
                }
                return encoding;
            }
        }

        public Encoding TestEncoding
        {
            get
            {
                if (testEncoding != null)
                    return testEncoding;
                var encoding = Encoding;
                if (encoding != null)
                {
                    testEncoding = Encoding.GetEncoding(encoding.CodePage, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
                }
                return testEncoding;
            }
        }

    }
}