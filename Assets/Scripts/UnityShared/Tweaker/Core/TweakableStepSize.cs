namespace Tweaker.Core
{
	public class TweakableStepSize<T>
	{
		public T Size
		{
			get;
			private set;
		}

		public TweakableStepSize(T size)
		{
			Size = size;
		}
	}
}
