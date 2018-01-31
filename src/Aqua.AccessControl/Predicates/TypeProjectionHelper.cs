// Copyright (c) Christof Senn. All rights reserved. 

namespace Aqua.AccessControl.Predicates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal static class TypePredicateHelper
    {
        internal static Expression Apply(ITypePredicate typePredicate, Expression expression, Type type, bool isSingleElement)
        {
            if (isSingleElement)
            {
                var memberAccess = expression as MemberExpression;
                if (memberAccess == null)
                {
                    throw new NotSupportedException($"{expression.NodeType} expression for single element is not supported");
                }

                var propertyProjection = PredicateToProjection(memberAccess.Expression, memberAccess.Member, memberAccess.Expression.Type, memberAccess.Type, typePredicate.Predicate);
                return propertyProjection;
            }
            else
            {
                var isQueryable = typeof(IQueryable).IsAssignableFrom(expression.Type);

                var whereExpression = Expression.Call(GetWhereMethodInfo(isQueryable, typePredicate.Type), expression, typePredicate.Predicate);
                if (whereExpression.Type == expression.Type)
                {
                    return whereExpression;
                }

                var castExpression = Expression.Call(GetCastMethodInfo(isQueryable, type), whereExpression);
                return castExpression;
            }
        }

        private static MethodInfo GetWhereMethodInfo(bool isQueryable, Type type)
            => isQueryable
                ? MethodInfos.Queryable.Where(type)
                : MethodInfos.Enumerable.Where(type);

        private static MethodInfo GetCastMethodInfo(bool isQueryable, Type type)
            => isQueryable
                ? MethodInfos.Queryable.Cast(type)
                : MethodInfos.Enumerable.Cast(type);

        private static Expression PredicateToProjection(Expression expression, MemberInfo property, Type entityType, Type propertyType, LambdaExpression predicate)
        {
            // (T t) => predicate(t.p) ? t.p : default(P)
            var memberAccess = Expression.PropertyOrField(expression, property.Name);
            var parameterMap = new Dictionary<ParameterExpression, Expression>() { { predicate.Parameters.Single(), memberAccess } };
            var parameterReplacer = new ReplaceParameterExpressionVisitor(parameterMap);
            var test = parameterReplacer.Visit(predicate.Body);
            var ifTrue = memberAccess;
            var defaultValue = Expression.Lambda(Expression.Default(propertyType)).Compile().DynamicInvoke();
            var ifFalse = Expression.Constant(defaultValue, propertyType);
            return Expression.Condition(test, ifTrue, ifFalse, propertyType);
        }
    }
}
