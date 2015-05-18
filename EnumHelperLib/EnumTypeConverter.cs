using System;
using System.Collections.Generic;
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
                var attributes = typeof(TEnum).GetCustomAttributes(typeof(HexDigitAttribute), false);
                HexDigitAttribute hexDigit;
                if (attributes == null ||
                    attributes.Length == 0 ||
                    (hexDigit = attributes[0] as HexDigitAttribute) == null ||
                    hexDigit.Digit <= 0) {
                    return EnumHelper<TEnum>.ToDescription((TEnum)value);
                } else {
                    var flags = new List<string>();
                    var valueEnum = (TEnum)Enum.ToObject(typeof(TEnum), value);
                    foreach (var flag in EnumHelper<TEnum>.Values) {
                        if (HasFlag(valueEnum, flag)) {
                            flags.Add(EnumHelper<TEnum>.ToDescription(flag));
                        }
                    }
                    return Convert.ToInt32(value).ToString(hexDigit.Format) + ": " + String.Join(", ", flags);
                }
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public bool HasFlag(TEnum value, TEnum flag) {
            var valueInt = Convert.ToInt32(value);
            var flagInt = Convert.ToInt32(flag);
            return flagInt != 0 ?
                (valueInt & flagInt) == flagInt :
                valueInt == 0;
        }
    }
}
