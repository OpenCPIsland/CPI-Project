namespace Tweaker.Core
{
	public class TweakableToggleValue<T> : TweakableNamedToggleValue<T>
	{
		public TweakableToggleValue(T value)
			: base(value.ToString(), value)
		{
		}
	}
}
