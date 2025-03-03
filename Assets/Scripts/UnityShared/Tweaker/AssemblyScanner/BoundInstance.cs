using System;

namespace Tweaker.AssemblyScanner
{
	public class BoundInstance<T> : IBoundInstance where T : class
	{
		private readonly uint uniqueId;

		private readonly WeakReference weakReference;

		private static uint s_nextId = 1u;

		public object Instance
		{
			get
			{
				T result = null;
				if (weakReference.IsAlive)
				{
					result = (weakReference.Target as T);
				}
				return result;
			}
		}

		public uint UniqueId
		{
			get
			{
				return uniqueId;
			}
		}

		public Type Type
		{
			get
			{
				return typeof(T);
			}
		}

		public BoundInstance(T instance)
			: this(instance, s_nextId)
		{
			s_nextId++;
		}

		public BoundInstance(T instance, uint id)
		{
			weakReference = new WeakReference(instance);
			uniqueId = id;
		}
	}
}
