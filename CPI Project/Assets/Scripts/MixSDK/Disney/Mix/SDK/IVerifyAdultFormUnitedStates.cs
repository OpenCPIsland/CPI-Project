using System;

namespace Disney.Mix.SDK
{
	public interface IVerifyAdultFormUnitedStates : IVerifyAdultForm
	{
		string AddressLine1
		{
			get;
		}

		string PostalCode
		{
			get;
		}

		DateTime DateOfBirth
		{
			get;
		}

		string FirstName
		{
			get;
		}

		string LastName
		{
			get;
		}

		string Ssn
		{
			get;
		}
	}
}
