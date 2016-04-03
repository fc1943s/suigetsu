using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json.Converters;
using NUnit.Framework;
using Suigetsu.Core.Extensions;
using Suigetsu.Core.Serialization;

namespace Suigetsu.Core.Tests.Extensions
{
    [TestFixture]
    public class ObjectExtensionsTests : AssertionHelper
    {
        [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
        public class JsonClassTest
        {
            public bool JsonPropertyTest = false;
        }

        [Test]
        public void ToJsonTest()
        {
            Expect(new[] { 1, 2, 3 }.ToJson(), EqualTo("[1,2,3]"));

            Expect
                (new Dictionary<string, bool>
                {
                    { "Test1", true },
                    { "Test2", false }
                }.ToJson
                     (new JsonSerializerSettingsWrapperParameters
                     {
                         Indented = true
                     }),
                 EqualTo(@"{
  ""Test1"": true,
  ""Test2"": false
}".FixNewLine()));

            Expect
                (GenericEnumExtensionsTests.TestEnum.TestEnumItem.ToJson(),
                 EqualTo(GenericEnumExtensionsTests.TestEnum.TestEnumItem.GetRepo().Id.Wrap("\"")));

            Expect
                (GenericEnumExtensionsTests.TestEnum.TestEnumItem.ToJson
                     (new JsonSerializerSettingsWrapperParameters
                     {
                         RegisterCustomContracts = false
                     }),
                 EqualTo(((int)GenericEnumExtensionsTests.TestEnum.TestEnumItem).ToString()));

            Expect
                (946692000000.ToDateTime().ToJson
                     (new JsonSerializerSettingsWrapperParameters
                     {
                         OnCreateContract = contract =>
                         {
                             if(contract.UnderlyingType == typeof(DateTime))
                             {
                                 contract.Converter = new JavaScriptDateTimeConverter();
                             }
                         }
                     }),
                 EqualTo("new Date(946692000000)"));
        }
    }
}
