using System;
using UnityEngine;

namespace SettingsManagement
{
    [Serializable]
    sealed class ValueWrapper<T>
    {
        //#if PRETTY_PRINT_JSON
        //const bool PrettyPrintJson = true;
        //#else
        const bool PrettyPrintJson = false;
        //#endif

        [SerializeField]
        T value;

        public static string Serialize(T value)
        {
            var obj = new ValueWrapper<T>() { value = value };

            return JsonUtility.ToJson(obj, PrettyPrintJson);
        }

        public static T Deserialize(string json)
        {
            var value = new ValueWrapper<T>();
            JsonUtility.FromJsonOverwrite(json, value);
            return value.value;
        }

        public static T Copy(T value)
        {
            if (typeof(ValueType).IsAssignableFrom(typeof(T)))
                return value;
            var str = Serialize(value);
            return Deserialize(str);
        }
    }
}
