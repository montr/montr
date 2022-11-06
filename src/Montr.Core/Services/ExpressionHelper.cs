using System;
using System.Linq.Expressions;

namespace Montr.Core.Services
{
	public static class ExpressionHelper
	{
		public static string GetFullName<T>(Expression<Func<T, string>> expr)
		{
			return $"{typeof(T).FullName}.{GetMemberName(expr)}";
		}

		public static string GetMemberName<T>(Expression<Func<T, object>> expr)
		{
			return GetMemberName((Expression)expr);
		}

		public static string GetMemberName(Expression expression)
		{
			if (!(expression is LambdaExpression lambda)) throw new ArgumentNullException(nameof(expression));

			MemberExpression memberExpression = null;

			if (lambda.Body.NodeType == ExpressionType.Convert)
			{
				memberExpression = ((UnaryExpression)lambda.Body).Operand as MemberExpression;
			}
			else if (lambda.Body.NodeType == ExpressionType.MemberAccess)
			{
				memberExpression = lambda.Body as MemberExpression;
			}

			if (memberExpression == null)
			{
				throw new ArgumentException(nameof(expression));
			}

			return memberExpression.Member.Name;
		}
	}
}
