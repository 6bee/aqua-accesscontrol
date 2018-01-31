// Copyright (c) Christof Senn. All rights reserved. 

namespace Aqua.AccessControl.Predicates
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    public interface IPropertyProjection : IPredicate
    {
        Type Type { get; }

        MemberInfo Property { get; }

        Type PropertyType { get; }

        LambdaExpression Projection { get; }
    }
}
