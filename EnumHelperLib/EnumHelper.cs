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
    /// <term>C# の enum に関連する小技。 - Qiita</term>
    /// <description>http://qiita.com/hugo-sb/items/38fe86a09e8e0865d471</description>
    /// </remarks>
    static public class EnumHelper<TEnum>
        where TEnum : struct, IConvertible {

        static private Regex regPropertyResources = new Regex(@"\.Properties\.");
        static private Regex regLastResources = new Regex(@"\.resources$");

        public class ValueDescriptionPair :
            IEquatable<ValueDescriptionPair> {
            public TEnum Value { get; private set; }
            public string Description { get; private set; }

            public ValueDescriptionPair(TEnum value, string description) {
                this.Value = value;
                this.Description = description;
            }

            public override string ToString() {
                return Description;
            }

            #region IEquatable members

            public bool Equals(ValueDescriptionPair other) {
                if (other == null) {
                    return false;
                }
                return this.Value.Equals(other.Value) && this.Description == other.Description;
            }

            public override bool Equals(object obj) {
                if (obj == null) {
                    return false;
                }
                return Equals(obj as ValueDescriptionPair);
            }

            public override int GetHashCode() {
                return Value.GetHashCode() ^ Description.GetHashCode();
            }

            static public bool operator ==(ValueDescriptionPair a, ValueDescriptionPair b) {
                if ((object)a == null || (object)b == null) {
                    return Object.Equals(a, b);
                }
                return a.Equals(b);
            }

            static public bool operator !=(ValueDescriptionPair a, ValueDescriptionPair b) {
                return !(a == b);
            }

            #endregion
        }

        static private TEnum[] _values;
        static private string[] _names;
        static private string[] _descriptions;
        static private ValueDescriptionPair[] _valueDescriptionPairs;
        static private ResourceManager _resMan;

        static EnumHelper() {
            _values = (TEnum[])Enum.GetValues(typeof(TEnum));
            _names = Enum.GetNames(typeof(TEnum));
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
            get {
                if (_descriptions != null) {
                    return _descriptions;
                }
                var toDescription = _ResourceManager != null ?
                    (Func<TEnum, string>)((TEnum key) => ToDescription(key)) :
                    (Func<TEnum, string>)((TEnum key) => _ToDescription(key));
                var descriptions = new List<string>();
                foreach (var item in _values) {
                    descriptions.Add(toDescription(item));
                }
                return _descriptions = descriptions.ToArray();
            }
        }

        /// <summary>
        /// List of TEnum value and its description pair
        /// </summary>
        static public ValueDescriptionPair[] ValueDescriptionPairs {
            get {
                if (_valueDescriptionPairs != null) {
                    return _valueDescriptionPairs;
                }
                var toDescription = _ResourceManager != null ?
                    (Func<TEnum, string>)((key) => ToDescription(key)) :
                    (Func<TEnum, string>)((key) => _ToDescription(key));
                var valueDescriptionPairs = new List<ValueDescriptionPair>();
                foreach (var item in _values) {
                    valueDescriptionPairs.Add(new ValueDescriptionPair(item, toDescription(item)));
                }
                return _valueDescriptionPairs = valueDescriptionPairs.ToArray();
            }
        }

        /// <summary>
        /// Initialize cache
        /// </summary>
        /// <attention>
        /// You MUST call Init() after changing CurrentCulture.
        /// </attention>
        static public void Init() {
            _descriptions = null;
            _valueDescriptionPairs = null;
            _resMan = null;
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
            if (_ResourceManager == null) {
                return _ToDescription(en);
            }
            var key = String.Format("{0}_{1}", typeof(TEnum).Name, en.ToString());
            var localizedDescription = _resMan.GetString(key);
            return !String.IsNullOrEmpty(localizedDescription) ?
                localizedDescription :
                _ToDescription(en);
        }

        /// <summary>
        /// Returns Assembly name
        /// </summary>
        /// <returns>Assembly name</returns>
        /// <remarks>
        /// For debug
        /// </remarks>
        static public string GetAssemblyName() {
            var assembly = Assembly.GetEntryAssembly();
            return assembly != null ?
                assembly.GetName().Name :
                null;
        }

        static private ResourceManager _ResourceManager {
            get {
                if (_resMan != null) {
                    return _resMan;
                }
                Assembly assembly;
                string[] resNames;
                if ((assembly = Assembly.GetEntryAssembly()) == null ||
                    (resNames = assembly.GetManifestResourceNames()) == null ||
                    resNames.Length == 0 ||
                    (resNames = resNames.Where(name => regPropertyResources.IsMatch(name)).ToArray()) == null ||
                    resNames.Length == 0) {
                    return null;
                }
                return _resMan = new ResourceManager(regLastResources.Replace(resNames[0], ""), assembly);
            }
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
    /// <term>C# 3.0 : using extension methods for enum ToString</term>
    /// <description>http://blogs.msdn.com/b/abhinaba/archive/2005/10/21/483337.aspx</description>
    /// </item>
    /// <item>
    /// <term>[C#] 何故 enum に拘りたくなるのか？ - Moonmile Solutions Blog</term>
    /// <description>http://www.moonmile.net/blog/archives/3666</description>
    /// </item>
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
            if ((assembly = Assembly.GetEntryAssembly()) == null ||
                (resNames = assembly.GetManifestResourceNames()) == null ||
                resNames.Length == 0 ||
                (resNames = resNames.Where(name => regPropertyResources.IsMatch(name)).ToArray()) == null ||
                resNames.Length == 0 ||
                (resourceManager = new ResourceManager(regLastResources.Replace(resNames[0], ""), assembly)) == null) {
                return en._ToDescription();
            }
            var key = String.Format("{0}_{1}", en.GetType().Name, en.ToString());
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
            var assembly = Assembly.GetEntryAssembly();
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
