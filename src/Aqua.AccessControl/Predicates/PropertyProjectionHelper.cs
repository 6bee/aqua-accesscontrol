// Copyright (c) Christof Senn. All rights reserved. 

namespace Aqua.AccessControl.Predicates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal static class PropertyProjectionHelper
    {
        private sealed class MemberEqualityComparer : IEqualityComparer<MemberInfo>
        {
            public static readonly MemberEqualityComparer Instance = new MemberEqualityComparer();

            private MemberEqualityComparer()
            {
            }

            public bool Equals(MemberInfo x, MemberInfo y)
                => CreateObj(x).Equals(CreateObj(y));

            public int GetHashCode(MemberInfo obj)
                => CreateObj(obj).GetHashCode();

            private static object CreateObj(MemberInfo m)
                => new
                {
                    m.MemberType,
                    m.Name,
                    m.DeclaringType
                };
        }

        internal static Expression Apply(IEnumerable<IPropertyProjection> propertyProjections, Expression expression, Type type)
        {
            var projections = propertyProjections.ToDictionary(x => x.Property, x => x.Projection, MemberEqualityComparer.Instance);
            var lambda = GetTypeProjection(type, projections);
            return Expression.Call(GetMethodInfo(expression, type), expression, lambda);
        }

        private static LambdaExpression GetTypeProjection(Type type, IDictionary<MemberInfo, LambdaExpression> projections)
        {
            var parameterExpression = Expression.Parameter(type, projections.First().Value.Parameters.Single().Name);

            var parameterMap = projections
                .Select(x => x.Value)
                .ToDictionary(x => x.Parameters.Single(), x => (Expression)parameterExpression);

            var parameterReplacer = new ReplaceParameterExpressionVisitor(parameterMap);

            var bindings = new List<MemberBinding>();
            foreach (var p in type.GetProperties().Where(x => x.CanRead && x.CanWrite))
            {
                var propertyExpression = projections.TryGetValue(p, out LambdaExpression projection)
                    ? parameterReplacer.Visit(projection.Body)
                    : Expression.Property(parameterExpression, p);

                bindings.Add(Expression.Bind(p, propertyExpression));
            }

            var newExpression = Expression.New(type);
            var initializer = Expression.MemberInit(newExpression, bindings);
            return Expression.Lambda(initializer, parameterExpression);
        }

        private static MethodInfo GetMethodInfo(Expression expression, Type elementType)
            => typeof(IQueryable).IsAssignableFrom(expression.Type)
                ? MethodInfos.Queryable.Select(elementType, elementType)
                : MethodInfos.Enumerable.Select(elementType, elementType);

        internal static IEnumerable<IPropertyProjection> ToProjections(this IEnumerable<IPropertyPredicate> predicates)
        {
            return predicates
                .GroupBy(x => new { x.Property, x.PropertyType, x.Type })
                .Select(g =>
                {
                    var type = g.Key.Type;
                    var property = g.Key.Property;
                    var propertyType = g.Key.PropertyType;
                    var list = g.Select(x => x.Predicate).ToArray();
                    var projection = PredicateToProjection(property, type, propertyType, list);
                    return new PropertyProjection(property, type, propertyType, projection);
                })
                .ToArray();
        }

        private static LambdaExpression PredicateToProjection(MemberInfo property, Type entityType, Type propertyType, IEnumerable<LambdaExpression> predicates)
        {
            // (T t) => predicate(t) ? t.p : default(P)
            var parameter = Expression.Parameter(entityType, predicates.First().Parameters.Single().Name);
            var parameterMap = predicates.ToDictionary(x => x.Parameters.Single(), x => (Expression)parameter);
            var parameterReplacer = new ReplaceParameterExpressionVisitor(parameterMap);
            var test = predicates.Select(x => parameterReplacer.Visit(x.Body)).Aggregate(Expression.AndAlso);
            var ifTrue = Expression.PropertyOrField(parameter, property.Name);
            var defaultValue = Expression.Lambda(Expression.Default(propertyType)).Compile().DynamicInvoke();
            var ifFalse = Expression.Constant(defaultValue, propertyType);
            var ifThenElse = Expression.Condition(test, ifTrue, ifFalse, propertyType);
            return Expression.Lambda(ifThenElse, parameter);
        }
    }
}