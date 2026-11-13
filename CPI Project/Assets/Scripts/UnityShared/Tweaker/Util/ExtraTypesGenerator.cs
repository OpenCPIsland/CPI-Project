using System;
using System.Reflection;
using Tweaker.Core;

namespace Tweaker.Util
{
	public static class ExtraTypesGenerator
	{
		public static void GenerateExtraTypes<T>()
		{
			TweakableInfo<T> tweakableInfo = new TweakableInfo<T>(null, null, null, null, 0u, null, null);
			TweakableNamedToggleValue<T> tweakableNamedToggleValue = new TweakableNamedToggleValue<T>(null, default(T));
			TweakableToggleValue<T> tweakableToggleValue = new TweakableToggleValue<T>(default(T));
			BaseTweakable<T> baseTweakable = new BaseTweakable<T>((TweakableInfo<T>)null, (PropertyInfo)null, (WeakReference)null);
			VirtualField<T> virtualField = new VirtualField<T>();
			VirtualProperty<T> virtualProperty = new VirtualProperty<T>(null, null);
		}
	}
}
