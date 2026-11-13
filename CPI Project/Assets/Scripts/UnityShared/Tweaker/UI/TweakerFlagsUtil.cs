using Tweaker.Core;

namespace Tweaker.UI
{
	public static class TweakerFlagsUtil
	{
		public static bool IsSet(TweakableUIFlags flag, ITweakerObject tweakable)
		{
			bool result = false;
			TweakableUIFlagsAttribute customAttribute = tweakable.GetCustomAttribute<TweakableUIFlagsAttribute>();
			if (customAttribute != null)
			{
				result = ((customAttribute.Flags & flag) == flag);
			}
			return result;
		}
	}
}
