using System;
using System.Collections.Generic;

namespace Disney.Mix.SDK
{
	public interface IRegistrationProfile
	{
		string DisplayName
		{
			get;
		}

		DateTime? DisplayNameModeratedStatusDate
		{
			get;
		}

		string ProposedDisplayName
		{
			get;
		}

		DisplayNameProposedStatus DisplayNameProposedStatus
		{
			get;
		}

		AccountStatus AccountStatus
		{
			get;
		}

		string AgeBandKey
		{
			get;
		}

		bool AgeBandAssumed
		{
			get;
		}

		DateTime? DateOfBirth
		{
			get;
		}

		string Email
		{
			get;
		}

		bool EmailVerified
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

		string MiddleName
		{
			get;
		}

		string ParentEmail
		{
			get;
		}

		bool ParentEmailVerified
		{
			get;
		}

		string Username
		{
			get;
		}

		string LanguagePreference
		{
			get;
		}

		string CountryCode
		{
			get;
		}

		bool IsAdultVerified
		{
			get;
		}

		IEnumerable<KeyValuePair<string, bool>> MarketingItems
		{
			get;
		}

		DateTime LastRefreshTime
		{
			get;
		}
	}
}
