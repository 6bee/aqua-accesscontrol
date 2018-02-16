﻿// Copyright (c) Christof Senn. All rights reserved. 

namespace Aqua.AccessControl
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    internal static class TypeHelper
    {
        public static bool IsEnumerableType(Type type)
        {
            return typeof(string) != type
                && typeof(IEnumerable).IsAssignableFrom(type);
        }

        public static Type GetElementType(Type type)
        {
            var enumerableType = FindIEnumerable(type);
            return enumerableType?.GetGenericArguments().First();
        }

        private static Type FindIEnumerable(Type type)
        {
            if (type == null || type == typeof(string))
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
            if (interfaces != null && interfaces.Any())
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
            if (baseType != null && baseType != typeof(object))
            {
                return FindIEnumerable(baseType);
            }

            return null;
        }
    }
}
