using System;

namespace Tweaker.Core
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	public class TweakableAttribute : BaseTweakerAttribute
	{
		public TweakableAttribute(string name)
			: base(name)
		{
		}
	}
}
