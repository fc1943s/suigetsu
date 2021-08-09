﻿using System;
using System.ComponentModel;
using System.Linq;
using Suigetsu.Core.Util;

namespace Suigetsu.Core.Extensions
{
    /// <summary>
    ///     Extensions for the <see cref="T:System.Type" /> class and generic objects.
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        ///     Tests if the given <see langword="object" /> is equal any parameter.
        /// </summary>
        public static bool In<T>(this T item, params T[] items) => items != null && items.Contains(item);

        /// <summary>
        ///     Tests if the given <paramref name="type" /> is an <see langword="enum" />, throwing otherwise.
        /// </summary>
        /// <exception cref="InvalidEnumArgumentException">The provided type is not an enum type.</exception>
        public static void ValidateEnumType(this Type type)
        {
            if(type == null || !type.IsEnum)
            {
                throw new InvalidEnumArgumentException($"The type '{type?.Name}' is not an enum type.");
            }
        }

        /// <summary>
        ///     Tries to instantiate the given value/primitive <paramref name="type" />.
        /// </summary>
        public static object Default(this Type type)
        {
            if(type == typeof(string))
            {
                return string.Empty;
            }

            if(type.IsEnum)
            {
                return EnumUtils.GetEmpty(type);
            }

            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}
