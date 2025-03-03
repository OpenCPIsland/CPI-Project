using System;

namespace Tweaker.Core
{
	public class VirtualField<T>
	{
		private T value;

		private readonly Action<T> setter;

		private readonly Func<T> getter;

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

		public VirtualField()
		{
			value = default(T);
			setter = SetValue;
			getter = GetValue;
		}

		private void SetValue(T value)
		{
			this.value = value;
		}

		private T GetValue()
		{
			return value;
		}
	}
}
