// Copyright (c) Christof Senn. All rights reserved. 

namespace Aqua.AccessControl
{
    using System;
    using System.Reflection;

    internal static class Assert
    {
        public static T ArgumentNotNull<T>(T value, string argumentName, string message = null)
        {
            if (ReferenceEquals(null, value))
            {
                if (message == null)
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

        internal static MemberInfo PropertyInfoArgument(MemberInfo memberInfo, string parameterName)
        {
            var propertyInfo = memberInfo as PropertyInfo;
            if (propertyInfo == null)
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
