using NUnit.Framework.Internal;
using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class PairwiseAttribute : PropertyAttribute
	{
		public PairwiseAttribute()
			: base(PropertyNames.JoinType, "Pairwise")
		{
		}
	}
}
