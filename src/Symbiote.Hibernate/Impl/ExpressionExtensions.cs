/* 
Copyright 2008-2010 Alex Robson

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

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