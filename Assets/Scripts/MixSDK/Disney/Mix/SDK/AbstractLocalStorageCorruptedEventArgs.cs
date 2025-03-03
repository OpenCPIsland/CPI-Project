namespace Disney.Mix.SDK
{
	public abstract class AbstractLocalStorageCorruptedEventArgs : AbstractSessionTerminatedEventArgs
	{
		public bool Recovered;
	}
}
