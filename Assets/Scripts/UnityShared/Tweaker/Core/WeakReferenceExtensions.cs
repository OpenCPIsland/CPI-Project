using System;

namespace Tweaker.Core
{
	public static class WeakReferenceExtensions
	{
		public static bool TryGetTarget(this WeakReference weak, out object target)
		{
			if (weak.IsAlive)
			{
				target = weak.Target;
				return true;
			}
			target = null;
			return false;
		}
	}
}
