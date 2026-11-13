using LitJson;
using System;

namespace ClubPenguin.Net.Domain
{
	[Serializable]
	public struct PCSessionStartResponse
	{
		public string SessionId
		{
			get;
			private set;
		}

		public JsonData SessionSummary
		{
			get;
			private set;
		}
	}
}
