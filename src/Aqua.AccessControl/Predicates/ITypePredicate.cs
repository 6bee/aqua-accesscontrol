// Copyright (c) Christof Senn. All rights reserved. 

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
