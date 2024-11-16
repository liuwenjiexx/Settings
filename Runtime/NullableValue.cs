using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;

namespace UnityEngine
{


    /// <summary>
    /// 支持 <see cref="Nullable{T}"/> Unity序列化
    /// </summary>
    [Serializable]
    public struct NullableValue<T>
        where T : struct
    {
        [SerializeField]
        private bool hasValue;

        [SerializeField]
        private T value;

        public bool HasValue => hasValue;

        public T Value => hasValue ? value : Null.value;

        public NullableValue(T value)
        {
            this.value = value;
            hasValue = true;
        }


        public static readonly NullableValue<T> Null = new NullableValue<T>() { hasValue = false };

        //public static implicit operator NullableValue<in T>(object value)
        //{
        //    if (value == null) return Null;
        //    return new NullableValue<T>((T)value);
        //}

        public static implicit operator NullableValue<T>(T value) => new NullableValue<T>(value);

        public static implicit operator T(NullableValue<T> value) => value.hasValue ? value.Value : Null.value;

        public static implicit operator NullableValue<T>(Nullable<T> value) => value.HasValue ? new NullableValue<T>(value.Value) : Null;

        public static implicit operator Nullable<T>(NullableValue<T> value) => value.HasValue ? new Nullable<T>(value.Value) : null;


        public override int GetHashCode()
        {
            return hasValue ? value.GetHashCode() : 0;
        }


        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                if (!hasValue)
                    return true;
                return false;
            }

            if (obj is Nullable<T>)
            {
                var obj2 = (Nullable<T>)obj;
                if (hasValue != obj2.HasValue)
                    return false;
                return object.Equals(value, obj2.Value);
            }
            return false;
        }


        public override string ToString()
        {
            if (!hasValue)
                return "";
            return Value.ToString();
        }
    }

}
