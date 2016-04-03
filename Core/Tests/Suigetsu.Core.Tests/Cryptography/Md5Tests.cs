using NUnit.Framework;
using Suigetsu.Core.Cryptography;
using Suigetsu.Core.Extensions;
using Suigetsu.Core.IO;

namespace Suigetsu.Core.Tests.Cryptography
{
    [TestFixture]
    public class Md5Tests : AssertionHelper
    {
        [Test]
        public void FromFileTest()
        {
            Expect(Md5.FromFile(string.Empty), Empty);
            using(var file = new TempFile())
            {
                Expect(Md5.FromFile(file.FilePath), EqualTo("d41d8cd98f00b204e9800998ecf8427e"));
                file.Handle.WriteByte(65);
                file.Handle.Flush();
                Expect(Md5.FromFile(file.FilePath), EqualTo("7fc56270e7a70fa81a5935b72eacbe29"));
                Expect(Md5.FromFile(file.FilePath, 3), EqualTo("01484f32e1eae8c021a3bd2fb3d1062f"));
            }
        }

        [Test]
        public void FromStringTest()
        {
            Expect(Md5.FromString(string.Empty), EqualTo("d41d8cd98f00b204e9800998ecf8427e"));
            Expect(Md5.FromString("A"), EqualTo("7fc56270e7a70fa81a5935b72eacbe29"));
            Expect("A".GetMd5(3), EqualTo("01484f32e1eae8c021a3bd2fb3d1062f"));
        }

        [Test]
        public void IsValidTest()
        {
            Expect(Md5.IsValid("d41d8cd98f00b204e9800998ecf8427e"));
            Expect(Md5.IsValid(""), False);
            Expect(Md5.IsValid("d41d8cd98f00b204e9800998ecf8427g"), False);
        }

        [Test]
        public void SaltTest()
        {
            Expect(Md5.Salt(string.Empty), EqualTo("80170f5424a44bc7d69c716c6871c146"));
            Expect(Md5.Salt("A"), EqualTo("a1dc299f666c6945140445f932a22ef9"));

            Expect(Md5.Salt(string.Empty, "SALT"), EqualTo("365751b6a935d5665439502c5300ac8a"));
            Expect(Md5.Salt("A", "SALT"), EqualTo("99c3a80a7573b237c583b7c6b5d015af"));

            Expect(Md5.Salt(string.Empty, 3), EqualTo("9ec5e22348ccd369ece8d0d57f19130d"));
            Expect(Md5.Salt("A", 3), EqualTo("f90205c72be64dde3665d18cd062632b"));
        }
    }
}
