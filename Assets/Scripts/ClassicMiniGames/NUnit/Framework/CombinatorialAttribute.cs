using NUnit.Framework.Internal;
using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class CombinatorialAttribute : PropertyAttribute
	{
		public CombinatorialAttribute()
			: base(PropertyNames.JoinType, "Combinatorial")
		{
		}
	}
}
