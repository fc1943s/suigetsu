using NUnit.Framework;
using Suigetsu.Core.Enums;
using Suigetsu.Core.Extensions;
using Suigetsu.Core.Util;

namespace Suigetsu.Core.Tests.Extensions
{
    [TestFixture]
    public class GenericEnumExtensionsTests : AssertionHelper
    {
        public enum TestEnum
        {
            [EnumRepo("TestEnumItemId", "TestEnumItemDesc", 1, 2.0f, '3', TestEnum2.TestEnumItem2)]
            TestEnumItem,

            [EnumRepo("TestEnumItem2Id")]
            TestEnumItem2
        }

        private enum TestEnum2
        {
            [EnumRepo(null, "TestEnumItem2Desc")]
            TestEnumItem2
        }

        [Test]
        public void EnumRepoTest()
        {
            Expect(TestEnum.TestEnumItem.GetRepo().Desc, EqualTo("TestEnumItemDesc"));
            Expect(TestEnum.TestEnumItem2.GetRepo().Desc, Empty);
            Expect(EnumUtils.DummyEnum.DummyEnumItem.GetRepo().Desc, Empty);
            Expect(default(TestEnum).GetEmpty().GetRepo().Desc, Empty);

            Expect(default(TestEnum).ById(string.Empty).IsEmptyEnum());
            Expect(default(TestEnum).GetEmpty().IsEmptyEnum());
            Expect(TestEnum.TestEnumItem2.IsEmptyEnum(), False);

            Expect(EnumRepo.EnumById<TestEnum>("TestEnumItemId"), EqualTo(TestEnum.TestEnumItem));
            Expect(EnumRepo.EnumByDesc<TestEnum>(string.Empty), EqualTo(TestEnum.TestEnumItem2));
            Expect(default(TestEnum).ById("TestEnumItemId"), EqualTo(TestEnum.TestEnumItem));
            Expect(EnumRepo.EnumById<TestEnum>(string.Empty), EqualTo(default(TestEnum).GetEmpty()));
            Expect(default(TestEnum).ById(string.Empty), EqualTo(default(TestEnum).GetEmpty()));
            Expect(default(TestEnum).ByDesc(string.Empty), EqualTo(TestEnum.TestEnumItem2));
            Expect(default(TestEnum).GetRepoById("TestEnumItemId").Desc, EqualTo("TestEnumItemDesc"));

            Expect(TestEnum.TestEnumItem.GetRepo()[-1], Null);
            Expect(TestEnum.TestEnumItem.GetRepo().Arg<int>(-1), EqualTo(0));
            Expect(TestEnum.TestEnumItem.GetRepo().Arg<char>(-1), EqualTo('\0'));
            Expect(TestEnum.TestEnumItem.GetRepo().Arg<TestEnum>(-1), EqualTo(default(TestEnum).GetEmpty()));

            Expect(TestEnum.TestEnumItem.GetRepo()[0], EqualTo(1));
            Expect(TestEnum.TestEnumItem.GetRepo()[1], EqualTo(2.0f));
            Expect(TestEnum.TestEnumItem.GetRepo()[2], EqualTo('3'));
            Expect(TestEnum.TestEnumItem.GetRepo()[3], EqualTo(TestEnum2.TestEnumItem2));

            Expect(TestEnum.TestEnumItem.GetRepo().Arg<TestEnum2>(3).GetRepo().Id, EqualTo("TestEnumItem2"));
            Expect(TestEnum.TestEnumItem.GetRepo().ArgAsRepo(3).Id, EqualTo("TestEnumItem2"));
        }
    }
}
