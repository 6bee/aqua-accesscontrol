// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Predicates;

using System.Collections.Generic;
using System.Linq.Expressions;

internal sealed class ReplaceParameterExpressionVisitor : ExpressionVisitor
{
    private readonly IDictionary<ParameterExpression, Expression> _parameterMap;

    public ReplaceParameterExpressionVisitor(IDictionary<ParameterExpression, Expression> parameterMap)
        => _parameterMap = parameterMap.CheckNotNull();

    protected override Expression VisitParameter(ParameterExpression node)
        => _parameterMap.TryGetValue(node, out Expression expression) ? expression : node;
}