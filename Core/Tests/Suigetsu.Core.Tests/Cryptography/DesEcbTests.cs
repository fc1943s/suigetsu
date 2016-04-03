using System.Linq;
using System.Text;
using NUnit.Framework;
using Suigetsu.Core.Cryptography;

namespace Suigetsu.Core.Tests.Cryptography
{
    [TestFixture]
    public class DesEcbTests : AssertionHelper
    {
        [Test]
        public void CryptTest()
        {
            var data = Encoding.Default.GetBytes("TEST");
            const string key = "KEY12345";
            var keyBytes = Encoding.Default.GetBytes(key);

            var encryptedData1 = DesEcb.Encrypt(data, keyBytes, 3);
            var encryptedData2 = DesEcb.Encrypt(data, key);

            var result1 = DesEcb.Decrypt(encryptedData1, key, 3);
            var result2 = DesEcb.Decrypt(encryptedData2, keyBytes);

            Expect(result1, EqualTo(data));
            Expect(result2, EqualTo(data));

            Expect(DesEcb.Decrypt(DesEcb.Encrypt(data, string.Empty), string.Empty), EqualTo(data));
            Expect
                (DesEcb.Decrypt(DesEcb.Encrypt(Encoding.Default.GetBytes(string.Empty), string.Empty), string.Empty),
                 EqualTo(string.Empty));

            var bigData = Enumerable.Repeat<byte>(65, 20000).ToArray();
            Expect(DesEcb.Decrypt(DesEcb.Encrypt(bigData, key), key), EqualTo(bigData));
        }
    }
}
