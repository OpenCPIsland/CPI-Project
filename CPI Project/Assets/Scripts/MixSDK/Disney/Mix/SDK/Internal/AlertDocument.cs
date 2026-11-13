using DeviceDB;

namespace Disney.Mix.SDK.Internal
{
	[Serialized(110, new byte[]
	{

	})]
	public class AlertDocument : AbstractDocument
	{
		public static readonly string AlertIdFieldName = FieldNameGetter.Get((AlertDocument d) => d.AlertId);

		[Serialized(0, new byte[]
		{

		})]
		[Indexed]
		public long AlertId;

		[Serialized(1, new byte[]
		{

		})]
		public string Level;

		[Serialized(2, new byte[]
		{

		})]
		public string Type;
	}
}
