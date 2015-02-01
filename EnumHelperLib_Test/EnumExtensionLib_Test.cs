using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Threading;
using EnumHelperLib_Call;
using F2DUnitTests;
using NUnit.Framework;
using TakeAsh;

namespace EnumExtensionLib_Test {

    using EnumDescriptionDictionary = Dictionary<Options.NewLineCodes, string>;
    using NewLineCodeHelper = EnumHelper<Options.NewLineCodes>;

    [TestFixture]
    public class EnumExtensionLib_Test {

        public Dictionary<string, EnumDescriptionDictionary> expectedDescriptions =
            new Dictionary<string, EnumDescriptionDictionary>() {
            {
                "en-US",
                new EnumDescriptionDictionary(){
                    {Options.NewLineCodes.Lf, "[R_en] Unix(LF)"},
                    {Options.NewLineCodes.Cr, "[R_en] Mac(CR)"},
                    {Options.NewLineCodes.CrLf, "[A] Windows(CR+LF)"},
                    {Options.NewLineCodes.LfCr, "LfCr"},
                }
            },
            {
                "ja-JP",
                new EnumDescriptionDictionary(){
                    {Options.NewLineCodes.Lf, "[R_ja] ユニックス(LF)"},
                    {Options.NewLineCodes.Cr, "[R_en] Mac(CR)"},
                    {Options.NewLineCodes.CrLf, "[R_ja] ウィンドウズ(CR+LF)"},
                    {Options.NewLineCodes.LfCr, "LfCr"},
                }
            },
        };

        [SetUp]
        public void setup() {
            AssemblyUtilities.SetEntryAssembly(Assembly.GetAssembly(typeof(Options)));
        }

        private void SetCurrentCulture(string cultureName) {
            var culture = new CultureInfo(cultureName);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            NewLineCodeHelper.Init();
        }

        [TestCase]
        public void Assembly_Test() {
            var expected = "EnumHelperLib_Call";
            Assert.AreEqual(expected, NewLineCodeHelper.GetAssemblyName());
            Assert.AreEqual(expected, Options.NewLineCodes.Lf.GetAssemblyName());
        }

        [TestCase]
        public void Values_Test() {
            var expected = new Options.NewLineCodes[]{
                Options.NewLineCodes.Lf,
                Options.NewLineCodes.Cr,
                Options.NewLineCodes.CrLf,
                Options.NewLineCodes.LfCr,
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
                var expected = new KeyValuePair<Options.NewLineCodes, string>[expectedDescriptions[culture].Count];
                SetCurrentCulture(culture);
                for (var i = 0; i < NewLineCodeHelper.Values.Length; ++i) {
                    var key = NewLineCodeHelper.Values[i];
                    expected[i] = new KeyValuePair<Options.NewLineCodes, string>(key, expectedDescriptions[culture][key]);
                }
                CollectionAssert.AreEqual(
                    expected,
                    NewLineCodeHelper.ValueDescriptionPairs,
                    culture
                );
            }
        }

        [TestCase("Lf", Options.NewLineCodes.Lf)]
        [TestCase("Cr", Options.NewLineCodes.Cr)]
        [TestCase("CrLf", Options.NewLineCodes.CrLf)]
        [TestCase("LfCr", Options.NewLineCodes.LfCr)]
        [TestCase(null, default(Options.NewLineCodes))]
        [TestCase("", default(Options.NewLineCodes))]
        [TestCase("Undefined", default(Options.NewLineCodes))]
        public void GetValueFromName_Test(string name, Options.NewLineCodes expected) {
            Assert.AreEqual(
                expected,
                NewLineCodeHelper.GetValueFromName(name)
            );
        }

        [TestCase("Lf", Options.NewLineCodes.Lf)]
        [TestCase("Cr", Options.NewLineCodes.Cr)]
        [TestCase("CrLf", Options.NewLineCodes.CrLf)]
        [TestCase("LfCr", Options.NewLineCodes.LfCr)]
        [TestCase(null, null)]
        [TestCase("", null)]
        [TestCase("Undefined", null)]
        public void GetValueNullableFromName_Test(string name, Options.NewLineCodes? expected) {
            var actual = NewLineCodeHelper.GetValueNullableFromName(name);
            if (expected != null) {
                Assert.AreEqual(expected, actual);
            } else {
                Assert.Null(actual);
            }
        }

        [TestCase("en-US", "[R_en] Unix(LF)", Options.NewLineCodes.Lf)]
        [TestCase("en-US", "[R_en] Mac(CR)", Options.NewLineCodes.Cr)]
        [TestCase("en-US", "[A] Windows(CR+LF)", Options.NewLineCodes.CrLf)]
        [TestCase("en-US", "LfCr", Options.NewLineCodes.LfCr)]
        [TestCase("en-US", null, default(Options.NewLineCodes))]
        [TestCase("en-US", "", default(Options.NewLineCodes))]
        [TestCase("en-US", "Undefined", default(Options.NewLineCodes))]
        [TestCase("ja-JP", "[R_ja] ユニックス(LF)", Options.NewLineCodes.Lf)]
        [TestCase("ja-JP", "[R_en] Mac(CR)", Options.NewLineCodes.Cr)]
        [TestCase("ja-JP", "[R_ja] ウィンドウズ(CR+LF)", Options.NewLineCodes.CrLf)]
        [TestCase("ja-JP", "LfCr", Options.NewLineCodes.LfCr)]
        [TestCase("ja-JP", null, default(Options.NewLineCodes))]
        [TestCase("ja-JP", "", default(Options.NewLineCodes))]
        [TestCase("ja-JP", "Undefined", default(Options.NewLineCodes))]
        public void GetValueFromDescription_Test(string culture, string description, Options.NewLineCodes expected) {
            if (!String.IsNullOrEmpty(culture)) {
                SetCurrentCulture(culture);
            }
            Assert.AreEqual(
                expected,
                NewLineCodeHelper.GetValueFromDescription(description)
            );
        }

        [TestCase("en-US", "[R_en] Unix(LF)", Options.NewLineCodes.Lf)]
        [TestCase("en-US", "[R_en] Mac(CR)", Options.NewLineCodes.Cr)]
        [TestCase("en-US", "[A] Windows(CR+LF)", Options.NewLineCodes.CrLf)]
        [TestCase("en-US", "LfCr", Options.NewLineCodes.LfCr)]
        [TestCase("en-US", null, null)]
        [TestCase("en-US", "", null)]
        [TestCase("en-US", "Undefined", null)]
        [TestCase("ja-JP", "[R_ja] ユニックス(LF)", Options.NewLineCodes.Lf)]
        [TestCase("ja-JP", "[R_en] Mac(CR)", Options.NewLineCodes.Cr)]
        [TestCase("ja-JP", "[R_ja] ウィンドウズ(CR+LF)", Options.NewLineCodes.CrLf)]
        [TestCase("ja-JP", "LfCr", Options.NewLineCodes.LfCr)]
        [TestCase("ja-JP", null, null)]
        [TestCase("ja-JP", "", null)]
        [TestCase("ja-JP", "Undefined", null)]
        public void GetValueNullableFromDescription_Test(string culture, string description, Options.NewLineCodes? expected) {
            if (!String.IsNullOrEmpty(culture)) {
                SetCurrentCulture(culture);
            }
            var actual = NewLineCodeHelper.GetValueNullableFromDescription(description);
            if (expected != null) {
                Assert.AreEqual(expected, actual);
            } else {
                Assert.Null(actual);
            }
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

        [TestCase("Lf", true, Options.NewLineCodes.Lf)]
        [TestCase("Cr", true, Options.NewLineCodes.Cr)]
        [TestCase("CrLf", true, Options.NewLineCodes.CrLf)]
        [TestCase("LfCr", true, Options.NewLineCodes.LfCr)]
        [TestCase("1", true, Options.NewLineCodes.Lf)]
        [TestCase("2", true, Options.NewLineCodes.Cr)]
        [TestCase("4", true, Options.NewLineCodes.CrLf)]
        [TestCase("8", true, Options.NewLineCodes.LfCr)]
        [TestCase(null, false, default(Options.NewLineCodes))]
        [TestCase("", false, default(Options.NewLineCodes))]
        [TestCase("Undefined", false, default(Options.NewLineCodes))]
        public void TryParse_Test(string value, bool expectedReturn, Options.NewLineCodes expectedResult) {
            Options.NewLineCodes actualResult;
            Assert.AreEqual(expectedReturn, NewLineCodeHelper.TryParse(value, out actualResult));
            if (expectedReturn) {
                Assert.AreEqual(expectedResult, actualResult);
            }
        }

        public struct ToDescription_TestCase {
            public string Culture { get; set; }
            public Options.NewLineCodes Item { get; set; }
            public bool IsValid { get; set; }
            public string Expected { get; set; }

            public ToDescription_TestCase(
                string culture,
                Options.NewLineCodes item,
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
            new ToDescription_TestCase("en-US", Options.NewLineCodes.Lf, true,"[R_en] Unix(LF)"),
            new ToDescription_TestCase("en-US", Options.NewLineCodes.Cr, true, "[R_en] Mac(CR)"),
            new ToDescription_TestCase("en-US", Options.NewLineCodes.CrLf, true, "[A] Windows(CR+LF)"),
            new ToDescription_TestCase("en-US", Options.NewLineCodes.LfCr, true, "LfCr"),
            new ToDescription_TestCase("en-US", (Options.NewLineCodes)1, true, "[R_en] Unix(LF)"),
            new ToDescription_TestCase("en-US", (Options.NewLineCodes)2, true, "[R_en] Mac(CR)"),
            new ToDescription_TestCase("en-US", (Options.NewLineCodes)4, true, "[A] Windows(CR+LF)"),
            new ToDescription_TestCase("en-US", (Options.NewLineCodes)8, true, "LfCr"),
            new ToDescription_TestCase("en-US", (Options.NewLineCodes)0, false, "0"),
            new ToDescription_TestCase("en-US", (Options.NewLineCodes)3, false, "3"),
            new ToDescription_TestCase("ja-JP", Options.NewLineCodes.Lf, true, "[R_ja] ユニックス(LF)"),
            new ToDescription_TestCase("ja-JP", Options.NewLineCodes.Cr, true, "[R_en] Mac(CR)"),
            new ToDescription_TestCase("ja-JP", Options.NewLineCodes.CrLf, true, "[R_ja] ウィンドウズ(CR+LF)"),
            new ToDescription_TestCase("ja-JP", Options.NewLineCodes.LfCr, true, "LfCr"),
            new ToDescription_TestCase("ja-JP", (Options.NewLineCodes)1, true, "[R_ja] ユニックス(LF)"),
            new ToDescription_TestCase("ja-JP", (Options.NewLineCodes)2, true, "[R_en] Mac(CR)"),
            new ToDescription_TestCase("ja-JP", (Options.NewLineCodes)4, true, "[R_ja] ウィンドウズ(CR+LF)"),
            new ToDescription_TestCase("ja-JP", (Options.NewLineCodes)8, true, "LfCr"),
            new ToDescription_TestCase("ja-JP", (Options.NewLineCodes)0, false, "0"),
            new ToDescription_TestCase("ja-JP", (Options.NewLineCodes)3, false, "3"),
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
        public string ToDescription_Test(string culture, Options.NewLineCodes item, bool isValid, string expected) {
            if (!String.IsNullOrEmpty(culture)) {
                SetCurrentCulture(culture);
            }
            return item.ToDescription();
        }

        [Test, TestCaseSource(typeof(ToDescription_TestCaseProvider), "AllTestCases")]
        public string ValueDescriptionDictionary_Test(string culture, Options.NewLineCodes item, bool isValid, string expected) {
            if (!String.IsNullOrEmpty(culture)) {
                SetCurrentCulture(culture);
            }
            var dic = NewLineCodeHelper.ValueDescriptionDictionary;
            Assert.AreEqual(isValid, dic.ContainsKey(item));
            return isValid ?
                dic[item] :
                expected; // skip
        }

        [TestCase(Options.NewLineCodes.Lf, "\n", "\"\'\t")]
        [TestCase(Options.NewLineCodes.Cr, "\r", "\'\"")]
        [TestCase(Options.NewLineCodes.CrLf, "\r\n", "あ")]
        [TestCase(Options.NewLineCodes.LfCr, "\n\r", "\uD842\uDFB7")]
        [TestCase((Options.NewLineCodes)1, "\n", "\x22\u0027\x09")]
        [TestCase((Options.NewLineCodes)2, "\r", "\u0027\x22")]
        [TestCase((Options.NewLineCodes)4, "\r\n", "\u3042")]
        [TestCase((Options.NewLineCodes)8, "\n\r", "\xD842\xDFB7")]
        [TestCase((Options.NewLineCodes)0, null, null)]
        [TestCase((Options.NewLineCodes)3, null, null)]
        public void GetExtraProperty_Test(Options.NewLineCodes item, string expectedEntity, string expectedEscaped) {
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

        [TestCase(Options.NewLineCodes.Lf, Options.NewLineCodes.Lf)]
        [TestCase(Options.NewLineCodes.Cr, Options.NewLineCodes.Cr)]
        [TestCase(Options.NewLineCodes.CrLf, Options.NewLineCodes.CrLf)]
        [TestCase(Options.NewLineCodes.LfCr, Options.NewLineCodes.LfCr)]
        [TestCase((Options.NewLineCodes)0, (Options.NewLineCodes)0)]
        [TestCase((Options.NewLineCodes)3, (Options.NewLineCodes)3)]
        [TestCase(null, default(Options.NewLineCodes))]
        public void Cast_Test(object item, Options.NewLineCodes expected) {
            Assert.AreEqual(expected, NewLineCodeHelper.Cast(item));
        }
    }
}
