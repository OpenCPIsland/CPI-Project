namespace ClubPenguin.Tests
{
	public class TestGuest
	{
		public string Password
		{
			get;
			private set;
		}

		public string EmailAddress
		{
			get
			{
				return LogInData.profile.email;
			}
		}

		public string Username
		{
			get
			{
				return LogInData.profile.username;
			}
		}

		public LogInData LogInData
		{
			get;
			private set;
		}

		public TestGuest(LogInData logInData, string password)
		{
			LogInData = logInData;
			Password = password;
		}
	}
}
