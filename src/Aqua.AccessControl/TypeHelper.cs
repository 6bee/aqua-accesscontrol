// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

internal static class TypeHelper
{
    public static bool IsEnumerableType(Type type)
        => type is not null
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
        if (interfaces is not null)
        {
            foreach (var interfaceType in interfaces)
            {
                var enumerableType = FindIEnumerable(interfaceType);
                if (enumerableType is not null)
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

    /// <summary>
    /// Returns <see langword="true"/> if the give <paramref name="type"/> is assignable to the <paramref name="interfaceType"/> specified.
    /// </summary>
    /// <param name="type">The type to be examined.</param>
    /// <param name="interfaceType">The actualy type to be checked for.</param>
    /// <param name="genericTypeArguments">Out parameter with array of generic argument types, in case <paramref name="interfaceType"/> is an open generic type.</param>
    public static bool Implements(this Type type, Type interfaceType, [NotNullWhen(true)] out Type[]? genericTypeArguments)
    {
        type.AssertNotNull();
        interfaceType.AssertNotNull();
        var typeArgs = new Type[1][];
        if (type.Implements(interfaceType, typeArgs))
        {
            genericTypeArguments = typeArgs[0];
            return true;
        }

        genericTypeArguments = null;
        return false;
    }

    private static bool Implements(this Type type, Type interfaceType, Type[][] typeArgs)
    {
        var isAssignableFromSpecifiedInterface = interfaceType.IsGenericTypeDefinition
            ? IsAssignableToGenericTypeDefinition(interfaceType, typeArgs)
            : interfaceType.IsGenericType
            ? IsAssignableToGenericType(interfaceType, typeArgs)
            : interfaceType.IsAssignableFrom;

        return GetTypeHierarchy(type).Any(isAssignableFromSpecifiedInterface)
            || type.GetInterfaces().Any(isAssignableFromSpecifiedInterface);

        static IEnumerable<Type> GetTypeHierarchy(Type? t)
        {
            while (t is not null)
            {
                yield return t;
                t = t.BaseType;
            }
        }

        static Func<Type, bool> IsAssignableToGenericTypeDefinition(Type interfaceTypeInfo, Type[][] typeArgs)
        {
            return i =>
            {
                var isAssignable = false;
                if (i.IsGenericType)
                {
                    var typeDef = i.IsGenericTypeDefinition ? i : i.GetGenericTypeDefinition();
                    isAssignable = typeDef == interfaceTypeInfo;
                }

                if (isAssignable)
                {
                    typeArgs[0] = i.GenericTypeArguments;
                }

                return isAssignable;
            };
        }

        static Func<Type, bool> IsAssignableToGenericType(Type interfaceTypeInfo, Type[][] typeArgs)
        {
            var interfaceTypeDefinition = interfaceTypeInfo.GetGenericTypeDefinition();
            var interfaceGenericArguments = interfaceTypeInfo.GetGenericArguments();

            return i =>
            {
                if (i.IsGenericType && !i.IsGenericTypeDefinition)
                {
                    var typeDefinition = i.GetGenericTypeDefinition();
                    if (typeDefinition == interfaceTypeDefinition)
                    {
                        var genericArguments = i.GetGenericArguments();
                        var allArgumentsAreAssignable = Enumerable.Range(0, genericArguments.Length - 1)
                            .All(index => Implements(genericArguments[index], interfaceGenericArguments[index], typeArgs));
                        if (allArgumentsAreAssignable)
                        {
                            return true;
                        }
                    }
                }

                return false;
            };
        }
    }
}