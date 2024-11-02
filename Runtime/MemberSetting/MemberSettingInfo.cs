using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SettingsManagement
{

    [Serializable]
    class MemberSettingInfo
    {
        [SerializeField]
        private string name;

        [SerializeField]
        private string typeName;

        private MemberInfo member;


        public MemberSettingInfo(MemberInfo member)
        {
            if (member == null) throw new ArgumentNullException(nameof(member));

            typeName = member.DeclaringType.AssemblyQualifiedName;
            name = member.Name;
        }


        public MemberInfo Member
        {
            get
            {
                if (member == null)
                {
                    Type type = Type.GetType(typeName);
                    BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static;
                    member = type.GetProperty(name, bindingFlags);
                    if (member == null)
                    {
                        member = type.GetField(name, bindingFlags);
                    }
                }
                return member;
            }
        }

    }
}
