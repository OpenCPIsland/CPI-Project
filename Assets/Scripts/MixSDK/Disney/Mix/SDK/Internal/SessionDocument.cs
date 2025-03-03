using DeviceDB;

namespace Disney.Mix.SDK.Internal
{
	[Serialized(101, new byte[]
	{

	})]
	public class SessionDocument : AbstractDocument
	{
		public static readonly string SwidFieldName = FieldNameGetter.Get((SessionDocument f) => f.Swid);

		[Serialized(0, new byte[]
		{

		})]
		public string GuestControllerAccessToken;

		[Serialized(1, new byte[]
		{

		})]
		public string GuestControllerRefreshToken;

		[Serialized(2, new byte[]
		{

		})]
		[Indexed]
		public string Swid;

		[Serialized(3, new byte[]
		{

		})]
		public string DisplayNameText;

		[Serialized(4, new byte[]
		{

		})]
		public string GuestControllerEtag;

		[Serialized(5, new byte[]
		{

		})]
		public string AgeBand;

		[Serialized(6, new byte[]
		{

		})]
		public uint LastSessionUpdateTime;

		[Serialized(7, new byte[]
		{

		})]
		public bool LoggedOut;

		[Serialized(8, new byte[]
		{

		})]
		public long LastNotificationTime;

		[Serialized(9, new byte[]
		{

		})]
		public string ProposedDisplayName;

		[Serialized(10, new byte[]
		{

		})]
		public string ProposedDisplayNameStatus;

		[Serialized(11, new byte[]
		{

		})]
		public uint LastProfileRefreshTime;

		[Serialized(12, new byte[]
		{

		})]
		public byte[] CurrentSymmetricEncryptionKey;

		[Serialized(13, new byte[]
		{

		})]
		public long SessionId;

		[Serialized(14, new byte[]
		{

		})]
		public string PushNotificationToken;

		[Serialized(15, new byte[]
		{

		})]
		public string PushNotificationTokenType;

		[Serialized(16, new byte[]
		{

		})]
		public bool VisiblePushNotificationsEnabled;

		[Serialized(17, new byte[]
		{

		})]
		public string ProvisionId;

		[Serialized(18, new byte[]
		{

		})]
		public string AccountStatus;

		[Serialized(19, new byte[]
		{

		})]
		public long LatestNotificationSequenceNumber;

		[Serialized(20, new byte[]
		{

		})]
		public string CountryCode;

		[Serialized(21, new byte[]
		{

		})]
		public byte[] PreviousSymmetricEncryptionKey;

		[Serialized(22, new byte[]
		{

		})]
		public string FirstName;

		[Serialized(23, new byte[]
		{

		})]
		public int ProtocolVersion;
	}
}
