using DeviceDB;

namespace Disney.Mix.SDK.Internal
{
	[Serialized(102, new byte[]
	{

	})]
	public class UserDocument : AbstractDocument
	{
		public static readonly string SwidFieldName = FieldNameGetter.Get((UserDocument f) => f.Swid);

		public static readonly string DisplayNameFieldName = FieldNameGetter.Get((UserDocument f) => f.DisplayName);

		[Serialized(0, new byte[]
		{

		})]
		[Indexed]
		public string Swid;

		[Serialized(1, new byte[]
		{

		})]
		[Indexed]
		public string DisplayName;

		[Serialized(2, new byte[]
		{

		})]
		public long AvatarId;

		[Serialized(3, new byte[]
		{

		})]
		public string FirstName;

		[Serialized(4, new byte[]
		{

		})]
		public string HashedSwid;

		[Serialized(5, new byte[]
		{

		})]
		public string Status;
	}
}
