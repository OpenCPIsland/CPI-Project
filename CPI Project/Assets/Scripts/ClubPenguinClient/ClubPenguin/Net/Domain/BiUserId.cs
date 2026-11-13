using LitJson;

namespace ClubPenguin.Net.Domain
{
	public class BiUserId
	{
		public string biUserId;

		public BiUserId()
		{
			JsonMapper.RegisterExporter<BiUserId>(exportBiUserIdAsString);
		}

		private static void exportBiUserIdAsString(BiUserId value, JsonWriter writer)
		{
			writer.Write(value.biUserId);
		}
	}
}
