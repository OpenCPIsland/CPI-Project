using DeviceDB;

namespace Disney.Mix.SDK.Internal
{
	[Serialized(100, new byte[]
	{

	})]
	public class FriendDocument : AbstractDocument
	{
		public static readonly string SwidFieldName = FieldNameGetter.Get((FriendDocument f) => f.Swid);

		[Indexed]
		[Serialized(0, new byte[]
		{

		})]
		public string Swid;

		[Serialized(1, new byte[]
		{

		})]
		public bool IsTrusted;

		[Serialized(2, new byte[]
		{

		})]
		public string Nickname;
	}
}
