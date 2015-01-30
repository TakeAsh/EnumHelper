using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using F2DUnitTests;
using NUnit.Framework;
using TakeAsh;

namespace EnumExtensionLib_Test {

    using EnumDescriptionDictionary = Dictionary<NewLineCodes, string>;
    using NewLineCodeHelper = EnumHelper<NewLineCodes>;

    public enum NewLineCodes {
        [ExtraProperties("Entity:'\n', Escaped:'\\x22\\u0027\t'")]
        Lf = 1,

        [ExtraProperties("Entity : \"\r\"Escaped : '\\x0027\\x0022'")]
        [System.ComponentModel.Description("[A] Mac(CR)")]
        Cr = 2,

        [ExtraProperties("Entity:\t'\r\n';;;Escaped:\t\"\\x3042\"")] // U+3042 あ
        [System.ComponentModel.Description("[A] Windows(CR+LF)")]
        CrLf = 4,

        [ExtraProperties("Entity:\n\t'\n\r'\nEscaped:\n\t'\\uD842\\uDFB7'")] // U+00020BB7 𠮷
        LfCr = 8,
    }

    [TestFixture]
    public class EnumExtensionLib_Test {

        public Dictionary<string, EnumDescriptionDictionary> expectedDescriptions =
            new Dictionary<string, EnumDescriptionDictionary>() {
            {
                "en-US",
                new EnumDescriptionDictionary(){
                    {NewLineCodes.Lf, "[R_en] Unix(LF)"},
                    {NewLineCodes.Cr, "[R_en] Mac(CR)"},
                    {NewLineCodes.CrLf, "[A] Windows(CR+LF)"},
                    {NewLineCodes.LfCr, "LfCr"},
                }
            },
            {
                "ja-JP",
                new EnumDescriptionDictionary(){
                    {NewLineCodes.Lf, "[R_ja] ユニックス(LF)"},
                    {NewLineCodes.Cr, "[R_en] Mac(CR)"},
                    {NewLineCodes.CrLf, "[R_ja] ウィンドウズ(CR+LF)"},
                    {NewLineCodes.LfCr, "LfCr"},
                }
            },
        };

        [SetUp]
        public void setup() {
            AssemblyUtilities.SetEntryAssembly();
        }

        private void SetCurrentCulture(string cultureName) {
            var culture = new CultureInfo(cultureName);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            NewLineCodeHelper.Init();
        }

        [TestCase]
        public void Assembly_Test() {
            var expected = "EnumHelperLib_Test";
            Assert.AreEqual(expected, NewLineCodeHelper.GetAssemblyName());
            Assert.AreEqual(expected, NewLineCodes.Lf.GetAssemblyName());
        }

        [TestCase]
        public void Values_Test() {
            var expected = new NewLineCodes[]{
                NewLineCodes.Lf,
                NewLineCodes.Cr,
                NewLineCodes.CrLf,
                NewLineCodes.LfCr,
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
            foreach (var culture in expectedDescriptions.Keys) {
                var expected = new List<string>(expectedDescriptions[culture].Count);
                SetCurrentCulture(culture);
                foreach (var key in NewLineCodeHelper.Values) {
                    expected.Add(expectedDescriptions[culture][key]);
                }
                CollectionAssert.AreEqual(
                    expected.ToArray(),
                    NewLineCodeHelper.Descriptions,
                    culture
                );
            }
        }

        [TestCase]
        public void ValueDescriptionPairs_Test() {
            foreach (var culture in expectedDescriptions.Keys) {
                var expected = new KeyValuePair<NewLineCodes, string>[expectedDescriptions[culture].Count];
                SetCurrentCulture(culture);
                for (var i = 0; i < NewLineCodeHelper.Values.Length; ++i) {
                    var key = NewLineCodeHelper.Values[i];
                    expected[i] = new KeyValuePair<NewLineCodes, string>(key, expectedDescriptions[culture][key]);
                }
                CollectionAssert.AreEqual(
                    expected,
                    NewLineCodeHelper.ValueDescriptionPairs,
                    culture
                );
            }
        }

        [TestCase("Lf", NewLineCodes.Lf)]
        [TestCase("Cr", NewLineCodes.Cr)]
        [TestCase("CrLf", NewLineCodes.CrLf)]
        [TestCase("LfCr", NewLineCodes.LfCr)]
        [TestCase(null, default(NewLineCodes))]
        [TestCase("", default(NewLineCodes))]
        [TestCase("Undefined", default(NewLineCodes))]
        public void GetValueFromName_Test(string name, NewLineCodes expected) {
            Assert.AreEqual(
                expected,
                NewLineCodeHelper.GetValueFromName(name)
            );
        }

        [TestCase("Lf", NewLineCodes.Lf)]
        [TestCase("Cr", NewLineCodes.Cr)]
        [TestCase("CrLf", NewLineCodes.CrLf)]
        [TestCase("LfCr", NewLineCodes.LfCr)]
        [TestCase(null, null)]
        [TestCase("", null)]
        [TestCase("Undefined", null)]
        public void GetValueNullableFromName_Test(string name, NewLineCodes? expected) {
            var actual = NewLineCodeHelper.GetValueNullableFromName(name);
            if (expected != null) {
                Assert.AreEqual(expected, actual);
            } else {
                Assert.Null(actual);
            }
        }

        [TestCase("en-US", "[R_en] Unix(LF)", NewLineCodes.Lf)]
        [TestCase("en-US", "[R_en] Mac(CR)", NewLineCodes.Cr)]
        [TestCase("en-US", "[A] Windows(CR+LF)", NewLineCodes.CrLf)]
        [TestCase("en-US", "LfCr", NewLineCodes.LfCr)]
        [TestCase("en-US", null, default(NewLineCodes))]
        [TestCase("en-US", "", default(NewLineCodes))]
        [TestCase("en-US", "Undefined", default(NewLineCodes))]
        [TestCase("ja-JP", "[R_ja] ユニックス(LF)", NewLineCodes.Lf)]
        [TestCase("ja-JP", "[R_en] Mac(CR)", NewLineCodes.Cr)]
        [TestCase("ja-JP", "[R_ja] ウィンドウズ(CR+LF)", NewLineCodes.CrLf)]
        [TestCase("ja-JP", "LfCr", NewLineCodes.LfCr)]
        [TestCase("ja-JP", null, default(NewLineCodes))]
        [TestCase("ja-JP", "", default(NewLineCodes))]
        [TestCase("ja-JP", "Undefined", default(NewLineCodes))]
        public void GetValueFromDescription_Test(string culture, string description, NewLineCodes expected) {
            if (!String.IsNullOrEmpty(culture)) {
                SetCurrentCulture(culture);
            }
            Assert.AreEqual(
                expected,
                NewLineCodeHelper.GetValueFromDescription(description)
            );
        }

        [TestCase("Entity", "\n//\r//\r\n//\n\r")]
        [TestCase("Escaped", "\"\'\t//\'\"//あ//\uD842\uDFB7")]
        public void GetExtraProperties_Test(string key, string properties) {
            var expected = properties.Split(new string[] { "//" }, StringSplitOptions.RemoveEmptyEntries);
            CollectionAssert.AreEqual(
                expected,
                NewLineCodeHelper.GetAllExtraProperties(key)
            );
        }

        [TestCase(1, "Lf", true)]
        [TestCase(2, "Cr", true)]
        [TestCase(4, "CrLf", true)]
        [TestCase(8, "LfCr", true)]
        [TestCase(0, "Undefined", false)]
        [TestCase(3, "Undefined", false)]
        [TestCase("Lf", "Lf", true)]
        [TestCase("Cr", "Cr", true)]
        [TestCase("CrLf", "CrLf", true)]
        [TestCase("LfCr", "LfCr", true)]
        [TestCase(null, "Null", false)]
        [TestCase("", "Blank", false)]
        [TestCase("Undefined", "Undefined", false)]
        public void IsDefined_Test(object value, string message, bool expected) {
            Assert.AreEqual(
                expected,
                NewLineCodeHelper.IsDefined(value),
                String.Format("{0}: {1}", value, message)
            );
        }

        [TestCase("Lf", true, NewLineCodes.Lf)]
        [TestCase("Cr", true, NewLineCodes.Cr)]
        [TestCase("CrLf", true, NewLineCodes.CrLf)]
        [TestCase("LfCr", true, NewLineCodes.LfCr)]
        [TestCase("1", true, NewLineCodes.Lf)]
        [TestCase("2", true, NewLineCodes.Cr)]
        [TestCase("4", true, NewLineCodes.CrLf)]
        [TestCase("8", true, NewLineCodes.LfCr)]
        [TestCase(null, false, default(NewLineCodes))]
        [TestCase("", false, default(NewLineCodes))]
        [TestCase("Undefined", false, default(NewLineCodes))]
        public void TryParse_Test(string value, bool expectedReturn, NewLineCodes expectedResult) {
            NewLineCodes actualResult;
            Assert.AreEqual(expectedReturn, NewLineCodeHelper.TryParse(value, out actualResult));
            if (expectedReturn) {
                Assert.AreEqual(expectedResult, actualResult);
            }
        }

        public struct ToDescription_TestCase {
            public string Culture { get; set; }
            public NewLineCodes Item { get; set; }
            public bool IsValid { get; set; }
            public string Expected { get; set; }

            public ToDescription_TestCase(
                string culture,
                NewLineCodes item,
                bool isValid,
                string expected
            ) : this() {
                this.Culture = culture;
                this.Item = item;
                this.IsValid = isValid;
                this.Expected = expected;
            }
        }

        public static ToDescription_TestCase[] ToDescription_TestCases = new ToDescription_TestCase[]{
            new ToDescription_TestCase("en-US", NewLineCodes.Lf, true,"[R_en] Unix(LF)"),
            new ToDescription_TestCase("en-US", NewLineCodes.Cr, true, "[R_en] Mac(CR)"),
            new ToDescription_TestCase("en-US", NewLineCodes.CrLf, true, "[A] Windows(CR+LF)"),
            new ToDescription_TestCase("en-US", NewLineCodes.LfCr, true, "LfCr"),
            new ToDescription_TestCase("en-US", (NewLineCodes)1, true, "[R_en] Unix(LF)"),
            new ToDescription_TestCase("en-US", (NewLineCodes)2, true, "[R_en] Mac(CR)"),
            new ToDescription_TestCase("en-US", (NewLineCodes)4, true, "[A] Windows(CR+LF)"),
            new ToDescription_TestCase("en-US", (NewLineCodes)8, true, "LfCr"),
            new ToDescription_TestCase("en-US", (NewLineCodes)0, false, "0"),
            new ToDescription_TestCase("en-US", (NewLineCodes)3, false, "3"),
            new ToDescription_TestCase("ja-JP", NewLineCodes.Lf, true, "[R_ja] ユニックス(LF)"),
            new ToDescription_TestCase("ja-JP", NewLineCodes.Cr, true, "[R_en] Mac(CR)"),
            new ToDescription_TestCase("ja-JP", NewLineCodes.CrLf, true, "[R_ja] ウィンドウズ(CR+LF)"),
            new ToDescription_TestCase("ja-JP", NewLineCodes.LfCr, true, "LfCr"),
            new ToDescription_TestCase("ja-JP", (NewLineCodes)1, true, "[R_ja] ユニックス(LF)"),
            new ToDescription_TestCase("ja-JP", (NewLineCodes)2, true, "[R_en] Mac(CR)"),
            new ToDescription_TestCase("ja-JP", (NewLineCodes)4, true, "[R_ja] ウィンドウズ(CR+LF)"),
            new ToDescription_TestCase("ja-JP", (NewLineCodes)8, true, "LfCr"),
            new ToDescription_TestCase("ja-JP", (NewLineCodes)0, false, "0"),
            new ToDescription_TestCase("ja-JP", (NewLineCodes)3, false, "3"),
        };

        public static class ToDescription_TestCaseProvider {
            public static IEnumerable<TestCaseData> AllTestCases {
                get {
                    foreach (var testCase in ToDescription_TestCases) {
                        yield return new TestCaseData(testCase.Culture, testCase.Item, testCase.IsValid, testCase.Expected).Returns(testCase.Expected);
                    }
                }
            }
        }

        [Test, TestCaseSource(typeof(ToDescription_TestCaseProvider), "AllTestCases")]
        public string ToDescription_Test(string culture, NewLineCodes item, bool isValid, string expected) {
            if (!String.IsNullOrEmpty(culture)) {
                SetCurrentCulture(culture);
            }
            return item.ToDescription();
        }

        [Test, TestCaseSource(typeof(ToDescription_TestCaseProvider), "AllTestCases")]
        public string ValueDescriptionDictionary_Test(string culture, NewLineCodes item, bool isValid, string expected) {
            if (!String.IsNullOrEmpty(culture)) {
                SetCurrentCulture(culture);
            }
            var dic = NewLineCodeHelper.ValueDescriptionDictionary;
            Assert.AreEqual(isValid, dic.ContainsKey(item));
            return isValid ?
                dic[item] :
                expected; // skip
        }

        [TestCase(NewLineCodes.Lf, "\n", "\"\'\t")]
        [TestCase(NewLineCodes.Cr, "\r", "\'\"")]
        [TestCase(NewLineCodes.CrLf, "\r\n", "あ")]
        [TestCase(NewLineCodes.LfCr, "\n\r", "\uD842\uDFB7")]
        [TestCase((NewLineCodes)1, "\n", "\x22\u0027\x09")]
        [TestCase((NewLineCodes)2, "\r", "\u0027\x22")]
        [TestCase((NewLineCodes)4, "\r\n", "\u3042")]
        [TestCase((NewLineCodes)8, "\n\r", "\xD842\xDFB7")]
        [TestCase((NewLineCodes)0, null, null)]
        [TestCase((NewLineCodes)3, null, null)]
        public void GetExtraProperty_Test(NewLineCodes item, string expectedEntity, string expectedEscaped) {
            var actualEntity = item.GetExtraProperty("Entity");
            if (expectedEntity != null) {
                Assert.AreEqual(expectedEntity, actualEntity);
            } else {
                Assert.Null(actualEntity);
            }
            var actualEscaped = item.GetExtraProperty("Escaped");
            if (expectedEscaped != null) {
                Assert.AreEqual(expectedEscaped, actualEscaped);
            } else {
                Assert.Null(actualEscaped);
            }
        }
    }
}
