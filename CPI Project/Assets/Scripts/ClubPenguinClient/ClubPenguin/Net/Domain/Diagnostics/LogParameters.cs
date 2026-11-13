using System;

namespace ClubPenguin.Net.Domain.Diagnostics
{
	[Serializable]
	public class LogParameters
	{
		public string severity;

		public string message;

		public string logName;
	}
}
