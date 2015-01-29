using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TakeAsh {

    /// <summary>
    /// Attaches Dictionary&lt;string, string&gt; to an object.
    /// </summary>
    public class ExtraPropertiesAttribute : Attribute {

        static Regex regKeyValuePair =
            new Regex(@"(?<key>[A-Za-z_][A-Za-z0-9_]*)\s*:\s*(?<quote>['""])(?<value>[^'""]*)\k<quote>");
        static Regex regHex =
            new Regex(@"\\[uU](?<uni>[0-9A-Fa-f]{4})|\\[xX](?<hex>[0-9A-Fa-f]{2}([0-9A-Fa-f]{2})?)");

        private Dictionary<string, string> _extraProperties;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="keyValuePairs">Key Value Pairs</param>
        /// <remarks>
        /// <list type="table">
        /// <item><term>Key</term><description>[A-Za-z_][A-Za-z0-9_]*</description></item>
        /// <item><term>Value</term><description>[^'&quot;]*</description></item>
        /// <item><term>Quote</term><description>['&quot;]</description></item>
        /// <item><term>KeyValuePair</term><description>Key : Quote Value Quote</description></item>
        /// <item><term>KeyValuePairs</term><description>(KeyValuePair)*</description></item>
        /// </list>
        /// <list type="bullet">
        /// <item>Quote can't be ommited.</item>
        /// <item>Quote can't be written in Value. Use \\x22(&quot;), \\x27('), \\u0022(&quot;), \\u0027(') instead of Quote.</item>
        /// </list>
        /// </remarks>
        public ExtraPropertiesAttribute(string keyValuePairs) {
            this._extraProperties = new Dictionary<string, string>();
            foreach (Match kvp in regKeyValuePair.Matches(keyValuePairs)) {
                var value = kvp.Groups["value"].Value;
                if (!String.IsNullOrEmpty(value)) {
                    value = regHex.Replace(
                        value,
                        new MatchEvaluator(
                            hex => !String.IsNullOrEmpty(hex.Groups["uni"].Value) ?
                                ((char)Convert.ToInt32(hex.Groups["uni"].Value, 16)).ToString() :
                                ((char)Convert.ToInt32(hex.Groups["hex"].Value, 16)).ToString()
                        )
                    );
                }
                _extraProperties[kvp.Groups["key"].Value] = value;
            }
        }

        public string this[string key] {
            get {
                return _extraProperties != null && _extraProperties.ContainsKey(key) ?
                    _extraProperties[key] :
                    null;
            }
            set {
                if (_extraProperties == null) {
                    _extraProperties = new Dictionary<string, string>();
                }
                _extraProperties[key] = value;
            }
        }
    }
}
