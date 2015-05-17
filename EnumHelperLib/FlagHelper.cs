using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TakeAsh {

    public class FlagHelper<TFlag>
        where TFlag : struct, IConvertible {

        public const int DefaultDigit = 4;

        public FlagHelper() { }

        public FlagHelper(int digit = DefaultDigit) {
            Digit = digit;
        }

        public FlagHelper(TFlag value, int digit = DefaultDigit) {
            Digit = digit;
            Value = value;
        }

        public int Digit { get; private set; }

        public TFlag Value { get; set; }

        public override string ToString() {
            var ret = Convert.ToInt32(Value).ToString("X" + Digit);
            var flags = new List<string>();
            foreach (TFlag flag in Enum.GetValues(typeof(TFlag))) {
                if (HasFlag(flag)) {
                    flags.Add(EnumHelper<TFlag>.ToDescription(flag));
                }
            }
            return ret + ": " + String.Join(", ", flags);
        }

        public bool HasFlag(TFlag flag) {
            var flagInt = Convert.ToInt32(flag);
            var valueInt = Convert.ToInt32(Value);
            return flagInt != 0 ?
                (valueInt & flagInt) == flagInt :
                valueInt == 0;
        }
    }

    public static class FlagHelperExtensionMethods {

        public static FlagHelper<TFlag> ToFlag<TFlag>(
            this TFlag value,
            int digit = FlagHelper<TFlag>.DefaultDigit
        ) where TFlag : struct, IConvertible {

            return new FlagHelper<TFlag>(value, digit);
        }

        public static FlagHelper<TFlag> ToFlag<TFlag>(
            this int value,
            int digit = FlagHelper<TFlag>.DefaultDigit
        ) where TFlag : struct, IConvertible {

            return new FlagHelper<TFlag>((TFlag)Enum.ToObject(typeof(TFlag), value), digit);
        }
    }
}
