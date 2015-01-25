using System;
using NUnit.Framework;
using TakeAsh;

namespace EnumExtensionLib_Test {

    using NewLineCodeHelper = EnumHelper<NewLineCode>;

    public enum NewLineCode {
        [System.ComponentModel.Description("Unix(LF)")]
        Lf = 1,

        [System.ComponentModel.Description("Mac(CR)")]
        Cr = 2,

        [System.ComponentModel.Description("Windows(CR+LF)")]
        CrLf = 4,

        LfCr = 8,
    }

    [TestFixture]
    public class EnumExtensionLib_Test {

        [TestCase]
        public void Values_Test() {
            var expected = new NewLineCode[]{
                NewLineCode.Lf,
                NewLineCode.Cr,
                NewLineCode.CrLf,
                NewLineCode.LfCr,
            };
            CollectionAssert.AreEqual(
                expected,
                NewLineCodeHelper.Values
            );
        }

        [TestCase]
        public void Names_Test() {
            var expected = new string[]{
                "Lf",
                "Cr",
                "CrLf",
                "LfCr",
            };
            CollectionAssert.AreEqual(
                expected,
                NewLineCodeHelper.Names
            );
        }

        [TestCase]
        public void Descriptions_Test() {
            var expected = new string[]{
                "Unix(LF)",
                "Mac(CR)",
                "Windows(CR+LF)",
                "LfCr",
            };
            CollectionAssert.AreEqual(
                expected,
                NewLineCodeHelper.Descriptions
            );
        }

        [TestCase]
        public void ValueDescriptionPairs_Test() {
            var expected = new NewLineCodeHelper.ValueDescriptionPair[]{
                new NewLineCodeHelper.ValueDescriptionPair(NewLineCode.Lf, "Unix(LF)"),
                new NewLineCodeHelper.ValueDescriptionPair(NewLineCode.Cr, "Mac(CR)"),
                new NewLineCodeHelper.ValueDescriptionPair(NewLineCode.CrLf, "Windows(CR+LF)"),
                new NewLineCodeHelper.ValueDescriptionPair(NewLineCode.LfCr, "LfCr"),
            };
            CollectionAssert.AreEqual(
                expected,
                NewLineCodeHelper.ValueDescriptionPairs
            );
        }

        [TestCase("Lf", NewLineCode.Lf)]
        [TestCase("Cr", NewLineCode.Cr)]
        [TestCase("CrLf", NewLineCode.CrLf)]
        [TestCase("LfCr", NewLineCode.LfCr)]
        [TestCase(null, default(NewLineCode))]
        [TestCase("Undefined", default(NewLineCode))]
        public void GetValueFromName_Test(string name, NewLineCode expected) {
            Assert.AreEqual(
                expected,
                NewLineCodeHelper.GetValueFromName(name)
            );
        }

        [TestCase("Unix(LF)", NewLineCode.Lf)]
        [TestCase("Mac(CR)", NewLineCode.Cr)]
        [TestCase("Windows(CR+LF)", NewLineCode.CrLf)]
        [TestCase("LfCr", NewLineCode.LfCr)]
        [TestCase(null, default(NewLineCode))]
        [TestCase("Undefined", default(NewLineCode))]
        public void GetValueFromDescription_Test(string description, NewLineCode expected) {
            Assert.AreEqual(
                expected,
                NewLineCodeHelper.GetValueFromDescription(description)
            );
        }

        [TestCase(1, "Lf", true)]
        [TestCase(2, "Cr", true)]
        [TestCase(4, "CrLf", true)]
        [TestCase(8, "LfCr", true)]
        [TestCase(0, "Undefined", false)]
        [TestCase(3, "Undefined", false)]
        public void IsDefined_Test(int value, string message, bool expected) {
            Assert.AreEqual(
                expected,
                NewLineCodeHelper.IsDefined(value),
                String.Format("{0}: {1}", value, message)
            );
        }

        [TestCase(NewLineCode.Lf, "Unix(LF)")]
        [TestCase(NewLineCode.Cr, "Mac(CR)")]
        [TestCase(NewLineCode.CrLf, "Windows(CR+LF)")]
        [TestCase(NewLineCode.LfCr, "LfCr")]
        [TestCase((NewLineCode)1, "Unix(LF)")]
        [TestCase((NewLineCode)2, "Mac(CR)")]
        [TestCase((NewLineCode)4, "Windows(CR+LF)")]
        [TestCase((NewLineCode)8, "LfCr")]
        [TestCase((NewLineCode)0, "0")]
        [TestCase((NewLineCode)3, "3")]
        public void ToDescription_Test(NewLineCode item, string expected) {
            Assert.AreEqual(
                expected,
                item.ToDescription()
            );
        }
    }
}
