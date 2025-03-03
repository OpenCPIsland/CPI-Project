using LitJson;

namespace ClubPenguin.Net
{
	public static class PrototypeServiceEvents
	{
		public struct StateChangeEvent
		{
			public readonly long Id;

			public readonly JsonData Data;

			public StateChangeEvent(long id, JsonData data)
			{
				Id = id;
				Data = data;
			}
		}

		public struct ActionEvent
		{
			public readonly long Id;

			public readonly JsonData Data;

			public ActionEvent(long id, JsonData data)
			{
				Id = id;
				Data = data;
			}
		}
	}
}
