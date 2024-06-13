// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Predicates;

using System;
using System.Linq.Expressions;
using System.Reflection;

public interface IPropertyPredicate : IPredicate
{
    Type Type { get; }

    MemberInfo Property { get; }

    Type PropertyType { get; }

    LambdaExpression Predicate { get; }
}