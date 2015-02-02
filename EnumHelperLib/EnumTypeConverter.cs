using System;
using System.ComponentModel;

namespace TakeAsh {

    /// <summary>
    /// Provides ToDescription() as string converter.
    /// </summary>
    /// <typeparam name="TEnum">Enum type</typeparam>
    /// <remarks>
    /// <list type="bullet">
    /// <item>[c# - Data bind enum properties to grid and display description - Stack Overflow](http://stackoverflow.com/questions/1540103/)</item>
    /// </list>
    /// </remarks>
    public class EnumTypeConverter<TEnum> : EnumConverter
        where TEnum : struct, IConvertible {

        public EnumTypeConverter() : base(typeof(TEnum)) { }

        public override object ConvertTo(
            ITypeDescriptorContext context,
            System.Globalization.CultureInfo culture,
            object value,
            Type destinationType
        ) {
            if (destinationType == typeof(string)) {
                return EnumHelper<TEnum>.ToDescription((TEnum)value);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
