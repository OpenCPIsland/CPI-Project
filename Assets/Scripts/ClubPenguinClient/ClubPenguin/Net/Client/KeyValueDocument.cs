using DeviceDB;
using Disney.Kelowna.Common;

namespace ClubPenguin.Net.Client
{
	[Serialized(0, new byte[]
	{

	})]
	public class KeyValueDocument : AbstractDocument
	{
		public static readonly string KeyFieldName = FieldNameGetter.Get((KeyValueDocument f) => f.Key);

		[Serialized(0, new byte[]
		{

		})]
		[Indexed]
		public string Key;

		[Serialized(1, new byte[]
		{

		})]
		public string Value;
	}
}
