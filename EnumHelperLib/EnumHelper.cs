using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TakeAsh {

    /// <summary>
    /// Enum Helper Class
    /// </summary>
    /// <typeparam name="TEnum">Enum type</typeparam>
    /// <remarks>
    /// C# の enum に関連する小技。 - Qiita
    /// http://qiita.com/hugo-sb/items/38fe86a09e8e0865d471
    /// </remarks>
    static public class EnumHelper<TEnum>
        where TEnum : struct, IConvertible {

        static private TEnum[] _values;
        static private string[] _names;
        static private string[] _descriptions;

        static EnumHelper() {
            _values = (TEnum[])Enum.GetValues(typeof(TEnum));
            _names = Enum.GetNames(typeof(TEnum));

            var descriptions = new List<string>();
            foreach (var item in _values) {
                descriptions.Add(ToDescription(item));
            }
            _descriptions = descriptions.ToArray();
        }

        static public TEnum[] Values {
            get { return _values; }
        }

        static public string[] Names {
            get { return _names; }
        }

        static public string[] Descriptions {
            get { return _descriptions; }
        }

        static public TEnum GetValueFromName(string name) {
            var index = Array.IndexOf(_names, name);
            return index >= 0 ?
                _values[index] :
                default(TEnum);
        }

        static public TEnum GetValueFromDescription(string name) {
            var index = Array.IndexOf(_descriptions, name);
            return index >= 0 ?
                _values[index] :
                default(TEnum);
        }

        static public bool IsDefined(int value) {
            return Enum.IsDefined(typeof(TEnum), value);
        }

        static private string ToDescription(TEnum en) {
            var memInfos = typeof(TEnum).GetMember(en.ToString());
            if (memInfos == null || memInfos.Length == 0) {
                return en.ToString();
            }
            var attrs = memInfos[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attrs == null || attrs.Length == 0) {
                return en.ToString();
            }
            return ((DescriptionAttribute)attrs[0]).Description;
        }
    }

    /// <summary>
    /// Enum Extension Methods
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>
    /// C# 3.0 : using extension methods for enum ToString
    /// http://blogs.msdn.com/b/abhinaba/archive/2005/10/21/483337.aspx
    /// </item>
    /// <item>
    /// [C#] 何故 enum に拘りたくなるのか？ - Moonmile Solutions Blog
    /// http://www.moonmile.net/blog/archives/3666
    /// </item>
    /// </list>
    /// </remarks>
    static public class EnumExtensionMethods {

        static public string ToDescription(this Enum en) {
            var memInfos = en.GetType().GetMember(en.ToString());
            if (memInfos == null || memInfos.Length == 0) {
                return en.ToString();
            }
            var attrs = memInfos[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attrs == null || attrs.Length == 0) {
                return en.ToString();
            }
            return ((DescriptionAttribute)attrs[0]).Description;
        }
    }
}
