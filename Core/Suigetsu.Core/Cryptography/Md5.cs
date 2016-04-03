using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Suigetsu.Core.Common;
using Suigetsu.Core.Extensions;

namespace Suigetsu.Core.Cryptography
{
    public static class Md5
    {
        public static string FromFile(string path, int repeatCount = 1)
        {
            if(!File.Exists(path))
            {
                return string.Empty;
            }

            using(var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                var hash = FromStream(stream);
                if(repeatCount > 1)
                {
                    hash = FromString(hash, repeatCount - 1);
                }
                return hash;
            }
        }

        public static string Salt(string text, string salt = null, int repeatCount = 1)
        {
            for(var i = 0; i < repeatCount; i++)
            {
                text = FromString(FromString(FromString(salt.IsEmpty() ? text : salt).Reverse()) + FromString(text));
            }

            return text;
        }

        public static string Salt(string text, int repeatCount) => Salt(text, null, repeatCount);

        private static string FromStream(Stream stream)
        {
            using(var md5 = MD5.Create())
                return FormatHash(md5.ComputeHash(stream));
        }

        private static string FormatHash(IEnumerable<byte> hash)
        {
            var sb = new StringBuilder();
            foreach(var t in hash)
            {
                sb.Append(t.ToString("X2"));
            }

            return sb.ToString().ToLowerInvariant();
        }

        private static string FromBytes(byte[] bytes)
        {
            using(var md5 = MD5.Create())
                return FormatHash(md5.ComputeHash(bytes));
        }

        public static string FromString(string text, int repeatCount = 1)
        {
            for(var i = 0; i < repeatCount; i++)
            {
                text = FromBytes(Encoding.ASCII.GetBytes(text));
            }

            return text;
        }

        public static bool IsValid(string md5) => Regex.HasMatch(md5, "^([A-Fa-f0-9]{32})$");
    }
}
