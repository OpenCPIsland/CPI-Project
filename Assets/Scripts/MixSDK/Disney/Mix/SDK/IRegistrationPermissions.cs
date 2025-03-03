namespace Disney.Mix.SDK
{
	public interface IRegistrationPermissions
	{
		IRegistrationPermission Username
		{
			get;
		}

		IRegistrationPermission Password
		{
			get;
		}

		IRegistrationPermission DateOfBirth
		{
			get;
		}

		IRegistrationPermission FirstName
		{
			get;
		}

		IRegistrationPermission LastName
		{
			get;
		}

		IRegistrationPermission Email
		{
			get;
		}

		IRegistrationPermission ParentEmail
		{
			get;
		}
	}
}
