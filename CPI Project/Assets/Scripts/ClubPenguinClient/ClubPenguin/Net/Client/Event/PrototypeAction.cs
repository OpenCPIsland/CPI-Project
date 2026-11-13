using LitJson;
using System;

namespace ClubPenguin.Net.Client.Event
{
	[Serializable]
	public struct PrototypeAction
	{
		public long userid;

		public JsonData data;
	}
}
