// Copyright (c) Christof Senn. All rights reserved. 

namespace Aqua.AccessControl.Predicates
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    internal sealed class PredicateExpressionVisitor
    {
        private readonly IEnumerable<IPredicate> _globalPredicates;
        private readonly IEnumerable<ITypePredicate> _typePredicates;
        private readonly IEnumerable<IPropertyProjection> _propertyProjections;

        public PredicateExpressionVisitor(IEnumerable<IPredicate> predicates)
        {
            Assert.ArgumentNotNull(predicates, nameof(predicates));

            _typePredicates = predicates
                .OfType<ITypePredicate>()
                .ToArray();

            var projections = predicates
                .OfType<IPropertyPredicate>()
                .ToProjections()
                .ToArray();

            _propertyProjections = predicates
                .OfType<IPropertyProjection>()
                .Concat(projections)
                .ToArray();

            var collisions = _propertyProjections
                .GroupBy(x => new { x.Property, x.Type })
                .Where(x => x.Count() > 1)
                .ToArray();

            if (collisions.Any())
            {
                var message = $"Multiple predicates and/or projections defined for propert{(collisions.Count() > 1 ? "ies" : "y")}: {string.Join("; ", collisions.Select(x => $"{x.Key.Type}.{x.Key.Property.Name}"))}";
                throw new ArgumentException(message, nameof(predicates));
            }

            _globalPredicates = predicates
                .Where(x => !(x is ITypePredicate))
                .Where(x => !(x is IPropertyPredicate))
                .Where(x => !(x is IPropertyProjection))
                .ToArray();
        }

        public Expression Visit(Expression expression)
            => new Visitor(this).Run(expression);

        private sealed class Visitor : ExpressionVisitor
        {
            private sealed class Scope : IDisposable
            {
                private readonly Scope _parent;
                private readonly Visitor _visitor;
                private readonly Dictionary<Expression, Expression> _substitutes;

                public Scope(Visitor visitor)
                    : this(null, visitor)
                {
                }

                private Scope(Scope parent, Visitor visitor)
                {
                    _substitutes = new Dictionary<Expression, Expression>(ReferenceEqualityComparer<Expression>.Instance);
                    _parent = parent;
                    _visitor = visitor;
                    _visitor._scope = this;
                }

                public void Dispose()
                {
                    _visitor._scope = _parent;
                }

                internal IDisposable PushSubstitute(Expression expression, MethodCallExpression node)
                {
                    var scope = new Scope(this, _visitor);
                    scope._substitutes[expression] = node;
                    return scope;
                }

                public Expression GetSubstitute(Expression expression)
                {
                    if (_substitutes.TryGetValue(expression, out Expression exp))
                    {
                        IsSubstituted = true;
                        return exp;
                    }

                    return _parent?.GetSubstitute(expression) ?? expression;
                }

                public bool IsSubstituted { get; private set; }
            }

            private Scope _scope;
            private readonly PredicateExpressionVisitor _outer;

            public Visitor(PredicateExpressionVisitor outer)
            {
                _scope = new Scope(this);
                _outer = outer;
            }

            private IEnumerable<IPredicate> GetGlobalPredicates()
                => _outer._globalPredicates;

            private IEnumerable<ITypePredicate> GetTypePredicates(Type type)
                => _outer._typePredicates.Where(x => x.Type.IsAssignableFrom(type));

            private IEnumerable<IPropertyProjection> GetPropertyProjections(Type type)
                => _outer._propertyProjections.Where(x => x.Type.IsAssignableFrom(type)); // TODO: filter out collisions of subtypes and basetypes -> take most specific

            public Expression Run(Expression node)
            {
                var expression = Visit(node);

                foreach (var predicate in GetGlobalPredicates())
                {
                    expression = predicate.ApplyTo(expression);
                }

                return expression;
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (!ReferenceEquals(null, node.Object))
                {
                    using (_scope.PushSubstitute(node.Object, node))
                    {
                        var expression = Visit(node.Object);
                        if (_scope.IsSubstituted)
                        {
                            return expression;
                        }
                    }
                }

                var exp = (MethodCallExpression)base.VisitMethodCall(node);
                exp = AddWhereConditionIfRequired(exp);
                return exp;
            }

            private MethodCallExpression AddWhereConditionIfRequired(MethodCallExpression node)
            {
                if (node.Method.DeclaringType == typeof(Queryable) && string.Equals(node.Method.Name, nameof(Queryable.Select)))
                {
                    var quoteExpression = node.Arguments.Last() as UnaryExpression;
                    if (quoteExpression?.NodeType == ExpressionType.Quote)
                    {
                        var selectedType = (quoteExpression.Operand as LambdaExpression)?.ReturnType;
                        if (selectedType != null && GetTypePredicates(selectedType).Any())
                        {
                            var where = MethodInfos.Queryable.Where(selectedType);
                            var x = Expression.Parameter(selectedType, "x");
                            var notNull = Expression.Lambda(Expression.NotEqual(x, Expression.Constant(null, selectedType)), x);
                            return Expression.Call(where, node, notNull);
                        }
                    }
                }

                return node;
            }

            protected override Expression VisitConstant(ConstantExpression node)
            {
                var expression = base.VisitConstant(node);
                var constantExpression = expression as ConstantExpression;
                if (constantExpression != null)
                {
                    var isSingleElement = false;
                    var runtimeType = constantExpression.Value?.GetType();
                    var type = TypeHelper.GetElementType(runtimeType) ?? TypeHelper.GetElementType(constantExpression.Type);
                    if (type == null)
                    {
                        type = runtimeType ?? constantExpression.Type;
                        isSingleElement = true;
                    }

                    expression = ApplyTypeFilters(constantExpression, type, isSingleElement);
                }

                return expression;
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                var expression = base.VisitMember(node);

                var isSingleElement = false;
                var type = TypeHelper.GetElementType(expression.Type);
                if (type == null)
                {
                    type = expression.Type;
                    isSingleElement = true;
                }

                return ApplyTypeFilters(expression, type, isSingleElement);
            }

            private Expression ApplyTypeFilters(Expression expression, Type type, bool isSingleElement)
            {
                if (type != null)
                {
                    var typePredicates = GetTypePredicates(type).ToArray();
                    var propertyProjections = GetPropertyProjections(type).ToArray();
                    if (typePredicates.Any() || propertyProjections.Any())
                    {
                        expression = _scope.GetSubstitute(expression);

                        foreach (var typeFilter in typePredicates)
                        {
                            expression = TypePredicateHelper.Apply(typeFilter, expression, type, isSingleElement);
                        }

                        if (propertyProjections.Any())
                        {
                            expression = PropertyProjectionHelper.Apply(propertyProjections, expression, type);
                        }
                    }
                }

                return expression;
            }
        }
    }
}
