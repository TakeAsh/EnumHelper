using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;

namespace TakeAsh {

    /// <summary>
    /// Enum Extension Methods
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>[C# 3.0 : using extension methods for enum ToString - I know the answer (it's 42) - MSDN Blogs](http://blogs.msdn.com/b/abhinaba/archive/2005/10/21/483337.aspx)</item>
    /// <item>[[C#] 何故 enum に拘りたくなるのか？ | Moonmile Solutions Blog](http://www.moonmile.net/blog/archives/3666)</item>
    /// </list>
    /// </remarks>
    static public class EnumExtensionMethods {

        static private Regex regPropertyResources = new Regex(@"\.Properties\.");
        static private Regex regLastResources = new Regex(@"\.resources$");

        /// <summary>
        /// Returns Enum item description
        /// </summary>
        /// <param name="en">Enum item</param>
        /// <returns>Enum item description</returns>
        /// <remarks>
        /// Enum item description come from:
        /// <list type="number">
        /// <item>localized resource (EnumType_ItemName)</item>
        /// <item>default resource (EnumType_ItemName)</item>
        /// <item>Description attribute (System.ComponentModel)</item>
        /// <item>Enum item name</item>
        /// </list>
        /// </remarks>
        static public string ToDescription(this Enum en) {
            Assembly assembly;
            string[] resNames;
            ResourceManager resourceManager;
            if ((assembly = Assembly.GetAssembly(en.GetType())) == null ||
                (resNames = assembly.GetManifestResourceNames()) == null ||
                resNames.Length == 0 ||
                (resNames = resNames.Where(name => regPropertyResources.IsMatch(name)).ToArray()) == null ||
                resNames.Length == 0 ||
                (resourceManager = new ResourceManager(regLastResources.Replace(resNames[0], ""), assembly)) == null) {
                return en._ToDescription();
            }
            var enumType = en.GetType();
            var key = (enumType.ReflectedType != null ? enumType.ReflectedType.Name + "_" : "") +
                enumType.Name + "_" +
                en.ToString();
            var localizedDescription = resourceManager.GetString(key);
            return !String.IsNullOrEmpty(localizedDescription) ?
                localizedDescription :
                en._ToDescription();
        }

        /// <summary>
        /// Gets Assembly name
        /// </summary>
        /// <param name="en">Enum item</param>
        /// <returns>Assembly name</returns>
        /// <remarks>
        /// For debug
        /// </remarks>
        static public string GetAssemblyName(this Enum en) {
            var assembly = Assembly.GetAssembly(en.GetType());
            return assembly != null ?
                assembly.GetName().Name :
                null;
        }

        /// <summary>
        /// Returns Enum item description
        /// </summary>
        /// <param name="en">Enum item</param>
        /// <returns>Enum item description</returns>
        /// <remarks>
        /// Enum item description come from:
        /// <list type="number">
        /// <item>Description attribute (System.ComponentModel)</item>
        /// <item>Enum item name</item>
        /// </list>
        /// </remarks>
        static private string _ToDescription(this Enum en) {
            var descriptionAttribute = AttributeHelper<DescriptionAttribute>.GetAttribute(en);
            return descriptionAttribute != null ?
                descriptionAttribute.Description :
                en.ToString();
        }

        /// <summary>
        /// Returns extra property from ExtraPropertiesAttribute
        /// </summary>
        /// <param name="en">Enum item</param>
        /// <param name="key">key of ExtraPropertiesAttribute</param>
        /// <returns>value of ExtraPropertiesAttribute</returns>
        static public string GetExtraProperty(this Enum en, string key) {
            var extraPropertiesAttribute = AttributeHelper<ExtraPropertiesAttribute>.GetAttribute(en);
            return extraPropertiesAttribute != null ?
                extraPropertiesAttribute[key] :
                null;
        }

        static public string ToHexWithFlag(this Enum en) {
            var enumType = en.GetType();
            var attributes = enumType.GetCustomAttributes(typeof(HexDigitAttribute), false);
            HexDigitAttribute hexDigit;
            if (attributes == null ||
                attributes.Length == 0 ||
                (hexDigit = attributes[0] as HexDigitAttribute) == null ||
                hexDigit.Digit <= 0) {
                return en.ToDescription();
            } else {
                var flags = new List<string>();
                var value = (Enum)Enum.ToObject(enumType, en);
                foreach (Enum flag in Enum.GetValues(enumType)) {
                    if (HasFlag(value, flag)) {
                        flags.Add(flag.ToDescription());
                    }
                }
                return Convert.ToInt32(value).ToString(hexDigit.Format) + ": " + String.Join(", ", flags);
            }
        }

        static public bool HasFlag(Enum value, Enum flag) {
            var valueInt = Convert.ToInt32(value);
            var flagInt = Convert.ToInt32(flag);
            return flagInt != 0 ?
                (valueInt & flagInt) == flagInt :
                valueInt == 0;
        }

        static public TEnum ToEnum<TEnum>(this int value)
            where TEnum : struct, IConvertible {

            return (TEnum)Enum.ToObject(typeof(TEnum), value);
        }

        static public TEnum ToFlag<TEnum>(this int value)
            where TEnum : struct, IConvertible {

            return (TEnum)Enum.ToObject(typeof(TEnum), value);
        }
    }
}
