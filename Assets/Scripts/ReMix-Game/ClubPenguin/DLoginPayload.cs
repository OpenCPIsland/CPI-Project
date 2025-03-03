namespace ClubPenguin
{
	public struct DLoginPayload
	{
		public readonly string Username;

		public readonly string Password;

		public DLoginPayload(string username, string password)
		{
			this = default(DLoginPayload);
			Username = username;
			Password = password;
		}
	}
}
