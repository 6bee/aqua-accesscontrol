// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl
{
    using System;
    using System.Linq;
    using System.Reflection;

    internal static class MethodInfos
    {
        internal static class Enumerable
        {
            private static readonly MethodInfo _castMethodInfo = typeof(System.Linq.Enumerable).GetMethods()
                .Single(x => string.Equals(x.Name, nameof(System.Linq.Enumerable.Cast)));

            private static readonly MethodInfo _firstOrDefaultMethodInfo = typeof(System.Linq.Enumerable).GetMethods()
                .Where(x => string.Equals(x.Name, nameof(System.Linq.Enumerable.FirstOrDefault)))
                .Single(x => x.GetParameters().Length == 1);

            private static readonly MethodInfo _selectMethodInfo = typeof(System.Linq.Enumerable).GetMethods()
                .Where(x => string.Equals(x.Name, nameof(System.Linq.Enumerable.Select)))
                .Single(x => x.GetParameters()[1].ParameterType.GenericTypeArguments.Length == 2);

            private static readonly MethodInfo _singleOrDefaultMethodInfo = typeof(System.Linq.Enumerable).GetMethods()
                .Where(x => string.Equals(x.Name, nameof(System.Linq.Enumerable.SingleOrDefault)))
                .Single(x => x.GetParameters().Length == 1);

            private static readonly MethodInfo _whereMethodInfo = typeof(System.Linq.Enumerable).GetMethods()
                .Where(x => string.Equals(x.Name, nameof(System.Linq.Enumerable.Where)))
                .Single(x => x.GetParameters()[1].ParameterType.GenericTypeArguments.Length == 2);

            public static MethodInfo Cast(Type t) => _castMethodInfo.MakeGenericMethod(t);

            public static MethodInfo FirstOrDefault(Type t) => _firstOrDefaultMethodInfo.MakeGenericMethod(t);

            public static MethodInfo Select(Type tSource, Type tResult) => _selectMethodInfo.MakeGenericMethod(tSource, tResult);

            public static MethodInfo SingleOrDefault(Type t) => _singleOrDefaultMethodInfo.MakeGenericMethod(t);

            public static MethodInfo Where(Type t) => _whereMethodInfo.MakeGenericMethod(t);
        }

        internal static class Queryable
        {
            private static readonly MethodInfo _castMethodInfo = typeof(System.Linq.Queryable).GetMethods()
                .Single(x => string.Equals(x.Name, nameof(System.Linq.Queryable.Cast)));

            private static readonly MethodInfo _selectMethodInfo = typeof(System.Linq.Queryable).GetMethods()
                .Where(x => string.Equals(x.Name, nameof(System.Linq.Queryable.Select)))
                .Single(x => x.GetParameters()[1].ParameterType.GenericTypeArguments[0].GenericTypeArguments.Length == 2);

            private static readonly MethodInfo _whereMethodInfo = typeof(System.Linq.Queryable).GetMethods()
                .Where(x => string.Equals(x.Name, nameof(System.Linq.Queryable.Where)))
                .Single(x => x.GetParameters()[1].ParameterType.GenericTypeArguments[0].GenericTypeArguments.Length == 2);

            public static MethodInfo Cast(Type t) => _castMethodInfo.MakeGenericMethod(t);

            public static MethodInfo Select(Type tSource, Type tResult) => _selectMethodInfo.MakeGenericMethod(tSource, tResult);

            public static MethodInfo Where(Type t) => _whereMethodInfo.MakeGenericMethod(t);
        }
    }
}
