using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Suigetsu.Core.Extensions;

namespace Suigetsu.Core.Cryptography
{
    public static class DesEcb
    {
        public static IEnumerable<byte> Encrypt(IEnumerable<byte> data, string key, int repeatCount = 1)
        {
            return Encrypt(data, Encoding.Default.GetBytes(key), repeatCount);
        }

        public static IEnumerable<byte> Encrypt(IEnumerable<byte> data, IEnumerable<byte> key, int repeatCount = 1)
        {
            var keyEnumerable = key as byte[] ?? key.ToArray();
            var dataEnumerable = data as byte[] ?? data.ToArray();

            if(keyEnumerable.IsEmpty() || dataEnumerable.IsEmpty())
            {
                return dataEnumerable;
            }

            DES desEncrypt = new DESCryptoServiceProvider();
            desEncrypt.Mode = CipherMode.ECB;
            desEncrypt.Key = keyEnumerable;
            var encryptedStream = new MemoryStream();
            var cryptoStream = new CryptoStream(encryptedStream, desEncrypt.CreateEncryptor(), CryptoStreamMode.Write);
            cryptoStream.Write(dataEnumerable, 0, dataEnumerable.Length);
            cryptoStream.FlushFinalBlock();
            IEnumerable<byte> result = encryptedStream.ToArray();
            if(repeatCount > 1)
            {
                result = Encrypt(result, keyEnumerable, repeatCount - 1);
            }
            return result;
        }

        public static IEnumerable<byte> Decrypt(IEnumerable<byte> data, string key, int repeatCount = 1)
        {
            return Decrypt(data, Encoding.Default.GetBytes(key), repeatCount);
        }

        public static IEnumerable<byte> Decrypt(IEnumerable<byte> data, IEnumerable<byte> key, int repeatCount = 1)
        {
            var keyEnumerable = key as byte[] ?? key.ToArray();
            var dataEnumerable = data as byte[] ?? data.ToArray();

            if(keyEnumerable.IsEmpty() || dataEnumerable.IsEmpty())
            {
                return dataEnumerable;
            }

            DES desDecrypt = new DESCryptoServiceProvider();
            desDecrypt.Mode = CipherMode.ECB;

            desDecrypt.Key = keyEnumerable.ToArray();
            var decryptedStream = new MemoryStream();
            var cryptoStream = new CryptoStream(decryptedStream, desDecrypt.CreateDecryptor(), CryptoStreamMode.Write);

            cryptoStream.Write(dataEnumerable.ToArray(), 0, dataEnumerable.Length);
            cryptoStream.FlushFinalBlock();

            IEnumerable<byte> result = decryptedStream.ToArray();
            if(repeatCount > 1)
            {
                result = Decrypt(result, keyEnumerable, repeatCount - 1);
            }
            return result;
        }
    }
}
