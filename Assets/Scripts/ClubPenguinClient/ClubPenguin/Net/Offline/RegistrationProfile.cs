using ClubPenguin.Net.Client;
using Disney.Kelowna.Common;

namespace ClubPenguin.Net.Offline
{
	public struct RegistrationProfile : IOfflineData
	{
		public string firstName;

		public string userName;

		public string displayName;

		public string parentEmail;

		public string password;

		public void Init()
		{
		}

		public string Id()
		{
			return Id(userName);
		}

		public static string Id(string userName)
		{
			return MD5HashUtil.GetHash(userName.ToLowerInvariant());
		}

		public static RegistrationProfile Load(string userName)
		{
			RegistrationProfile registrationProfile = OfflineDatabase.Read<RegistrationProfile>(Id(userName));
			if (string.IsNullOrEmpty(registrationProfile.userName))
			{
				registrationProfile.userName = userName;
				OfflineDatabase.Write(registrationProfile, Id(userName));
			}
			return registrationProfile;
		}
	}
}
