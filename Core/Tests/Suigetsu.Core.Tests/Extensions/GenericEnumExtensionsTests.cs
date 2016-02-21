using NUnit.Framework;
using Suigetsu.Core.Extensions;

namespace Suigetsu.Core.Tests.Extensions
{
    [TestFixture]
    public class GenericEnumExtensionsTests : AssertionHelper
    {
        //TODO: missing code
        private enum TestEnum
        {
            //[EnumRepo("TestEnumItemId", "TestEnumItemDesc", 1, 2.0f, '3', TestEnum2.TestEnumItem2)]
            TestEnumItem
        }

        private enum TestEnum2
        {
            //[EnumRepo(null, "TestEnumItem2Desc")]
            TestEnumItem2
        }

        [Test]
        public void EnumRepoTest()
        {
            //Expect(TestEnum.TestEnumItem.GetRepo().Desc, EqualTo("TestEnumItemDesc"));
            //Expect(EnumUtils.DummyEnum.DummyEnumItem.GetRepo().Desc, Empty);
            //Expect(default(TestEnum).GetEmpty().GetRepo().Desc, Empty);

            //Expect(default(TestEnum).ById(string.Empty).IsEmptyEnum());
            Expect(default(TestEnum).GetEmpty().IsEmptyEnum());
            Expect(TestEnum.TestEnumItem.IsEmptyEnum(), False);

            //Expect(EnumRepo.EnumById<TestEnum>("TestEnumItemId"), EqualTo(TestEnum.TestEnumItem));
            //Expect(default(TestEnum).ById("TestEnumItemId"), EqualTo(TestEnum.TestEnumItem));
            //Expect(EnumRepo.EnumById<TestEnum>(string.Empty), EqualTo(default(TestEnum).GetEmpty()));
            //Expect(default(TestEnum).ById(string.Empty), EqualTo(default(TestEnum).GetEmpty()));
            //Expect(default(TestEnum).GetRepoById("TestEnumItemId").Desc, EqualTo("TestEnumItemDesc"));

            //Expect(TestEnum.TestEnumItem.GetRepo()[-1], Null);
            //Expect(TestEnum.TestEnumItem.GetRepo().Arg<int>(-1), EqualTo(0));
            //Expect(TestEnum.TestEnumItem.GetRepo().Arg<char>(-1), EqualTo('\0'));
            //Expect(TestEnum.TestEnumItem.GetRepo().Arg<TestEnum>(-1), EqualTo(default(TestEnum).GetEmpty()));

            //Expect(TestEnum.TestEnumItem.GetRepo()[0], EqualTo(1));
            //Expect(TestEnum.TestEnumItem.GetRepo()[1], EqualTo(2.0f));
            //Expect(TestEnum.TestEnumItem.GetRepo()[2], EqualTo('3'));
            //Expect(TestEnum.TestEnumItem.GetRepo()[3], EqualTo(TestEnum2.TestEnumItem2));

            //Expect(TestEnum.TestEnumItem.GetRepo().Arg<TestEnum2>(3).GetRepo().Id, EqualTo("TestEnumItem2"));
            //Expect(TestEnum.TestEnumItem.GetRepo().ArgAsRepo(3).Id, EqualTo("TestEnumItem2"));
        }
    }
}
