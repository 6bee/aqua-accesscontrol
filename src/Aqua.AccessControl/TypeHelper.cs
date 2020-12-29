// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    internal static class TypeHelper
    {
        public static bool IsEnumerableType(Type type)
            => !ReferenceEquals(type, null)
            && type != typeof(string)
            && typeof(IEnumerable).IsAssignableFrom(type);

        public static Type? GetElementType(Type? type)
        {
            var enumerableType = FindIEnumerable(type);
            return enumerableType?.GetGenericArguments().First();
        }

        private static Type? FindIEnumerable(Type? type)
        {
            if (type is null || type == typeof(string))
            {
                return null;
            }

            if (type.IsArray)
            {
                return typeof(IEnumerable<>).MakeGenericType(type.GetElementType());
            }

            if (type.IsGenericType)
            {
                foreach (var arg in type.GetGenericArguments())
                {
                    var enumerableType = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (enumerableType.IsAssignableFrom(type))
                    {
                        return enumerableType;
                    }
                }
            }

            var interfaces = type.GetInterfaces();
            if (interfaces != null)
            {
                foreach (var interfaceType in interfaces)
                {
                    var enumerableType = FindIEnumerable(interfaceType);
                    if (enumerableType != null)
                    {
                        return enumerableType;
                    }
                }
            }

            var baseType = type.BaseType;
            if (baseType is null || baseType == typeof(object))
            {
                return null;
            }

            return FindIEnumerable(baseType);
        }
    }
}
