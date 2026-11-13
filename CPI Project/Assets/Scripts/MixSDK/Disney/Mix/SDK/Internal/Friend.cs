namespace Disney.Mix.SDK.Internal
{
	public class Friend : IInternalFriend, IFriend
	{
		private readonly IUserDatabase userDatabase;

		public string Swid
		{
			get;
			private set;
		}

		public bool IsTrusted
		{
			get;
			private set;
		}

		public string HashedId
		{
			get
			{
				return userDatabase.GetUserBySwid(Swid).HashedSwid;
			}
		}

		public IDisplayName DisplayName
		{
			get;
			private set;
		}

		public string FirstName
		{
			get;
			private set;
		}

		public AccountStatus Status
		{
			get
			{
				string status = userDatabase.GetUserBySwid(Swid).Status;
				return GuestControllerUtils.GetAccountStatus(status);
			}
		}

		public string Id
		{
			get
			{
				return Swid;
			}
		}

		public Friend(string swid, bool isTrusted, IDisplayName displayName, string firstName, IUserDatabase userDatabase)
		{
			this.userDatabase = userDatabase;
			Swid = swid;
			IsTrusted = isTrusted;
			DisplayName = displayName;
			FirstName = firstName;
		}

		public void ChangeTrust(bool isTrusted)
		{
			IsTrusted = isTrusted;
		}
	}
}
