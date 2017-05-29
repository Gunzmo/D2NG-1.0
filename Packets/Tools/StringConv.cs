using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace D2NG.Tools
{
    public class StringUtils
    {
        public static string GetTypeValueString(object obj, Type valType, object val, bool includeType)
        {
            if (val == null)
                return null;

            string str;
            if (valType == typeof(System.String))
            {
                if (includeType)
                    return String.Concat("(", valType.Name, ") ", (string)val);
                else
                    return (string)val;
            }
            else if (valType.IsEnum)
            {
                str = Enum.GetName(valType, val);
                if (valType.IsDefined(typeof(System.FlagsAttribute), false))
                {
                    if (str == "None")
                        return null;
                    /* This throws invalid casts !?!?
					if (Enum.GetUnderlyingType(valType) == typeof(UInt64))
					{
						if ((ulong) val == 0)
							return null;
					}
					else
						if ((long) val == 0)
							return null;
					*/
                }
                else if (str == "Unknown" || str == "NotApplicable")
                    return null;
                if (Enum.IsDefined(valType, val))
                {
                    if (includeType)
                        return String.Concat(valType.Name, ".", str);
                    else
                        return str;
                }
                else
                {
                    if (includeType)
                    {
                        //MAYBE: if (valType.IsDefined(typeof(System.FlagsAttribute), false))
                        // enumerate flags as : valType.Name, ".", flagName" | ", valType.Name, ".", flagName2" ...
                        return String.Concat("(", valType.Name, ") ", val.ToString());
                    }
                    else
                        return val.ToString();
                }
            }
            else if (val is System.Collections.IEnumerable)
            {
                System.Collections.IEnumerable valColl = val as System.Collections.IEnumerable;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                if (includeType)
                {
                    sb.Append("(");
                    sb.Append(valType.Name);
                    sb.Append(") ");
                }
                bool weHaveAGuitar = false;
                foreach (object valChild in valColl)
                {
                    if (weHaveAGuitar) sb.Append(", ");
                    else weHaveAGuitar = true;
                    sb.Append(valChild.ToString());
                }
                if (!weHaveAGuitar)
                    return null;

                return sb.ToString();
            }
            else
            {
                System.Reflection.FieldInfo nullField = obj.GetType().GetField("NULL_" + valType.Name,
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);

                //TODO: val.Equals(nullField.GetValue(obj)) !?
                if (nullField != null && val.ToString() == nullField.GetValue(obj).ToString())
                    return null;

                if (includeType)
                    return String.Concat("(", valType.Name, ") ", val.ToString());
                else
                    return val.ToString();
            }
        }

        public static string ToFormatedInfoString(object obj, bool includeType,
            string nameValueSeparator, string itemSeparator)
        {
            return ToFormatedInfoString(obj, includeType, nameValueSeparator, itemSeparator, "{0}{1}{2}");
        }
        public static string ToFormatedInfoString(object obj, bool includeType,
            string nameValueSeparator, string itemSeparator, string itemFormat)
        {
            Type type = obj.GetType();
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            System.Reflection.PropertyInfo[] properties;
            Type valType;
            object val;
            string str;
            bool weHaveAGuitar = false;

            if (null != type.GetField("WRAPPED", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
            {
                properties = type.BaseType.GetProperties(
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.DeclaredOnly
                );

                for (int i = 0; i < properties.Length; i++)
                {
                    valType = properties[i].PropertyType;
                    val = properties[i].GetValue(obj, null);
                    str = StringUtils.GetTypeValueString(obj, valType, val, includeType);
                    if (str == null)
                        continue;

                    sb.AppendFormat(itemFormat,
                        (weHaveAGuitar || i > 0 ? itemSeparator : ""),
                        properties[i].Name + nameValueSeparator,
                        str
                        );
                }
                if (properties.Length > 0)
                    weHaveAGuitar = true;
            }

            System.Reflection.FieldInfo[] fields = type.GetFields(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.DeclaredOnly
                );
            properties = type.GetProperties(
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.DeclaredOnly
                );

            // Fields
            for (int i = 0; i < fields.Length; i++)
            {
                valType = fields[i].FieldType;
                val = fields[i].GetValue(obj);
                str = StringUtils.GetTypeValueString(obj, valType, val, includeType);
                if (str == null)
                    continue;

                sb.AppendFormat(itemFormat,
                    (weHaveAGuitar || i > 0 ? itemSeparator : ""),
                    fields[i].Name + nameValueSeparator,
                    str
                    );
            }
            if (fields.Length > 0)
                weHaveAGuitar = true;

            // Properties
            for (int i = 0; i < properties.Length; i++)
            {
                valType = properties[i].PropertyType;
                val = properties[i].GetValue(obj, null);
                str = StringUtils.GetTypeValueString(obj, valType, val, includeType);
                if (str == null)
                    continue;

                sb.AppendFormat(itemFormat,
                    (weHaveAGuitar || i > 0 ? itemSeparator : ""),
                    properties[i].Name + nameValueSeparator,
                    str
                    );
            }

            return sb.ToString();
        }
    }
}
