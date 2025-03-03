namespace Tweaker.Core
{
	public class TweakableNamedToggleValue<T>
	{
		public string Name;

		public T Value
		{
			get;
			private set;
		}

		public TweakableNamedToggleValue(string name, T value)
		{
			Name = name;
			Value = value;
		}
	}
}
