using DeviceDB;

namespace Disney.Mix.SDK.Internal
{
	[Serialized(108, new byte[]
	{

	})]
	public class KeyValueDocument : AbstractDocument
	{
		public static readonly string KeyFieldName = FieldNameGetter.Get((KeyValueDocument f) => f.Key);

		[Indexed]
		[Serialized(0, new byte[]
		{

		})]
		public string Key;

		[Serialized(1, new byte[]
		{

		})]
		public string Value;
	}
}
