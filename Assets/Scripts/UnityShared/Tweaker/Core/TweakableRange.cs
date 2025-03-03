namespace Tweaker.Core
{
	public class TweakableRange<T>
	{
		public T MinValue
		{
			get;
			private set;
		}

		public T MaxValue
		{
			get;
			private set;
		}

		public TweakableRange(T minValue, T maxValue)
		{
			MinValue = minValue;
			MaxValue = maxValue;
		}
	}
}
