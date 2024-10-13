using System.Reflection;


namespace Unity.SettingsManagement
{

    public class MemberSetting<T> : Setting<T>, IMemberSetting
    {
        public MemberSetting(MemberSettings settings, MemberInfo targetMember, T value, SettingsScope scope = SettingsScope.RuntimeProject)
            : base(settings.Settings, MemberSettings.GetKey(targetMember), value, scope, combine: IsCombineValue(targetMember))
        {
            this.OwnerSettings = settings;
            this.TargetMember = targetMember;
        }

        public MemberSettings OwnerSettings { get; private set; }

        public MemberInfo TargetMember { get; private set; }

        static bool IsCombineValue(MemberInfo member)
        {

            if (member.IsDefined(typeof(CombineAttribute), true))
            {
                return true;
            }
            return false;
        }


        public bool TryApplyValue(object instance, string platform, string variant)
        {
            if (TargetMember == null)
                return false;
            T newValue;
            if (IsCombine)
            {
                SettingsUtility.GetCombineValues(this, platform, variant, out newValue);
            }
            else
            {
                if (!TryGetValue(platform, variant, out newValue))
                {
                    return false;
                }
            }

            var pInfo = TargetMember as PropertyInfo;
            if (pInfo != null)
            {
                var setter = pInfo.GetSetMethod();
                if (setter != null)
                {
                    pInfo.SetValue(instance, newValue);
                    return true;
                }
            }
            else
            {
                var fInfo = TargetMember as FieldInfo;
                if (!fInfo.IsInitOnly)
                {
                    fInfo.SetValue(instance, newValue);
                    return true;
                }
            }
            return false;
        }

        public bool TryApplyValueComparer(object instance, string platform, string variant)
        {
            if (TargetMember == null)
                return false;
            T newValue;
            if (IsCombine)
            {
                SettingsUtility.GetCombineValues(this, platform, variant, out newValue);
            }
            else
            {
                if (!TryGetValue(platform, variant, out newValue))
                {
                    return false;
                }
            }

            object oldValue;
            if (TargetMember is PropertyInfo pInfo)
            {
                oldValue = pInfo.GetValue(instance);
                if (!object.Equals(oldValue, newValue))
                {
                    var setter = pInfo.GetSetMethod();
                    if (setter != null)
                    {
                        pInfo.SetValue(instance, newValue);
                        return true;
                    }
                }
            }
            else if (TargetMember is FieldInfo fInfo)
            {
                oldValue = fInfo.GetValue(instance);
                if (!object.Equals(oldValue, newValue))
                {
                    if (!fInfo.IsInitOnly)
                    {
                        fInfo.SetValue(instance, newValue);
                        return true;
                    }
                }
            }
            return false;
        }

    }


}