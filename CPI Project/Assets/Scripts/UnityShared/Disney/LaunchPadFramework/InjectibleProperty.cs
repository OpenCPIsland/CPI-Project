namespace Disney.LaunchPadFramework
{
	public abstract class InjectibleProperty<T>
	{
		public bool Injectible;

		public T DefaultValue;

		private bool injected;

		private T injectedValue;

		public T Value
		{
			get
			{
				return injected ? injectedValue : DefaultValue;
			}
			set
			{
				if (Injectible)
				{
					injected = true;
					injectedValue = value;
				}
			}
		}
	}
}
