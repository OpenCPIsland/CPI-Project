using LitJson;
using System;

namespace ClubPenguin.Net.Client.Event
{
	[Serializable]
	public struct ConsumableUseFailureEvent
	{
		public string Type;

		public JsonData Properties;
	}
}
