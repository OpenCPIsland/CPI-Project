namespace Tweaker.Core
{
	public class Tweakable<T> : AutoTweakable
	{
		public T value;

		public T SetTweakableValue(T value)
		{
			if (CheckValidTweakable())
			{
				tweakable.SetValue(value);
			}
			return (T)tweakable.GetValue();
		}

		public Tweakable(T value)
		{
			this.value = value;
		}

		public Tweakable()
			: this(default(T))
		{
		}
	}
}
