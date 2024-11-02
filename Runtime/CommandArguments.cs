using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
namespace SettingsManagement
{
    class CommandArguments : IEnumerable<string>
    {
        private List<string> list = new();
        private IEqualityComparer<string> keyComparer;

        public CommandArguments(bool ignoreCase = false)
            : this(null, ignoreCase)
        {
        }

        public CommandArguments(IEqualityComparer<string> keyComparer)
            : this(null, keyComparer)
        {
        }

        public CommandArguments(IEnumerable<string> args, bool ignoreCase = false)
            : this(args, (ignoreCase ? StringComparer.InvariantCultureIgnoreCase : StringComparer.Ordinal))
        {
        }

        public CommandArguments(IEnumerable<string> args, IEqualityComparer<string> keyComparer)
        {
            if (keyComparer != null)
            {
                this.keyComparer = keyComparer;
            }
            else
            {
                this.keyComparer = StringComparer.Ordinal;
            }

            if (args != null)
            {
                AddArgs(args);
            }

        }


        public string this[int index]
        {
            get
            {

                if (index < 0 || index >= list.Count)
                    throw new ArgumentOutOfRangeException("index");
                return list[index];
            }
        }

        public string this[string name]
        {
            get => Get(name);
            set
            {
                int index = FindIndex(name);
                if (index >= 0)
                {
                    list[index + 1] = value;
                }
                else
                {
                    list.Add(name);
                    list.Add(value);
                }
            }
        }

        public List<string> List => list;

        public int Count => list.Count;

        public bool Has(string name)
        {
            return FindIndex(name) != -1;
        }


        public int FindIndex(string name)
        {
            if (name != null)
            {
                name = name.Trim();
            }

            for (int i = 0; i < list.Count; i++)
            {
                string s = list[i];
                if (s != null)
                {
                    s = s.Trim();
                }
                if (keyComparer.Equals(s, name))
                {
                    return i;
                }
            }

            return -1;
        }

        public void AddArgs(IEnumerable<string> args)
        {
            if (args == null)
                return;

            foreach (var arg in args)
            {
                this.list.Add(arg);
            }
        }


        public bool TryGet(string name, out string value)
        {
            value = null;
            int index = FindIndex(name);
            if (index == -1)
                return false;
            if (index + 1 < list.Count)
            {
                value = list[index + 1];
                return true;
            }
            return false;
        }

        public string Get(string name)
        {
            if (TryGet(name, out var value))
            {
                return value;
            }
            throw new KeyNotFoundException($"Not found argument name: {name}");
        }


        public T Get<T>(string name)
        {
            string value = Get(name);
            var type = typeof(T);
            try
            {
                if (type.IsPrimitive)
                {
                    TypeCode typeCode = Type.GetTypeCode(type);
                    switch (typeCode)
                    {
                        case TypeCode.String:
                            return (T)(object)value;
                        case TypeCode.Boolean:
                            return (T)(object)bool.Parse(value);
                        case TypeCode.Int32:
                            return (T)(object)int.Parse(value);
                        case TypeCode.UInt32:
                            return (T)(object)uint.Parse(value);
                        case TypeCode.Single:
                            return (T)(object)float.Parse(value);
                        case TypeCode.Double:
                            return (T)(object)double.Parse(value);
                        case TypeCode.Int64:
                            return (T)(object)long.Parse(value);
                        case TypeCode.UInt64:
                            return (T)(object)ulong.Parse(value);
                    }
                }
                else if (type == typeof(string))
                {
                    return (T)(object)value;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Format error. name: '{name}' value: '{value}'", ex);
            }
            throw new NotImplementedException();
        }

        public bool TryGet<T>(string name, out T value)
        {
            value = default;
            int index = FindIndex(name);
            if (index == -1)
                return false;
            if (index + 1 >= list.Count)
            {
                return false;
            }

            string strValue = list[index + 1];

            var type = typeof(T);
            try
            {
                if (type.IsPrimitive)
                {
                    TypeCode typeCode = Type.GetTypeCode(type);
                    switch (typeCode)
                    {
                        case TypeCode.String:
                            value = (T)(object)strValue;
                            return true;
                        case TypeCode.Boolean:
                            if (!bool.TryParse(strValue, out var b))
                                return false;
                            value = (T)(object)b;
                            return true;
                        case TypeCode.Int32:
                            if (!int.TryParse(strValue, out var i32))
                                return false;
                            value = (T)(object)i32;
                            return true;
                        case TypeCode.UInt32:
                            if (!uint.TryParse(strValue, out var u32))
                                return false;
                            value = (T)(object)u32;
                            return true;
                        case TypeCode.Single:
                            if (!float.TryParse(strValue, out var f32))
                                return false;
                            value = (T)(object)f32;
                            return true;
                        case TypeCode.Double:
                            if (!double.TryParse(strValue, out var f64))
                                return false;
                            value = (T)(object)f64;
                            return true;
                        case TypeCode.Int64:
                            if (!long.TryParse(strValue, out var i64))
                                return false;
                            value = (T)(object)i64;
                            return true;
                        case TypeCode.UInt64:
                            if (!ulong.TryParse(strValue, out var u64))
                                return false;
                            value = (T)(object)u64;
                            return true;

                    }
                }
                else if (type == typeof(string))
                {
                    value = (T)(object)strValue;
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Format error. name: '{name}' value: '{value}', type: {type.Name}", ex);
            }
            throw new NotImplementedException($"CommandArgs Error Key: {name}, Type: {type.Name}");
        }

        public bool GetBool(string name)
        {
            return bool.Parse(Get(name));
        }

        public bool Get(string name, ref string value)
        {
            if (TryGet(name, out var newValue))
            {
                if (value != newValue)
                {
                    value = newValue;
                    return true;
                }
            }
            return false;
        }

        public bool Get(string name, ref int value)
        {
            if (TryGet(name, out var newValue))
            {
                if (int.TryParse(newValue, out var v))
                {
                    if (value != v)
                    {
                        value = v;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool Get(string name, ref bool value)
        {
            if (TryGet(name, out var newValue))
            {
                if (bool.TryParse(newValue, out var v))
                {
                    if (value != v)
                    {
                        value = v;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool Get(string name, ref uint value)
        {
            if (TryGet(name, out var newValue))
            {
                if (uint.TryParse(newValue, out var v))
                {
                    if (value != v)
                    {
                        value = v;
                        return true;
                    }
                }
            }
            return false;
        }
        public bool Get(string name, ref ushort value)
        {
            if (TryGet(name, out var newValue))
            {
                if (ushort.TryParse(newValue, out var v))
                {
                    if (value != v)
                    {
                        value = v;
                        return true;
                    }
                }
            }
            return false;
        }


        public bool Get(string name, ref float value)
        {
            if (TryGet(name, out var newValue))
            {
                if (float.TryParse(newValue, out var v))
                {
                    if (value != v)
                    {
                        value = v;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool GetString2(string name, out string value)
        {
            value = null;
            for (int i = 0; i < list.Count; i++)
            {
                string str = list[i];
                int index = str.IndexOf("=");
                if (index == -1)
                    continue;

                if (!keyComparer.Equals(str.Substring(0, index), name))
                {
                    continue;
                }
                if (index >= str.Length - 1)
                    value = string.Empty;
                else
                    value = str.Substring(index + 1);
                return true;
            }
            return false;
        }

        public bool GetUInt322(string name, out uint value)
        {
            value = 0;
            if (!GetString2(name, out var str))
                return false;
            return uint.TryParse(str, out value);
        }

        public void Fill(object target, ConverterDelegate converter = null)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));
            Type type = target.GetType();

            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetField | BindingFlags.SetProperty;
            bindingFlags |= BindingFlags.Instance;

            _Fill(type, ref target, converter, flags: bindingFlags);
        }

        public void Fill(Type type, ConverterDelegate converter = null)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            object target = null;

            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetField | BindingFlags.SetProperty;
            bindingFlags |= BindingFlags.Static;

            _Fill(type, ref target, converter, flags: bindingFlags);
        }

        bool _Fill(Type type, ref object target, ConverterDelegate converter, BindingFlags flags)
        {
            string argName;
            string strValue;
            object value;
            object oldValue;
            Type valueType = null;
            PropertyInfo pInfo;
            FieldInfo fInfo;
            bool hasValue;
            bool changed = false;


            foreach (var member in type.GetMembers(flags))
            {
                value = null;
                strValue = null;
                hasValue = false;

                valueType = null;
                fInfo = null;
                pInfo = null;
                try
                {
                    if (member.MemberType == MemberTypes.Property)
                    {
                        pInfo = (PropertyInfo)member;
                        valueType = pInfo.PropertyType;

                        value = pInfo.GetValue(target);
                    }
                    else if (member.MemberType == MemberTypes.Field)
                    {
                        fInfo = (FieldInfo)member;
                        valueType = fInfo.FieldType;

                        value = fInfo.GetValue(target);
                    }
                    else
                    {
                        continue;
                    }

                    oldValue = value;

                    foreach (var attr in member.GetCustomAttributes<CommandArgumentAttribute>())
                    {
                        argName = null;
                        strValue = null;

                        if (pInfo != null)
                        {
                            if (!pInfo.CanWrite)
                            {
                                Debug.LogWarning($"{pInfo.DeclaringType.Name}.{pInfo.Name} Can't Write");
                                break;
                            }
                        }
                        else if (fInfo.IsInitOnly)
                        {
                            Debug.LogWarning($"{fInfo.DeclaringType.Name}.{fInfo.Name} Can't Write");
                            break;
                        }

                        if (!string.IsNullOrEmpty(attr.Name))
                        {
                            argName = attr.Name;
                        }
                        else
                        {
                            argName = member.Name;
                        }

                        if (attr.Toggle)
                        {
                            if (Has(attr.Name))
                            {
                                value = true;
                                hasValue = true;
                            }
                            else
                            {
                                value = false;
                            }
                            break;
                        }


                        if (TryGet(argName, out strValue))
                        {
                            if (attr.Trim)
                            {
                                if (strValue != null)
                                {
                                    strValue = strValue.Trim();
                                }
                            }
                            value = strValue;
                            hasValue = true;
                            break;
                        }
                    }


                    if (hasValue)
                    {
                        if (value == null)
                        {
                            if (valueType.IsValueType)
                            {
                                value = Activator.CreateInstance(valueType);
                            }
                        }
                        else if (value.GetType() != valueType)
                        {
                            try
                            {
                                if (!(converter != null && converter(ref value, valueType)))
                                {
                                    value = SettingsUtility.ConvertValue(value, valueType);
                                }
                            }
                            catch (Exception e)
                            {
                                throw new Exception($"Convert error TargetType: {valueType?.Name}, String: '{(strValue == null ? "(null)" : strValue)}', StringLength: {strValue?.Length}", e);
                            }
                        }
                    }

                    if (value != null)
                    {
                        Type realValueType = value.GetType();
                        //Type realValueGenType = null;
                        //if (realValueType.IsGenericType)
                        //    realValueGenType = realValueType.GetGenericTypeDefinition();

                        ////递归设置参数
                        //if (!(realValueType.IsPrimitive ||
                        //    realValueType.IsEnum ||
                        //    realValueType == typeof(string) ||
                        //    realValueGenType == typeof(Nullable<>) ||
                        //    realValueGenType == typeof(NullableValue<>) ||
                        //    realValueGenType == typeof(List<>)))
                        //{
                        //    if (_Fill(realValueType, ref value, converter, flags))
                        //    {
                        //        changed = true;
                        //    }
                        //}

                        if (realValueType.IsDefined(typeof(CommandArgumentAttribute), true))
                        {
                            if (_Fill(realValueType, ref value, converter, flags))
                            {
                                changed = true;
                            }
                        }
                    }


                    if (hasValue)
                    {
                        if (!object.Equals(value, oldValue))
                        {
                            changed = true;

                            if (pInfo != null)
                            {
                                if (pInfo.CanWrite)
                                {
                                    pInfo.SetValue(target, value);
                                }
                            }
                            else
                            {
                                if (!fInfo.IsInitOnly)
                                {
                                    fInfo.SetValue(target, value);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new Exception($"Fill Command Line Argument Error, Type: {member.DeclaringType.Name}, Member: {member.Name}, ValueType: {valueType?.Name}", e);
                }
            }

            return changed;
        }

        public static CommandArguments Parse(string commandArgs)
        {
            CommandArguments args = new CommandArguments();
            if (string.IsNullOrEmpty(commandArgs))
                return args;
            args.ParseArgs(commandArgs);
            return args;
        }

        public void ParseArgs(string commandArgs)
        {
            if (string.IsNullOrEmpty(commandArgs))
                return;


            char? quotesChar = null;
            char ch = '\0', prevChar = '\0';

            int quotesCharIndex = 0;
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < commandArgs.Length; i++)
            {
                if (i > 0)
                    prevChar = ch;
                ch = commandArgs[i];

                switch (ch)
                {
                    case '"':
                    case '\'':
                        if (quotesChar == null)
                        {
                            quotesChar = ch;
                            quotesCharIndex = builder.Length;
                        }
                        else
                        {
                            if (quotesChar.Value == ch)
                            {
                                quotesChar = null;
                            }
                        }
                        break;
                    case ' ':
                        if (quotesChar == null)
                        {
                            if (builder.Length > 0)
                            {
                                list.Add(builder.ToString());
                                builder.Clear();
                            }
                        }
                        else
                        {
                            builder.Append(ch);
                        }
                        break;
                    default:
                        builder.Append(ch);
                        break;
                }
            }

            if (builder.Length > 0)
            {
                if (quotesChar != null)
                {
                    builder.Insert(quotesCharIndex, quotesChar.Value);
                }
                list.Add(builder.ToString());
            }

        }


        public string[] ToArray()
        {
            return list.ToArray();
        }

        public IEnumerator<string> GetEnumerator()
        {
            foreach (var arg in list)
            {
                yield return arg;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static CommandArguments EnvironmentArguments()
        {
            return new CommandArguments(Environment.GetCommandLineArgs());
        }
    }


    public delegate bool ConverterDelegate(ref object value, Type targetType);

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = true)]
    public class CommandArgumentAttribute : Attribute
    {
        public CommandArgumentAttribute()
        {
        }

        public CommandArgumentAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }

        /// <summary>
        /// 开关参数
        /// </summary>
        public bool Toggle { get; set; }

        /// <summary>
        /// 剔除看不到的空白字符 \t \r 等
        /// </summary>
        public bool Trim { get; set; } = true;

    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class EnvironmentVariableAttribute : Attribute
    {
        public EnvironmentVariableAttribute()
        {
        }

        public EnvironmentVariableAttribute(string name)
        {
            this.Name = name;
        }

        public string Name { get; set; }


        public bool Trim { get; set; } = true;

    }
}