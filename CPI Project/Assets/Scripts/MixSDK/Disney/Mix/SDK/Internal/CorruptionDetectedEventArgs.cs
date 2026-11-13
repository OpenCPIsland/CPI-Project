using System;

namespace Disney.Mix.SDK.Internal
{
	public class CorruptionDetectedEventArgs : EventArgs
	{
		public bool Recovered;

		public CorruptionDetectedEventArgs(bool recovered)
		{
			Recovered = recovered;
		}
	}
}
