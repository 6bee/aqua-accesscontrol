// Copyright (c) Christof Senn. All rights reserved. 

namespace Aqua.AccessControl.Predicates
{
    using System.Linq.Expressions;

    public interface IPredicate
    {
        Expression ApplyTo(Expression expression);
    }
}
