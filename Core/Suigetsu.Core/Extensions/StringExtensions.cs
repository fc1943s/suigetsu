namespace Suigetsu.Core.Extensions
{
    /// <summary>
    ///     Extensions for the <see cref="T:System.String" /> class and generic objects.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        ///     Shortcut for the <see cref="M:System.String.Format(System.String,System.Object[])" /> method.
        /// </summary>
        public static string FormatWith(this string format, params object[] args)
        {
            return string.Format(format, args);
        }
    }
}
