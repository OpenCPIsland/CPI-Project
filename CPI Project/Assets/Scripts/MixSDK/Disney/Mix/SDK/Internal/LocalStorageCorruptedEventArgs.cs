namespace Disney.Mix.SDK.Internal
{
	public class LocalStorageCorruptedEventArgs : AbstractLocalStorageCorruptedEventArgs
	{
		public LocalStorageCorruptedEventArgs(bool recovered)
		{
			Recovered = recovered;
		}
	}
}
