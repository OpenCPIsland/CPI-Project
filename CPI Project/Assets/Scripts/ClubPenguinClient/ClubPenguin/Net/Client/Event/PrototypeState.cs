using LitJson;
using System;

namespace ClubPenguin.Net.Client.Event
{
	[Serializable]
	public struct PrototypeState
	{
		public long id;

		public JsonData data;
	}
}
