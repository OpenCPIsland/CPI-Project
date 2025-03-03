namespace Disney.Mix.SDK.Internal
{
	public static class RemoteUserFactory
	{
		public static IInternalFriend CreateFriend(string swid, bool isTrusted, string displayNameText, string firstName, IUserDatabase userDatabase)
		{
			DisplayName displayName = new DisplayName(displayNameText);
			return new Friend(swid, isTrusted, displayName, firstName, userDatabase);
		}

		public static IInternalUnidentifiedUser CreateUnidentifiedUser(string displayNameText, string firstName, IUserDatabase userDatabase)
		{
			DisplayName displayName = new DisplayName(displayNameText);
			return new UnidentifiedUser(displayName, firstName, userDatabase);
		}
	}
}
