using System;
using System.IO;
using UnityEngine;

namespace Unity.SettingsManagement
{
    //PackageSettingsRepository 类名被 Unity 内部使用, npm 下载的包 Unity 会剔除 PackageSettingsRepository 类, 导致缺少这个类
    [Serializable]
    public class PackageSettingRepository : FileSettingsRepository
    {
        [SerializeField]
        private string package;


        public PackageSettingRepository(string package, SettingsScope scope, string name = "Settings")
            : base(GetSettingsPath(scope, package, name), scope, name)
        {
            if (package == null)
                throw new ArgumentNullException(nameof(package));

            this.package = package;
        }

        public PackageSettingRepository(string baseDir, string package, SettingsScope scope, string name = "Settings")
            : base(GetSettingsPath(scope, package, name, baseDir), scope, name)
        {
            if (package == null)
                throw new ArgumentNullException(nameof(package));
            this.package = package;
        }


        public static string GetSettingsPath(SettingsScope scope, string package, string name, string baseDir = null)
        {
            string filePath;

            filePath = FileSettingsRepository.GetSettingsFolder(scope, baseDir);

            filePath = Path.Combine(filePath, "Packages", package);

            filePath = Path.Combine(filePath, name);

            filePath += ".json";

            filePath = filePath.Replace('\\', '/');
            return filePath;
        }

        public static string GetSettingsDir(SettingsScope scope, string package, string baseDir = null)
        {
            string path = GetSettingsPath(scope, package, "Settings", baseDir);
            return Path.GetDirectoryName(path);
        }

    }
}