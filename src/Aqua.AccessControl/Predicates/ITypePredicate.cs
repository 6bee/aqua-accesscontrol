// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Predicates
{
    using System;
    using System.Linq.Expressions;

    public interface ITypePredicate : IPredicate
    {
        Type Type { get; }

        LambdaExpression Predicate { get; }
    }
}
