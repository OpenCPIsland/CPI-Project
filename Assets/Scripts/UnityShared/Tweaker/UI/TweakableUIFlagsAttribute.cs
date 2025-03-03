using System;
using Tweaker.Core;

namespace Tweaker.UI
{
	public class TweakableUIFlagsAttribute : Attribute, ICustomTweakerAttribute
	{
		public readonly TweakableUIFlags Flags;

		public TweakableUIFlagsAttribute(TweakableUIFlags flags)
		{
			Flags = flags;
		}
	}
}
