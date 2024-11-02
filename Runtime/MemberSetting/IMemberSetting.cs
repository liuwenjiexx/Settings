using System.Reflection;

namespace SettingsManagement
{
    public interface IMemberSetting : ISetting
    {
        MemberSettings OwnerSettings { get; }

        MemberInfo TargetMember { get; }

        bool TryApplyValue(object instance, string platform, string variant);
        bool TryApplyValueComparer(object instance, string platform, string variant);
    }
}
