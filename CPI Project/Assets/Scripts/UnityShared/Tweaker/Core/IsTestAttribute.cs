using System;

namespace Tweaker.Core
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event, AllowMultiple = false)]
	public class IsTestAttribute : Attribute, ICustomTweakerAttribute
	{
	}
}
