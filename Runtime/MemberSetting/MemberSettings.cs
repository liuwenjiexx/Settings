using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SettingsManagement
{

    public class MemberSettings
    {
        public MemberSettings(Settings settings, string key, SettingsScope scope = SettingsScope.RuntimeProject)
        {
            members = new(settings, key, new(), scope);
            InitializeSettings();
        }

        private Setting<List<MemberSettingInfo>> members;
        private List<IMemberSetting> settingList = new();

        public Settings Settings => members.Settings;

        public IReadOnlyList<IMemberSetting> SettingList
        {
            get => settingList;
        }

        private void InitializeSettings()
        {
            settingList.Clear();
            var items = members.Value;
            SettingsScope scope = members.Scope;

            foreach (var item in items)
            {
                if (item.Member != null)
                {
                    var setting = CreateSetting(item);
                    if (setting == null)
                        continue;
                    settingList.Add(setting);
                }
            }
        }

        internal static string GetKey(MemberInfo memberInfo)
        {
            if (memberInfo == null) throw new ArgumentNullException(nameof(memberInfo));

            return $"{memberInfo.ReflectedType.FullName}:{memberInfo.Name}";
        }

        static bool EqualMember(MemberInfo a, MemberInfo b)
        {
            if (a == null)
            {
                return b == null;
            }
            else if (b == null)
            {
                return a == null;
            }
            if (a.DeclaringType != b.DeclaringType)
                return false;
            return a.Name.Equals(b.Name);
        }

        public bool ContainsMember(MemberInfo memberInfo)
        {
            var list = members.Value;
            if (list == null) return false;

            foreach (var mInfo in list)
            {
                if (EqualMember(mInfo.Member, memberInfo))
                    return true;
            }
            return false;
        }

        public IMemberSetting AddSetting(MemberInfo memberInfo)
        {
            var list = members.Value;
            if (list == null)
                list = new();

            foreach (var mInfo in list)
            {
                if (EqualMember(mInfo.Member, memberInfo))
                    return null;
            }

            MemberSettingInfo member = new MemberSettingInfo(memberInfo);

            list.Add(member);
            members.SetValue(list, true);
            var setting = CreateSetting(member);
            settingList.Add(setting);
            return setting;
        }

        public bool DeleteSetting(IMemberSetting setting)
        {
            var member = setting.TargetMember;
            if (!settingList.Remove(setting))
                return false;

            if (member != null)
            {
                var list = members.Value;
                if (list == null)
                    return false;

                for (int i = 0; i < list.Count; i++)
                {
                    if (EqualMember(list[i].Member, member))
                    {
                        list.RemoveAt(i);
                        break;
                    }
                }
                members.SetValue(list, true);
            }
            return true;
        }

        private int _FindSettingMemberIndex(MemberInfo member)
        {
            if (member == null)
                return -1;
            var items = members.Value;

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                if (EqualMember(item.Member, member))
                {
                    return i;
                }
            }
            return -1;
        }

        public bool MoveSetting(IMemberSetting setting, int newIndex)
        {
            int index = settingList.IndexOf(setting);
            if (index == -1)
                return false;
            if (newIndex < 0 || newIndex >= settingList.Count)
                return false;
            if (index == newIndex)
                return false;

            int index2 = _FindSettingMemberIndex(setting.TargetMember);
            int newIndex2 = _FindSettingMemberIndex(settingList[newIndex].TargetMember);

            if (index2 == -1 || newIndex2 == -1)
            {
                return false;
            }

            settingList[index] = settingList[newIndex];
            settingList[newIndex] = setting;

            var items = members.Value;

            var memberMeta = items[index2];
            items[index2] = items[newIndex2];
            items[newIndex2] = memberMeta;
            members.SetValue(items, true);
            return true;
        }

        private IMemberSetting CreateSetting(MemberSettingInfo member)
        {
            var memberInfo = member.Member;
            var pInfo = memberInfo as PropertyInfo;
            Type valueType = null;

            if (pInfo != null)
            {
                valueType = pInfo.PropertyType;

            }
            else
            {
                var fInfo = memberInfo as FieldInfo;
                if (fInfo != null)
                {
                    valueType = fInfo.FieldType;
                }
            }

            if (valueType == null || valueType == typeof(void))
                return null;

            Type settingType = typeof(MemberSetting<>).MakeGenericType(valueType);
            IMemberSetting setting = Activator.CreateInstance(settingType, new object[] { this, memberInfo, SettingsUtility.GetDefualtValue(valueType), members.Scope }) as IMemberSetting;


            return setting;
        }


    }

}
