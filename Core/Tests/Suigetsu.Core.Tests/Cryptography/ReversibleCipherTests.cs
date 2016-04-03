using NUnit.Framework;
using Suigetsu.Core.Cryptography;

namespace Suigetsu.Core.Tests.Cryptography
{
    [TestFixture]
    public class ReversibleCipherTests : AssertionHelper
    {
        [Test]
        public void DecryptTest()
        {
            Expect
                (ReversibleCipher.Decrypt("00b400ca00ac008e00bd00c700ba00d000cb00c800b900b70094", "SUIGETSU"),
                 EqualTo("ImportantData"));
            Expect
                (ReversibleCipher.Decrypt
                     ("0087008f007b007a008000bb0089008c009200bb0079007a008300b90083008600ad00ad00790089008900c00089008900aa00ac007b0086008a00b9007b007a00ad00bb0089008c00be00910079007a00ae00b70083008600ac00ad00790089008d00be00890089",
                      "SUIGETSU",
                      2),
                 EqualTo("ImportantData"));
            Expect(ReversibleCipher.Decrypt("TEST", string.Empty), EqualTo("TEST"));
        }

        [Test]
        public void EncryptDecryptTest()
        {
            var text = string.Empty;
            for(var i = 1; i <= 700; i++)
            {
                text += new string(new[] { (char)i });
            }

            var key = text.Substring(text.Length / 2);
            var hash = ReversibleCipher.Encrypt(text, key, 2);
            var revertedText = ReversibleCipher.Decrypt(hash, key, 2);
            Expect(revertedText, EqualTo(text));
        }

        [Test]
        public void EncryptTest()
        {
            Expect
                (ReversibleCipher.Encrypt("ImportantData", "SUIGETSU"),
                 EqualTo("00b400ca00ac008e00bd00c700ba00d000cb00c800b900b70094"));

            Expect
                (ReversibleCipher.Encrypt("ImportantData", "SUIGETSU", 2),
                 EqualTo
                     ("0087008f007b007a008000bb0089008c009200bb0079007a008300b90083008600ad00ad00790089008900c00089008900aa00ac007b0086008a00b9007b007a00ad00bb0089008c00be00910079007a00ae00b70083008600ac00ad00790089008d00be00890089"));
            Expect(ReversibleCipher.Encrypt("TEST", string.Empty), EqualTo("TEST"));
        }
    }
}
