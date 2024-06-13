// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Predicates;

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
        predicates.AssertNotNull();

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

        if (collisions.Length > 0)
        {
            var message = $"Multiple predicates and/or projections defined for propert{(collisions.Length > 1 ? "ies" : "y")}: {string.Join("; ", collisions.Select(x => $"{x.Key.Type}.{x.Key.Property.Name}"))}";
            throw new ArgumentException(message, nameof(predicates));
        }

        _globalPredicates = predicates
            .Where(x => x is not ITypePredicate)
            .Where(x => x is not IPropertyPredicate)
            .Where(x => x is not IPropertyProjection)
            .ToArray();
    }

    public Expression Visit(Expression expression)
        => new Visitor(this).Run(expression);

    private sealed class Visitor : ExpressionVisitor
    {
        private sealed class Scope : IDisposable
        {
            private readonly Scope? _parent;
            private readonly Visitor _visitor;
            private readonly Dictionary<Expression, Expression> _substitutes;
            private readonly Dictionary<Expression, bool> _isSubstituted;
            private readonly List<Expression> _filterExpressions = [];
            private bool _isSelect = false;

            public Scope(Visitor visitor)
                : this(null, visitor)
            {
            }

            private Scope(Scope? parent, Visitor visitor)
            {
                _substitutes = new Dictionary<Expression, Expression>(ReferenceEqualityComparer<Expression>.Instance);
                _isSubstituted = new Dictionary<Expression, bool>(ReferenceEqualityComparer<Expression>.Instance);
                _parent = parent;
                _visitor = visitor.CheckNotNull();
                _visitor._scope = this;
            }

            public bool IsSelectScope => _isSelect || _parent?.IsSelectScope is true;

            public void Dispose() => _visitor._scope = _parent!;

            public Scope Push() => new(this, _visitor);

            public IDisposable PushSubstitute(Expression expression, MethodCallExpression node)
            {
                var scope = Push();
                scope._substitutes[expression] = node;
                return scope;
            }

            internal IDisposable PushMethodCall(MethodCallExpression node)
            {
                var scope = Push();
                scope._isSelect =
                    node.Method.DeclaringType == typeof(Queryable) &&
                    string.Equals(node.Method.Name, nameof(Queryable.Select), StringComparison.Ordinal);
                return scope;
            }

            public Expression GetSubstitute(Expression expression)
            {
                if (_substitutes.TryGetValue(expression, out Expression exp))
                {
                    _isSubstituted[expression] = true;
                    return exp;
                }

                return _parent?.GetSubstitute(expression) ?? expression;
            }

            public bool IsSubstituted(Expression expression)
                => _isSubstituted.ContainsKey(expression);

            public void PutFilterBeforeSelect(Expression filterExpression)
            {
                if (_isSelect)
                {
                    _filterExpressions.Add(filterExpression);
                }
                else
                {
                    if (_parent is null)
                    {
                        throw new Exception("Expected parent scope to exists.");
                    }

                    _parent.PutFilterBeforeSelect(filterExpression);
                }
            }

            public MethodCallExpression PrependFilters(MethodCallExpression node)
            {
                if (_isSelect && _filterExpressions.Count > 0)
                {
                    var quoteExpression = node.Arguments.Last() as UnaryExpression;
                    if (quoteExpression?.NodeType != ExpressionType.Quote)
                    {
                        throw new Exception("Unable to retrieve parameter");
                    }

                    var lambdaExpression = quoteExpression.Operand as LambdaExpression;
                    if (lambdaExpression is null)
                    {
                        throw new Exception($"Expected predicate of type {nameof(LambdaExpression)}.");
                    }

                    var parameter = lambdaExpression.Parameters.Single();

                    var select = node;
                    var queryable = select.Arguments.First();
                    var elementType = TypeHelper.GetElementType(queryable.Type) ?? queryable.Type;
                    var where = MethodInfos.Queryable.Where(elementType);
                    foreach (var filter in _filterExpressions)
                    {
                        var predicate = Expression.Lambda(filter, parameter);
                        queryable = Expression.Call(where, queryable, predicate);
                    }

                    var arguments = select.Arguments.ToList();
                    arguments[0] = queryable;
                    node = select.Update(null, arguments);
                }

                return node;
            }
        }

        private readonly PredicateExpressionVisitor _outer;
        private Scope _scope;

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

        protected override Expression VisitExtension(Expression node)
        {
            var expression = base.VisitExtension(node);

            var isSingleElement = false;
            var type = TypeHelper.GetElementType(expression.Type);
            if (type is null)
            {
                type = expression.Type;
                isSingleElement = true;
            }

            expression = ApplyTypeFilters(expression, type, isSingleElement);

            return expression;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (!ReferenceEquals(node.Object, null))
            {
                using (_scope.PushSubstitute(node.Object, node))
                {
                    var expression = Visit(node.Object);
                    if (_scope.IsSubstituted(node.Object))
                    {
                        // TODO: what about the method call and potential arguments?
                        return expression;
                    }
                }
            }

            using (_scope.PushMethodCall(node))
            {
                var method = node.Method;
                MethodCallExpression? exp = null;
                if (method.DeclaringType.Name.Contains("EntityFramework") &&
                    method.Name is "Include" or "ThenInclude" &&
                    method.IsGenericMethod)
                {
                    // fix-up method call for Include/ThenInclude if generic type argument changed

                    var instance = Visit(node.Object);
                    var arguments = node.VisitArguments(this);
                    if (ReferenceEquals(instance, node.Object) && arguments is null)
                    {
                        exp = node;
                    }
                    else
                    {
                        var parameters = method.GetParameters();
                        if (parameters.Length is 2 && arguments?.Count is 2)
                        {
                            var parameter2 = parameters[1].ParameterType;
                            var argument2 = arguments[1].Type;
                            if (parameter2 is not null &&
                                argument2 is not null &&

                                !parameter2.IsAssignableFrom(argument2) &&

                                parameter2.IsGenericType &&
                                parameter2.Implements(typeof(Expression<>), out var paramLambdaType) &&
                                paramLambdaType[0].Implements(typeof(Func<,>), out var paramFuncTypes) &&
                                paramFuncTypes[1].Implements(typeof(IEnumerable<>), out var paramElementType) &&

                                argument2.IsGenericType &&
                                argument2.Implements(typeof(Expression<>), out var lambdaType) &&
                                lambdaType[0].Implements(typeof(Func<,>), out var funcTypes) &&
                                funcTypes[1].Implements(typeof(IEnumerable<>), out var elementType) &&

                                paramFuncTypes[0] == funcTypes[0])
                            {
                                var types = method.Name is "Include"
                                    ? funcTypes // Include has two generic arguments
                                    : funcTypes.Prepend(method.GetGenericArguments()[0]).ToArray(); // ThenInclude has three generic arguments
                                method = method.GetGenericMethodDefinition().MakeGenericMethod(types);
                            }
                        }

                        exp = Expression.Call(instance, method, arguments);
                    }
                }
                else
                {
                    exp = (MethodCallExpression)base.VisitMethodCall(node);
                }

                return _scope.PrependFilters(exp);
            }
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            var body = Visit(node.Body);
            var parameters = node.VisitParameters(this);

            return ReferenceEquals(body, node.Body) && parameters is null
                ? node
                : Expression.Lambda(body, parameters ?? node.Parameters);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            var expression = base.VisitConstant(node);
            if (expression is ConstantExpression constantExpression)
            {
                var isSingleElement = false;
                var runtimeType = constantExpression.Value?.GetType();
                var type = TypeHelper.GetElementType(runtimeType) ?? TypeHelper.GetElementType(constantExpression.Type);
                if (type is null)
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
            if (type is null)
            {
                type = expression.Type;
                isSingleElement = true;
            }

            return ApplyTypeFilters(expression, type, isSingleElement);
        }

        private Expression ApplyTypeFilters(Expression expression, Type type, bool isSingleElement)
        {
            if (type is not null)
            {
                var typePredicates = GetTypePredicates(type).ToArray();
                var propertyProjections = GetPropertyProjections(type).ToArray();
                if (typePredicates.Length > 0 || propertyProjections.Length > 0)
                {
                    expression = _scope.GetSubstitute(expression);

                    foreach (var typeFilter in typePredicates)
                    {
                        if (isSingleElement && _scope.IsSelectScope)
                        {
                            var filterExpression = TypePredicateHelper.GetPredicate(typeFilter, expression);
                            _scope.PutFilterBeforeSelect(filterExpression);
                        }
                        else
                        {
                            expression = TypePredicateHelper.Apply(typeFilter, expression, type, isSingleElement);
                        }
                    }

                    if (propertyProjections.Length > 0)
                    {
                        expression = PropertyProjectionHelper.Apply(propertyProjections, expression, type);
                    }
                }
            }

            return expression;
        }
    }
}