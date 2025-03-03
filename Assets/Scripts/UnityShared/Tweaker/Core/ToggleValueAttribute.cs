using System;

namespace Tweaker.Core
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true)]
	public class ToggleValueAttribute : NamedToggleValueAttribute
	{
		public ToggleValueAttribute(object value, uint order = 0u)
			: base(value.ToString(), value, order)
		{
		}
	}
}
