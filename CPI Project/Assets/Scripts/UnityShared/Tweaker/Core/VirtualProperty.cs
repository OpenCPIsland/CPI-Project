using System;
using System.Reflection;

namespace Tweaker.Core
{
	public class VirtualProperty<T>
	{
		private readonly FieldInfo fieldInfo;

		private readonly WeakReference weakReference;

		private readonly Action<T> setter;

		private readonly Func<T> getter;

		public WeakReference WeakInstance
		{
			get
			{
				return weakReference;
			}
		}

		public Action<T> Setter
		{
			get
			{
				return setter;
			}
		}

		public Func<T> Getter
		{
			get
			{
				return getter;
			}
		}

		public object StrongInstance
		{
			get
			{
				if (weakReference == null)
				{
					return null;
				}
				object target = null;
				WeakInstance.TryGetTarget(out target);
				return target;
			}
		}

		public bool IsValid
		{
			get
			{
				return WeakInstance == null || StrongInstance != null;
			}
		}

		public VirtualProperty(FieldInfo field, WeakReference instance)
		{
			fieldInfo = field;
			weakReference = instance;
			setter = SetValue;
			getter = GetValue;
		}

		private void SetValue(T value)
		{
			fieldInfo.SetValue(StrongInstance, value);
		}

		private T GetValue()
		{
			return (T)fieldInfo.GetValue(StrongInstance);
		}
	}
}
