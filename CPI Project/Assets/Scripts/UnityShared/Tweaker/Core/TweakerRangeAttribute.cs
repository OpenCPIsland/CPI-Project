using System;

namespace Tweaker.Core
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
	public class TweakerRangeAttribute : Attribute
	{
		public object MinValue;

		public object MaxValue;

		public TweakerRangeAttribute(object minValue, object maxValue)
		{
			MinValue = minValue;
			MaxValue = maxValue;
		}
	}
}
