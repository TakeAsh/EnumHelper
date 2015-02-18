using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TakeAsh {

    /// <summary>
    /// The dictionary converting an Enum to another Enum
    /// </summary>
    /// <typeparam name="TKey">Key enum type</typeparam>
    /// <typeparam name="TValue">Value enum type</typeparam>
    public class EnumEnumDictionary<TKey, TValue>
        : Dictionary<TKey, TValue>
        where TKey : struct, IConvertible
        where TValue : struct, IConvertible {

        public EnumEnumDictionary() : base() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="autoInitialize">If true, initialize with TKey/TValue pairs</param>
        /// <remarks>
        /// <list type="bullet">
        /// <item>If autoInitialize is true but TKey length is not equal to TValue length, ArgumentException is thrown.</item>
        /// </list>
        /// </remarks>
        public EnumEnumDictionary(bool autoInitialize)
            : base() {
            if (autoInitialize) {
                var keys = EnumHelper<TKey>.Values;
                var values = EnumHelper<TValue>.Values;
                if (keys.Length == values.Length) {
                    for (var i = 0; i < keys.Length; ++i) {
                        this.Add(keys[i], values[i]);
                    }
                } else {
                    throw new ArgumentException("TKey length is not equal to TValue length");
                }
            }
        }

        public TValue this[TKey? key] {
            get {
                var key1 = key ?? default(TKey);
                return base.ContainsKey(key1) ?
                    base[key1] :
                    default(TValue);
            }
        }
    }
}
