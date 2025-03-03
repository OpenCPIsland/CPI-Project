using System;

namespace Tweaker.Core
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
	public class StepSizeAttribute : Attribute
	{
		public object Size;

		public StepSizeAttribute(object size)
		{
			Size = size;
		}
	}
}
