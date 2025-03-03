namespace ClubPenguin.Net.Client
{
	public static class CPKeyValueDatabaseErrorEvents
	{
		public struct CorruptionErrorEvent
		{
			public readonly bool Recovered;

			public CorruptionErrorEvent(bool recovered)
			{
				Recovered = recovered;
			}
		}
	}
}
