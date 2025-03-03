using System;

namespace DeviceDB
{
	[Serializable]
	public class CorruptionException : Exception
	{
		public CorruptionException(string message)
			: base(message)
		{
		}

		public CorruptionException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
