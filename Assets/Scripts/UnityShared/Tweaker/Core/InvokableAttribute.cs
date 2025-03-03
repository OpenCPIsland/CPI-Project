using System;

namespace Tweaker.Core
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Event, AllowMultiple = false)]
	public class InvokableAttribute : BaseTweakerAttribute
	{
		public InvokableAttribute(string name)
			: base(name)
		{
		}
	}
}
