using System;
using System.IO;
using UnityEngine;


namespace SettingsManagement.Editor
{
    [Serializable]
    public sealed class EditorSettingsRepository : FileSettingsRepository
    {
        [SerializeField]
        private string package;

        public EditorSettingsRepository(string package, string name = "Settings")
            : base(GetSettingsPath(package, name), SettingsScope.UnityEditor, name)
        {
            this.package = package;

        }

        static string GetSettingsPath(string package, string name)
        {
            if (package == null)
                throw new ArgumentNullException(nameof(package));
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            string filePath;

            filePath = Path.Combine(GetSettingsFolder(SettingsScope.UnityEditor),
                "Packages",
                 package,
                 $"{name}.json");
            return filePath;
        }
    }
}