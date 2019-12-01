using System;
using System.Linq;

namespace marti_tech_demo.Helper
{
    public static class EnumHelper
    {
        /// <summary>
        /// Enum string verisini döndürür.
        /// </summary>
        /// <typeparam name="TAttribute">Enum tipi</typeparam>
        /// <param name="value">Enum değeri</param>
        /// <returns>Enum string açıklaması</returns>
        public static TAttribute GetAttribute<TAttribute>(this Enum value)
            where TAttribute : Attribute
        {
            var enumType = value.GetType();
            var name = Enum.GetName(enumType, value);
            return enumType.GetField(name).GetCustomAttributes(false).OfType<TAttribute>().SingleOrDefault();
        }
    }
}