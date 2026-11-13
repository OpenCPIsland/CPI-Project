using System;

namespace Tweaker.AssemblyScanner
{
	public class BoundInstanceFactory
	{
		public static IBoundInstance Create(object instance)
		{
			Type type = typeof(BoundInstance<>).MakeGenericType(instance.GetType());
			return (IBoundInstance)Activator.CreateInstance(type, instance);
		}

		public static IBoundInstance Create(object instance, uint id)
		{
			Type type = typeof(BoundInstance<>).MakeGenericType(instance.GetType());
			return (IBoundInstance)Activator.CreateInstance(type, instance, id);
		}
	}
}
