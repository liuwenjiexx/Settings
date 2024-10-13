namespace Unity.SettingsManagement
{
    public enum SettingsScope
    {
        /// <summary>
        /// Repository: Assets/Resources/ProjectSettings/Packages/[PackageName]
        /// </summary>
        RuntimeProject = 1,

        /// <summary>
        /// Repository: [persistentDataPath]/UserSettings/Packages/[PackageName]
        /// </summary>
        RuntimeUser,

        /// <summary>
        /// Repository: ProjectSettings/Packages/[PackageName]
        /// </summary> 
        EditorProject,

        /// <summary>
        /// Repository: UserSettings/Packages/[PackageName]
        /// </summary> 
        EditorUser,

        /// <summary>
        /// Windows: %LOCALAPPDATA%/Unity/Editor/EditorSettings/Packages/<PackageName>
        /// </summary>
        UnityEditor,
    }
}