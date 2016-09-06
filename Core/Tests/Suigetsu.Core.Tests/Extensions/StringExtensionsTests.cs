using System;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Suigetsu.Core.Extensions;
using Suigetsu.Core.Serialization;

namespace Suigetsu.Core.Tests.Extensions
{
    [TestFixture]
    public class StringExtensionsTests : AssertionHelper
    {
        [Test]
        public void FromJsonTest()
        {
            Expect("[1,2,3]".FromJson<int[]>(), EquivalentTo(new[] { 1, 2, 3 }));

            Expect
                ("{WrongJsonPropertyTest: 1}".FromJson<ObjectExtensionsTests.JsonClassTest>
                     (new JsonSerializerSettingsWrapperParameters
                     {
                         OnCreateProperty = property =>
                         {
                             if(property.PropertyName == "JsonPropertyTest")
                             {
                                 property.PropertyName = "WrongJsonPropertyTest";
                             }
                         }
                     }).JsonPropertyTest);

            Expect
                (GenericEnumExtensionsTests.TestEnum.TestEnumItem.GetRepo()
                     .Id.Wrap("\"")
                     .FromJson<GenericEnumExtensionsTests.TestEnum>(),
                 EqualTo(GenericEnumExtensionsTests.TestEnum.TestEnumItem));

            Expect
                (((int)GenericEnumExtensionsTests.TestEnum.TestEnumItem).ToString()
                     .FromJson<GenericEnumExtensionsTests.TestEnum>
                     (new JsonSerializerSettingsWrapperParameters
                     {
                         RegisterCustomContracts = false
                     }),
                 EqualTo(GenericEnumExtensionsTests.TestEnum.TestEnumItem));
        }

        [Test]
        public void GetNumbersTest() => Expect("A1B2C".GetNumbers(), EqualTo("12"));

        [Test]
        public void HexToBytesTest()
        {
            Expect("41".HexToBytes(), EquivalentTo(new[] { 65 }));
            Expect("0041".HexToDigits().ToArray(), EquivalentTo(new[] { 65 }));
            Expect("41".HexToDigits(2).ToArray(), EquivalentTo(new[] { 65 }));
            Expect("9".HexToDigits(1).ToArray(), EquivalentTo(new[] { 9 }));
            Expect("9".HexToDigits(1).ToArray(), EquivalentTo(new[] { 9 }));
            Expect(string.Empty.HexToDigits().ToArray(), Empty);
            Expect("414".HexToDigits(2).ToArray(), EquivalentTo(new[] { 65 }));
        }

        [Test]
        public void HexToStringTest()
        {
            Expect("0041".HexToString(), EqualTo("A"));
            Expect("41".HexToString(2), EqualTo("A"));
            Expect("9".HexToString(1), EqualTo("\t"));
        }

        [Test]
        public void IsEmptyTest()
        {
            Expect(string.Empty.IsEmpty());
            Expect(((string)null).IsEmpty());
            Expect(StringExtensions.IsEmpty(null));
        }

        [Test]
        public void IsEmptyTrimTest()
        {
            Expect(string.Empty.IsEmptyTrim());
            Expect(((string)null).IsEmptyTrim());
            Expect(" ".IsEmptyTrim());
        }

        [Test]
        public void ReverseTest() => Expect("1bA".Reverse(), EqualTo("Ab1"));

        [Test]
        public void ToDateTimeTest()
            => Expect("01/02/2000 00:30:00".ToDateTime(), EqualTo(new DateTime(2000, 02, 01, 00, 30, 00)));

        [Test]
        public void ToIntTest()
        {
            Expect("1234".ToInt(), EqualTo(1234));
            Expect("".ToInt(), EqualTo(0));
            Expect(((string)null).ToInt(), EqualTo(0));
        }

        [Test]
        public void ToNIntTest()
        {
            Expect("1234".ToNInt(), EqualTo(1234));
            Expect("".ToNInt(), EqualTo(null));
            Expect(((string)null).ToNInt(), EqualTo(null));
        }

        [Test]
        public void ToLongTest() => Expect("1234".ToLong().ToString(), EqualTo("1234"));

        [Test]
        public void ToUtf8Test()
            => Expect(Encoding.GetEncoding(1252).GetString(new byte[] { 0xC3, 0xA7 }).ToUtf8(), EqualTo("ç"));
    }
}
