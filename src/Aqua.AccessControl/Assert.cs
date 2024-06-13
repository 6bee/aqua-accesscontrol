// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl;

using System;
using System.Reflection;
using System.Runtime.CompilerServices;

internal static class Assert
{
    internal static MemberInfo PropertyInfoArgument(MemberInfo memberInfo, [CallerArgumentExpression("memberInfo")] string? parameterName = null)
    {
        memberInfo.AssertNotNull(parameterName);

        if (memberInfo is not PropertyInfo propertyInfo)
        {
            throw new ArgumentException($"Expected {parameterName} to be property selector");
        }

        if (!propertyInfo.CanRead)
        {
            throw new ArgumentException($"Property declared by {parameterName} may not be read from");
        }

        if (!propertyInfo.CanWrite)
        {
            throw new ArgumentException($"Property declared by {parameterName} may not be written to");
        }

        return memberInfo;
    }
}
