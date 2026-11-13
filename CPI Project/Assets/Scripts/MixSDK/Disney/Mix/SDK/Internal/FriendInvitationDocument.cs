using DeviceDB;

namespace Disney.Mix.SDK.Internal
{
	[Serialized(104, new byte[]
	{

	})]
	public class FriendInvitationDocument : AbstractDocument
	{
		public static readonly string FriendInvitationIdFieldName = FieldNameGetter.Get((FriendInvitationDocument i) => i.FriendInvitationId);

		[Indexed]
		[Serialized(0, new byte[]
		{

		})]
		public long FriendInvitationId;

		[Serialized(1, new byte[]
		{

		})]
		public string DisplayName;

		[Serialized(2, new byte[]
		{

		})]
		public bool IsInviter;

		[Serialized(3, new byte[]
		{

		})]
		public bool IsTrusted;
	}
}
