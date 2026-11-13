using NUnit.Framework.Internal;
using System;

namespace NUnit.Framework
{
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public sealed class DescriptionAttribute : PropertyAttribute
	{
		public DescriptionAttribute(string description)
			: base(PropertyNames.Description, description)
		{
		}
	}
}
