using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;

namespace TakeAsh {

    /// <summary>
    /// Enum Helper Class
    /// </summary>
    /// <typeparam name="TEnum">Enum type</typeparam>
    /// <remarks>
    /// <list type="bullet">
    /// <item>[C# の enum に関連する小技。 - Qiita](http://qiita.com/hugo-sb/items/38fe86a09e8e0865d471)</item>
    /// </list>
    /// </remarks>
    static public class EnumHelper<TEnum>
        where TEnum : struct, IConvertible {

        static private Regex regPropertyResources = new Regex(@"\.Properties\.");
        static private Regex regLastResources = new Regex(@"\.resources$");

        static private TEnum[] _values;
        static private string[] _names;
        static private string[] _descriptions;
        static private KeyValuePair<TEnum, string>[] _valueDescriptionPairs;
        static private Dictionary<TEnum, string> _valueDescriptionDictionary;
        static private ResourceManager _resMan;

        static EnumHelper() {
            _values = (TEnum[])Enum.GetValues(typeof(TEnum));
            _names = Enum.GetNames(typeof(TEnum));
            Init();
        }

        /// <summary>
        /// List of TEnum values
        /// </summary>
        static public TEnum[] Values {
            get { return _values; }
        }

        /// <summary>
        /// List of TEnum item names
        /// </summary>
        static public string[] Names {
            get { return _names; }
        }

        /// <summary>
        /// List of TEnum item descriptions
        /// </summary>
        /// <remarks>
        /// TEnum item description come from:
        /// <list type="number">
        /// <item>localized resource (TEnumType_ItemName)</item>
        /// <item>default resource (TEnumType_ItemName)</item>
        /// <item>Description attribute (System.ComponentModel)</item>
        /// <item>TEnum item name</item>
        /// </list>
        /// </remarks>
        static public string[] Descriptions {
            get { return _descriptions; }
        }

        /// <summary>
        /// List of TEnum value and its description pair
        /// </summary>
        static public KeyValuePair<TEnum, string>[] ValueDescriptionPairs {
            get { return _valueDescriptionPairs; }
        }

        /// <summary>
        /// Returns the dictionary of TEnum item and Description.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>ValueDescriptionDictionary can be used as like as ToDescription().</item>
        /// <item>
        /// ValueDescriptionDictionary is different from ToDescription(), when TEnum item doesn't exist in TEnum.
        /// <list type="table">
        /// <item><term>ValueDescriptionDictionary</term><description>throw KeyNotFoundException.</description></item>
        /// <item><term>ToDescription()</term><description>returns stringified item.</description></item>
        /// </list>
        /// </item>
        /// </list>
        /// </remarks>
        static public Dictionary<TEnum, string> ValueDescriptionDictionary {
            get { return _valueDescriptionDictionary; }
        }

        /// <summary>
        /// Initialize cache
        /// </summary>
        /// <remarks>
        /// You MUST call Init() after changing CurrentCulture.
        /// </remarks>
        static public void Init() {
            var toDescription = (_resMan = _GetResourceManager()) != null ?
                (Func<TEnum, string>)((key) => ToDescription(key)) :
                (Func<TEnum, string>)((key) => _ToDescription(key));
            var descriptions = new List<string>();
            var valueDescriptionPairs = new List<KeyValuePair<TEnum, string>>();
            foreach (var item in _values) {
                var description = toDescription(item);
                descriptions.Add(description);
                valueDescriptionPairs.Add(new KeyValuePair<TEnum, string>(item, description));
            }
            _descriptions = descriptions.ToArray();
            _valueDescriptionPairs = valueDescriptionPairs.ToArray();
            _valueDescriptionDictionary = valueDescriptionPairs.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        /// <summary>
        /// Returns TEnum value from name
        /// </summary>
        /// <param name="name">TEnum name</param>
        /// <returns>
        /// <list type="table">
        /// <item><term>TEnum value</term><description>when it is found.</description></item>
        /// <item><term>TEnum default value</term><description>when it is not found.</description></item>
        /// </list>
        /// </returns>
        static public TEnum GetValueFromName(string name) {
            var index = Array.IndexOf(_names, name);
            return index >= 0 ?
                _values[index] :
                default(TEnum);
        }

        /// <summary>
        /// Returns TEnum value from name
        /// </summary>
        /// <param name="name">TEnum name</param>
        /// <returns>
        /// <list type="table">
        /// <item><term>TEnum value</term><description>when it is found.</description></item>
        /// <item><term>null</term><description>when it is null, or blank, or not found.</description></item>
        /// </list>
        /// </returns>
        static public Nullable<TEnum> GetValueNullableFromName(string name) {
            var index = Array.IndexOf(_names, name);
            if (index >= 0) {
                return _values[index];
            } else {
                return null;
            }
        }

        /// <summary>
        /// Returns TEnum value from description
        /// </summary>
        /// <param name="description">TEnum description</param>
        /// <returns>
        /// <list type="table">
        /// <item><term>TEnum value</term><description>when it is found.</description></item>
        /// <item><term>TEnum default value</term><description>when it is not found.</description></item>
        /// </list>
        /// </returns>
        static public TEnum GetValueFromDescription(string description) {
            var index = Array.IndexOf(Descriptions, description);
            return index >= 0 ?
                _values[index] :
                default(TEnum);
        }

        /// <summary>
        /// Returns TEnum value from description
        /// </summary>
        /// <param name="description">TEnum description</param>
        /// <returns>
        /// <list type="table">
        /// <item><term>TEnum value</term><description>when it is found.</description></item>
        /// <item><term>null</term><description>when it is null, or blank, or not found.</description></item>
        /// </list>
        /// </returns>
        static public Nullable<TEnum> GetValueNullableFromDescription(string description) {
            var index = Array.IndexOf(Descriptions, description);
            if (index >= 0) {
                return _values[index];
            } else {
                return null;
            }
        }

        /// <summary>
        /// Returns the ExtraProperty value of a specific TEnum item
        /// </summary>
        /// <param name="en">TEnum item</param>
        /// <param name="key">ExtraProperty key</param>
        /// <returns>ExtraProperty value</returns>
        static public string GetExtraProperty(TEnum en, string key) {
            var extraPropertiesAttribute = AttributeHelper<ExtraPropertiesAttribute>.GetAttribute(en);
            return extraPropertiesAttribute != null ?
                extraPropertiesAttribute[key] :
                null;
        }

        /// <summary>
        /// Returns the ExtraProperty values for all TEnum items
        /// </summary>
        /// <param name="key">ExtraProperty key</param>
        /// <returns>The ExtraProperty values</returns>
        static public string[] GetAllExtraProperties(string key) {
            var properties = new List<string>();
            foreach (var item in _values) {
                properties.Add(GetExtraProperty(item, key));
            }
            return properties.ToArray();
        }

        /// <summary>
        /// Returns an indication whether a constant exists in TEnum.
        /// </summary>
        /// <param name="value">The value of a constant in TEnum.</param>
        /// <returns>
        /// <list type="table">
        /// <item><term>true</term><description>if a constant in TEnum has a value equal to value</description></item>
        /// <item><term>false</term><description>if a constant in TEnum doesn't have a value equal to value</description></item>
        /// </list>
        /// </returns>
        static public bool IsDefined(object value) {
            return value != null ?
                Enum.IsDefined(typeof(TEnum), value) :
                false;
        }

        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
        /// </summary>
        /// <param name="value">The string representation of the enumeration name or underlying value to convert.</param>
        /// <param name="result">
        /// When this method returns, result contains an object of type TEnum whose value is represented by value if the parse operation succeeds.
        /// If the parse operation fails, result contains the default value of the underlying type of TEnum.
        /// Note that this value need not be a member of the TEnum enumeration.
        /// This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// <list type="table">
        /// <item><term>true</term><description>if the value parameter was converted successfully</description></item>
        /// <item><term>false</term><description>otherwise</description></item>
        /// </list>
        /// </returns>
        static public bool TryParse(string value, out TEnum result) {
            return Enum.TryParse<TEnum>(value, out result);
        }

        /// <summary>
        /// Returns TEnum item description
        /// </summary>
        /// <param name="en">TEnum value</param>
        /// <returns>TEnum item description</returns>
        /// <remarks>
        /// TEnum item description come from:
        /// <list type="number">
        /// <item>localized resource (TEnumType_ItemName)</item>
        /// <item>default resource (TEnumType_ItemName)</item>
        /// <item>Description attribute (System.ComponentModel)</item>
        /// <item>TEnum item name</item>
        /// </list>
        /// </remarks>
        static public string ToDescription(TEnum en) {
            if (_resMan == null) {
                return _ToDescription(en);
            }
            var enumType = typeof(TEnum);
            var key = (enumType.ReflectedType != null ? enumType.ReflectedType.Name + "_" : "") +
                enumType.Name + "_" +
                en.ToString();
            var localizedDescription = _resMan.GetString(key);
            return !String.IsNullOrEmpty(localizedDescription) ?
                localizedDescription :
                _ToDescription(en);
        }

        /// <summary>
        /// Returns TEnum value casted from obj
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns>
        /// <list type="table">
        /// <item><term>TEnum value</term><description>when obj is not null, and castable to TEnum</description></item>
        /// <item><term>default(TEnum)</term><description>when obj is null</description></item>
        /// </list>
        /// </returns>
        static public TEnum Cast(object obj) {
            return (TEnum?)obj ?? default(TEnum);
        }

        /// <summary>
        /// Returns Assembly name
        /// </summary>
        /// <returns>Assembly name</returns>
        /// <remarks>
        /// For debug
        /// </remarks>
        static public string GetAssemblyName() {
            var assembly = Assembly.GetAssembly(typeof(TEnum));
            return assembly != null ?
                assembly.GetName().Name :
                null;
        }

        static private ResourceManager _GetResourceManager() {
            Assembly assembly;
            string[] resNames;
            if ((assembly = Assembly.GetAssembly(typeof(TEnum))) == null ||
                (resNames = assembly.GetManifestResourceNames()) == null ||
                resNames.Length == 0 ||
                (resNames = resNames.Where(name => regPropertyResources.IsMatch(name)).ToArray()) == null ||
                resNames.Length == 0) {
                return null;
            }
            return new ResourceManager(regLastResources.Replace(resNames[0], ""), assembly);
        }

        /// <summary>
        /// Returns TEnum item description
        /// </summary>
        /// <param name="en">TEnum value</param>
        /// <returns>TEnum item description</returns>
        /// <remarks>
        /// TEnum item description come from:
        /// <list type="number">
        /// <item>Description attribute (System.ComponentModel)</item>
        /// <item>TEnum item name</item>
        /// </list>
        /// </remarks>
        static private string _ToDescription(TEnum en) {
            var descriptionAttribute = AttributeHelper<DescriptionAttribute>.GetAttribute(en);
            return descriptionAttribute != null ?
                descriptionAttribute.Description :
                en.ToString();
        }
    }
}
