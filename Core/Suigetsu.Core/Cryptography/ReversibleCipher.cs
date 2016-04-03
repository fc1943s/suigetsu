using System.Linq;
using Suigetsu.Core.Extensions;

namespace Suigetsu.Core.Cryptography
{
    public static class ReversibleCipher
    {
        public static string Encrypt(string text, string key, int repeatCount = 1)
        {
            if(key.IsEmpty())
            {
                return text;
            }

            var result = string.Empty;
            var front = true;

            text = text.Reverse();

            for(int i = 0, j = 0; i < text.Length; i++)
            {
                var charCode = (40000 + text[i] + key[j] + j) % 40000;
                result += charCode.ToString("X4");

                front = j != key.Length - 1 && (j == 0 || front);
                j = (j + (front ? 1 : -1)) % key.Length;
            }

            result = result.ToLowerInvariant();
            if(repeatCount > 1)
            {
                result = Encrypt(result, key, repeatCount - 1);
            }
            return result;
        }

        public static string Decrypt(string text, string key, int repeatCount = 1)
        {
            if(key.IsEmpty())
            {
                return text;
            }

            var result = string.Empty;
            var front = true;

            var bytes = text.HexToDigits().ToList();

            for(int i = 0, j = 0; i < bytes.Count; i++)
            {
                var processedChar = ((40000 + bytes[i]) - key[j] - j) % 40000;
                result += (char)processedChar;

                front = j != key.Length - 1 && (j == 0 || front);
                j = (j + (front ? 1 : -1)) % key.Length;
            }

            result = result.Reverse();
            if(repeatCount > 1)
            {
                result = Decrypt(result, key, repeatCount - 1);
            }
            return result;
        }
    }
}
