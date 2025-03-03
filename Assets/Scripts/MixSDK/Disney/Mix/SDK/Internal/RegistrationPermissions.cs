namespace Disney.Mix.SDK.Internal
{
	public class RegistrationPermissions : IRegistrationPermissions
	{
		public IRegistrationPermission Username
		{
			get;
			private set;
		}

		public IRegistrationPermission Password
		{
			get;
			private set;
		}

		public IRegistrationPermission DateOfBirth
		{
			get;
			private set;
		}

		public IRegistrationPermission FirstName
		{
			get;
			private set;
		}

		public IRegistrationPermission LastName
		{
			get;
			private set;
		}

		public IRegistrationPermission Email
		{
			get;
			private set;
		}

		public IRegistrationPermission ParentEmail
		{
			get;
			private set;
		}

		public RegistrationPermissions(IRegistrationPermission username, IRegistrationPermission password, IRegistrationPermission dateOfBirth, IRegistrationPermission firstName, IRegistrationPermission lastName, IRegistrationPermission email, IRegistrationPermission parentEmail)
		{
			Username = username;
			Password = password;
			DateOfBirth = dateOfBirth;
			FirstName = firstName;
			LastName = lastName;
			Email = email;
			ParentEmail = parentEmail;
		}
	}
}
