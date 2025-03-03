namespace Disney.Mix.SDK.Internal
{
	public class UnidentifiedUser : IInternalUnidentifiedUser, IUnidentifiedUser
	{
		private readonly IUserDatabase userDatabase;

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

		public UnidentifiedUser(IDisplayName displayName, string firstName, IUserDatabase userDatabase)
		{
			this.userDatabase = userDatabase;
			DisplayName = displayName;
			FirstName = firstName;
		}
	}
}
