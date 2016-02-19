using System.Linq;

namespace Suigetsu.Core.Extensions
{
    public static class TypeExtensions
    {
        public static bool In<T>(this T item, params T[] items)
        {
            return items != null && items.Contains(item);
        }
    }
}
