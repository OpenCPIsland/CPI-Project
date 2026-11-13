using System;
using System.Linq.Expressions;

namespace Disney.Kelowna.Common
{
	public static class FieldNameGetter
	{
		public static string Get<T, TMember>(Expression<Func<T, TMember>> expression)
		{
			return ((MemberExpression)expression.Body).Member.Name;
		}
	}
}
