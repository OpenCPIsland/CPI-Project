using Disney.Mix.SDK.Internal.GuestControllerDomain;
using System;

namespace Disney.Mix.SDK.Internal
{
	public static class GuestControllerUtils
	{
		public static string GetCountryCode(Profile profile)
		{
			return profile.region ?? profile.countryCodeDetected;
		}

		public static DateTime? ParseDateTime(AbstractLogger logger, string date)
		{
			if (date == null)
			{
				return null;
			}
			DateTime result;
			if (!DateTime.TryParse(date, out result))
			{
				logger.Error("Received an invalid date of birth: " + result);
				return null;
			}
			return result;
		}

		public static AccountStatus GetAccountStatus(string status)
		{
			switch (status)
			{
			case "ACTIVE":
				return AccountStatus.Active;
			case "DELETED":
				return AccountStatus.Deleted;
			default:
				return AccountStatus.Unknown;
			}
		}
	}
}
