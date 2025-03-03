namespace Disney.Mix.SDK.Internal
{
	public interface IRandom
	{
		int Next(int maxValue);

		long NextLong();
	}
}
