using System.Runtime.Remoting.Metadata.W3cXsd2001;

namespace Suigetsu.Core.Extensions
{
    /// <summary>
    ///     Extensions for the <see cref="T:System.Byte" /> class.
    /// </summary>
    public static class ByteExtensions
    {
        /// <summary>
        ///     Converts an array of byte into a concatenated lowercase hexadecimal string.
        /// </summary>
        /// <returns></returns>
        public static string ByteArrayToString(this byte[] data)
        {
            return new SoapHexBinary(data).ToString().ToLower();
        }
    }
}
