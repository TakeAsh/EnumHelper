using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TakeAsh;

namespace EnumHelperLib_Test {

    [TestFixture]
    public class EnumEnumDictionary_Test {

        public enum GeneralStates {
            Wainting,
            Working,
            Complete,
            Canceled,
            Failed,
        }

        public enum DeviceStates1 {
            Wainting,
            Working,
            Complete,
            Canceled,
            Failed,
        }

        public enum DeviceStates2 {
            Wainting,
            Working,
            Complete,
            Canceled,
            Failed,
            Exception,
        }

        public enum DeviceStates3 {
            Failed = -2,
            Canceled = -1,
            Wainting = 0,
            Working,
            Complete,
        }

        EnumEnumDictionary<GeneralStates, DeviceStates1> dic1a =
            new EnumEnumDictionary<GeneralStates, DeviceStates1>() {
                {GeneralStates.Wainting, DeviceStates1.Wainting},
                {GeneralStates.Working, DeviceStates1.Working},
                {GeneralStates.Complete, DeviceStates1.Complete},
                {GeneralStates.Canceled, DeviceStates1.Canceled},
                {GeneralStates.Failed, DeviceStates1.Failed},
            };

        EnumEnumDictionary<GeneralStates, DeviceStates1> dic1b =
            new EnumEnumDictionary<GeneralStates, DeviceStates1>(true);

        EnumEnumDictionary<GeneralStates, DeviceStates2> dic2a =
            new EnumEnumDictionary<GeneralStates, DeviceStates2>(){
                {GeneralStates.Wainting, DeviceStates2.Wainting},
                {GeneralStates.Working, DeviceStates2.Working},
                {GeneralStates.Complete, DeviceStates2.Complete},
                {GeneralStates.Canceled, DeviceStates2.Canceled},
                {GeneralStates.Failed, DeviceStates2.Failed},
            };

        EnumEnumDictionary<GeneralStates, DeviceStates3> dic3 =
            new EnumEnumDictionary<GeneralStates, DeviceStates3>(){
                {GeneralStates.Wainting, DeviceStates3.Wainting},
                {GeneralStates.Working, DeviceStates3.Working},
                {GeneralStates.Complete, DeviceStates3.Complete},
                {GeneralStates.Canceled, DeviceStates3.Canceled},
                {GeneralStates.Failed, DeviceStates3.Failed},
            };

        KeyValuePair<GeneralStates, DeviceStates3>[] dic3Expected =
            new KeyValuePair<GeneralStates, DeviceStates3>[]{
                new KeyValuePair<GeneralStates, DeviceStates3>(GeneralStates.Wainting, DeviceStates3.Wainting),
                new KeyValuePair<GeneralStates, DeviceStates3>(GeneralStates.Working, DeviceStates3.Working),
                new KeyValuePair<GeneralStates, DeviceStates3>(GeneralStates.Complete, DeviceStates3.Complete),
                new KeyValuePair<GeneralStates, DeviceStates3>(GeneralStates.Canceled, DeviceStates3.Canceled),
                new KeyValuePair<GeneralStates, DeviceStates3>(GeneralStates.Failed, DeviceStates3.Failed),
            };

        [TestCase]
        public void Constructor1_Manual() {
            var keys = EnumHelper<GeneralStates>.Values;
            var values = EnumHelper<DeviceStates1>.Values;
            Assert.AreEqual(keys.Length, dic1a.Count);
            for (var i = 0; i < keys.Length; ++i) {
                Assert.AreEqual(values[i], dic1a[keys[i]]);
            }
        }

        [TestCase]
        public void Constructor1_Auto() {
            var keys = EnumHelper<GeneralStates>.Values;
            var values = EnumHelper<DeviceStates1>.Values;
            Assert.AreEqual(keys.Length, dic1b.Count);
            for (var i = 0; i < keys.Length; ++i) {
                Assert.AreEqual(values[i], dic1b[keys[i]]);
            }
        }

        [TestCase((GeneralStates)(-1), default(DeviceStates1))]
        public void Indexer1a(GeneralStates key, DeviceStates1 expected) {
            Assert.AreEqual(expected, dic1a[key]);
        }

        [TestCase((GeneralStates)(-1), default(DeviceStates1))]
        [TestCase(null, default(DeviceStates1))]
        public void Indexer1b(GeneralStates? key, DeviceStates1 expected) {
            Assert.AreEqual(expected, dic1a[key]);
        }

        [TestCase]
        public void Constructor2_Manual() {
            var keys = EnumHelper<GeneralStates>.Values;
            var values = EnumHelper<DeviceStates2>.Values;
            Assert.AreEqual(keys.Length, dic2a.Count);
            for (var i = 0; i < keys.Length; ++i) {
                Assert.AreEqual(values[i], dic2a[keys[i]]);
            }
        }

        [TestCase]
        public void Constructor2_Auto() {
            var ex = Assert.Throws<ArgumentException>(() => {
                var dic2b = new EnumEnumDictionary<GeneralStates, DeviceStates2>(true);
            });
            Assert.AreEqual("TKey length is not equal to TValue length", ex.Message);
        }

        [TestCase]
        public void Constructor3() {
            var keys = EnumHelper<GeneralStates>.Values;
            var values = EnumHelper<DeviceStates3>.Values;
            Assert.AreEqual(dic3Expected.Length, dic3.Count);
            for (var i = 0; i < dic3Expected.Length; ++i) {
                Assert.AreEqual(dic3Expected[i].Value, dic3[dic3Expected[i].Key]);
            }
        }

        [TestCase((GeneralStates)(-1), default(DeviceStates3))]
        public void Indexer3a(GeneralStates key, DeviceStates3 expected) {
            Assert.AreEqual(expected, dic3[key]);
        }

        [TestCase((GeneralStates)(-1), default(DeviceStates3))]
        [TestCase(null, default(DeviceStates3))]
        public void Indexer3b(GeneralStates? key, DeviceStates3 expected) {
            Assert.AreEqual(expected, dic3[key]);
        }
    }
}
