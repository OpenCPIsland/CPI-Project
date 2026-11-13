namespace Disney.Mix.SDK
{
	public interface IFriend
	{
		IDisplayName DisplayName
		{
			get;
		}

		string FirstName
		{
			get;
		}

		string Id
		{
			get;
		}

		string HashedId
		{
			get;
		}

		bool IsTrusted
		{
			get;
		}

		AccountStatus Status
		{
			get;
		}
	}
}
