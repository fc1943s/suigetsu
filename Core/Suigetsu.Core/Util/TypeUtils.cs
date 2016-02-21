using Suigetsu.Core.Extensions;

namespace Suigetsu.Core.Util
{
    /// <summary>
    ///     Utility methods for the <see cref="T:System.Type" /> class and generic objects.
    /// </summary>
    public static class TypeUtils
    {
        /// <summary>
        ///     <para>Tries to instantiate the given value/primitive type.</para>
        ///     Shortcut for: <seealso cref="M:Suigetsu.Core.Extensions.TypeExtensions.Default(System.Type)" />
        /// </summary>
        public static T Default<T>()
        {
            return (T)typeof(T).Default();
        }
    }
}
