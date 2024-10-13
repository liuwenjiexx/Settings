using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Unity.SettingsManagement
{
    public interface ICombineSetting : ISetting
    {
    }

    public class CombineSetting<T> : Setting<List<T>>, ICombineSetting
    {
        public CombineSetting(Settings settings, string key, List<T> value, SettingsScope scope = SettingsScope.RuntimeProject) :
            this(settings, key, value, null, scope)
        {
        }

        public CombineSetting(Settings settings, string key, List<T> value, string repository, SettingsScope scope = SettingsScope.RuntimeProject) :
            base(settings, key, value, repository, scope,combine:true)
        {

        }

        public CombineSetting(GetParentDelegate2 getParent, Settings settings, string key, List<T> value, SettingsScope scope = SettingsScope.RuntimeProject) :
            this(getParent, settings, key, value, null, scope)
        {
        }

        public CombineSetting(GetParentDelegate2 getParent, Settings settings, string key, List<T> value, string repository, SettingsScope scope = SettingsScope.RuntimeProject) :
            base(getParent == null ? null :
                (key) =>
                {
                    return getParent(key);
                },
                settings, key, value, repository, scope, combine: true)
        {
        }


        public delegate CombineSetting<T> GetParentDelegate2(string key);


        public override List<T> GetValue(string platform, string variant)
        {
            Initialize();
            List<T> list = new();
            SettingsUtility.GetCombineValues(this, platform, variant,   list);
            return list;
        }





    }
}