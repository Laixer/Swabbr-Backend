using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Swabbr.Core.Extensions
{

    /// <summary>
    /// Extension methods for <see cref="Enum"/> items.
    /// </summary>
    public static class EnumExtensions
    {

        /// <summary>
        /// Extracts the <see cref="EnumMemberAttribute"/> value from a given 
        /// <see cref="Enum"/> item.
        /// </summary>
        /// <typeparam name="TEnum"><see cref="Enum"/></typeparam>
        /// <param name="enumerationValue"><see cref="Enum"/></param>
        /// <returns><see cref="EnumMemberAttribute"/> string value</returns>
        public static string GetEnumMemberAttribute<TEnum>(this TEnum enumerationValue)
           where TEnum : struct
        {
            Type type = enumerationValue.GetType();
            if (!type.IsEnum)
            {
                throw new ArgumentException("TEnum must be of Enum type");
            }

            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(EnumMemberAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    return ((EnumMemberAttribute)attrs[0]).Value;
                }
            }
            return enumerationValue.ToString();
        }

    }

}
