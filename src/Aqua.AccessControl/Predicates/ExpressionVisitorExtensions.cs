// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Predicates;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;

[EditorBrowsable(EditorBrowsableState.Never)]
internal static class ExpressionVisitorExtensions
{
    /// <summary>
    /// Visits method arguments.
    /// </summary>
    /// <remarks>
    /// Returns <see langword="null"/> if none of the arguments changed.
    /// </remarks>
    public static ReadOnlyCollection<Expression>? VisitArguments(this MethodCallExpression node, ExpressionVisitor visitor)
    {
        var arguments = visitor.VisitExpressionList(node.Arguments);
        return ReferenceEquals(arguments, node.Arguments)
            ? null
            : arguments;
    }

    /// <summary>
    /// Visits lambda parameters.
    /// </summary>
    /// <remarks>
    /// Returns <see langword="null"/> if none of the parameters changed.
    /// </remarks>
    public static ReadOnlyCollection<ParameterExpression>? VisitParameters<T>(this Expression<T> node, ExpressionVisitor visitor)
    {
        var parameters = visitor.VisitExpressionList(node.Parameters);
        return ReferenceEquals(parameters, node.Parameters)
            ? null
            : parameters;
    }

    /// <summary>
    /// Visits expressions.
    /// </summary>
    /// <remarks>
    /// If none of the arguments changed, the original list is returned.
    /// </remarks>
    public static ReadOnlyCollection<T> VisitExpressionList<T>(this ExpressionVisitor visitor, ReadOnlyCollection<T> list)
        where T : Expression
    {
        visitor.AssertNotNull();
        list.AssertNotNull();

        List<T>? visited = null;
        for (int i = 0, n = list.Count; i < n; i++)
        {
            var p = (T)visitor.Visit(list[i]);
            if (visited is not null)
            {
                visited.Add(p);
            }
            else if (p != list[i])
            {
                visited = new List<T>(n);
                for (int j = 0; j < i; j++)
                {
                    visited.Add(list[j]);
                }

                visited.Add(p);
            }
        }

        return visited?.AsReadOnly() ?? list;
    }
}