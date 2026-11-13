namespace Disney.Mix.SDK.Internal
{
	public static class AccountStatusFactory
	{
		public static AccountStatus Create(string accountStatusCode)
		{
			switch (accountStatusCode)
			{
			case "ACTIVE":
				return AccountStatus.Active;
			case "AWAIT_PARENT_CONSENT":
				return AccountStatus.AwaitingParentalConsent;
			case "ACCOUNT_DELETE_REQUESTED":
				return AccountStatus.DeleteRequested;
			default:
				return AccountStatus.Unknown;
			}
		}
	}
}
