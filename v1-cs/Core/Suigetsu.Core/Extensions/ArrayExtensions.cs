using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Suigetsu.Core.Extensions
{
    /// <summary>
    ///     Extensions for the <see cref="T:System.Array" /> class.
    /// </summary>
    public static class ArrayExtensions
    {
        /// <summary>
        ///     Converts arrays between compatible types.
        /// </summary>
        public static T[] Convert<T>(this Array array)
        {
            var result = new T[array.Length];

            var fromType = array.GetType().GetElementType();
            var toType = typeof(T);

            // Conversion between primitive types allow us to directly copy data through memory
            if(toType.IsPrimitive && fromType.IsPrimitive)
            {
                var fromSize = Marshal.SizeOf(fromType);

                if(fromSize != Marshal.SizeOf(toType))
                {
                    // When the types have different sizes, we need to copy item by item
                    for(var i = 0; i < array.Length; i++)
                    {
                        var item = new T[1];
                        Buffer.BlockCopy(array, i * fromSize, item, 0, 1);
                        result[i] = item[0];
                    }
                }
                else
                {
                    Buffer.BlockCopy(array, 0, result, 0, fromSize * result.Length);
                }
            }
            else
            {
                result = (from object v in array
                          select (T)System.Convert.ChangeType(v, typeof(T))).ToArray();
            }

            return result;
        }
    }
}
