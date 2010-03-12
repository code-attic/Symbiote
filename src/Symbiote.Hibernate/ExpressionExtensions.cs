using System;
using System.Linq.Expressions;

namespace Symbiote.Hibernate
{
    public static class ExpressionExtensions
    {
        public static string GetMemberName<TType, TMember>(this Expression<Func<TType, TMember>> memberExpression)
        {
            var memberExpr = memberExpression.Body as MemberExpression;
            if (memberExpr != null)
            {
                return memberExpr.Member.Name;
            }

            throw new ArgumentException(
                string.Format("The lambda expression ({0}) is not a valid member expression.", memberExpression.Body.ToString()));
        }
    }
}