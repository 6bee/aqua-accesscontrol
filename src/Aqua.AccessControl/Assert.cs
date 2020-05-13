// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl
{
    using System;
    using System.Reflection;

    internal static class Assert
    {
#nullable disable
        public static T ArgumentNotNull<T>(T value, string argumentName, string message = null)
        {
            if (ReferenceEquals(null, value))
            {
                if (message is null)
                {
                    throw new ArgumentNullException(argumentName);
                }
                else
                {
                    throw new ArgumentNullException(argumentName, message);
                }
            }

            return value;
        }
#nullable restore

        internal static MemberInfo PropertyInfoArgument(MemberInfo memberInfo, string parameterName)
        {
            var propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo is null)
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
}
