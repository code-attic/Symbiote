﻿using System;
using System.Linq;
using System.Linq.Expressions;

namespace Symbiote.Restfully.Impl
{
    public static class ExpressionExtensions
    {
        public static Expression<Action<T>> ChangeArgsToConstants<T>(this Expression<Action<T>> expression)
        {
            return Expression.Lambda<Action<T>>(
                ChangeArgsToConstants(expression.Body as MethodCallExpression),
                expression.TailCall,
                expression.Parameters);
        }

        public static Expression<Func<T, R>> ChangeArgsToConstants<T, R>(this Expression<Func<T, R>> expression)
        {
            return Expression.Lambda<Func<T, R>>(
                ChangeArgsToConstants(expression.Body as MethodCallExpression),
                expression.TailCall,
                expression.Parameters);
        }

        public static MethodCallExpression ChangeArgsToConstants(this MethodCallExpression expression)
        {
            var constantExpressions =
                expression
                    .Arguments
                    .Select(x => Expression.Constant(Expression.Lambda(x).Compile().DynamicInvoke(), x.Type));

            return Expression.Call(
                expression.Object,
                expression.Method,
                constantExpressions);
        }
    }
}
