using Disney.Mix.SDK.Internal.GuestControllerDomain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Disney.Mix.SDK.Internal
{
	public class RegistrationProfile : IInternalRegistrationProfile, IRegistrationProfile
	{
		private readonly AbstractLogger logger;

		public string DisplayName
		{
			get;
			private set;
		}

		public DateTime? DisplayNameModeratedStatusDate
		{
			get;
			private set;
		}

		public string ProposedDisplayName
		{
			get;
			private set;
		}

		public DisplayNameProposedStatus DisplayNameProposedStatus
		{
			get;
			private set;
		}

		public AccountStatus AccountStatus
		{
			get;
			private set;
		}

		public string AgeBandKey
		{
			get;
			private set;
		}

		public bool AgeBandAssumed
		{
			get;
			private set;
		}

		public DateTime? DateOfBirth
		{
			get;
			private set;
		}

		public string Email
		{
			get;
			private set;
		}

		public bool EmailVerified
		{
			get;
			private set;
		}

		public string FirstName
		{
			get;
			private set;
		}

		public string LastName
		{
			get;
			private set;
		}

		public string MiddleName
		{
			get;
			private set;
		}

		public string ParentEmail
		{
			get;
			private set;
		}

		public bool ParentEmailVerified
		{
			get;
			private set;
		}

		public string Username
		{
			get;
			private set;
		}

		public string LanguagePreference
		{
			get;
			private set;
		}

		public string CountryCode
		{
			get;
			private set;
		}

		public bool IsAdultVerified
		{
			get;
			set;
		}

		public IEnumerable<KeyValuePair<string, bool>> MarketingItems
		{
			get;
			private set;
		}

		public DateTime LastRefreshTime
		{
			get;
			private set;
		}

		public RegistrationProfile(AbstractLogger logger, string displayName, string proposedDisplayName, string proposedStatus, string firstName, string accountStatus, DateTime lastRefreshTime, string countryCode)
		{
			this.logger = logger;
			DisplayName = displayName;
			ProposedDisplayName = proposedDisplayName;
			DisplayNameProposedStatus = GetProposedDisplayNameStatus(proposedStatus);
			FirstName = firstName;
			AccountStatus = AccountStatusFactory.Create(accountStatus);
			LastRefreshTime = lastRefreshTime;
			CountryCode = countryCode;
		}

		public void Update(Profile profile, Disney.Mix.SDK.Internal.GuestControllerDomain.DisplayName displayName, IEnumerable<Disney.Mix.SDK.Internal.GuestControllerDomain.MarketingItem> marketing)
		{
			UpdateDisplayName(displayName);
			UpdateProfile(profile);
			UpdateMarketing(marketing);
			LastRefreshTime = DateTime.UtcNow;
		}

		public void UpdateDisplayName(string displayName)
		{
			DisplayName = displayName;
			DisplayNameProposedStatus = DisplayNameProposedStatus.Accepted;
		}

		private void UpdateDisplayName(Disney.Mix.SDK.Internal.GuestControllerDomain.DisplayName displayName)
		{
			if (displayName != null)
			{
				DateTime result;
				if (!DateTime.TryParse(displayName.moderatedStatusDate, out result))
				{
					DisplayNameModeratedStatusDate = null;
					logger.Error("Received an invalid moderated status date: " + result);
				}
				else
				{
					DisplayNameModeratedStatusDate = result;
				}
				DisplayName = displayName.displayName;
				ProposedDisplayName = displayName.proposedDisplayName;
				DisplayNameProposedStatus = GetProposedDisplayNameStatus(displayName.proposedStatus);
			}
		}

		private void UpdateProfile(Profile profile)
		{
			if (profile != null)
			{
				DateOfBirth = GuestControllerUtils.ParseDateTime(logger, profile.dateOfBirth);
				AgeBandKey = profile.ageBand;
				AgeBandAssumed = profile.ageBandAssumed;
				Email = profile.email;
				EmailVerified = profile.emailVerified;
				FirstName = profile.firstName;
				LastName = profile.lastName;
				MiddleName = profile.middleName;
				ParentEmail = profile.parentEmail;
				ParentEmailVerified = profile.parentEmailVerified;
				Username = profile.username;
				LanguagePreference = profile.languagePreference;
				CountryCode = GuestControllerUtils.GetCountryCode(profile);
				AccountStatus = AccountStatusFactory.Create(profile.status);
				IsAdultVerified = profile.isAdultVerified;
			}
		}

		private void UpdateMarketing(IEnumerable<Disney.Mix.SDK.Internal.GuestControllerDomain.MarketingItem> marketing)
		{
			MarketingItems = ((marketing == null) ? null : marketing.Select((Disney.Mix.SDK.Internal.GuestControllerDomain.MarketingItem p) => new KeyValuePair<string, bool>(p.code, p.subscribed)));
		}

		private static DisplayNameProposedStatus GetProposedDisplayNameStatus(string status)
		{
			switch (status)
			{
			case "ACCEPTED":
				return DisplayNameProposedStatus.Accepted;
			case "REJECTED":
				return DisplayNameProposedStatus.Rejected;
			case "PENDING":
				return DisplayNameProposedStatus.Pending;
			default:
				return DisplayNameProposedStatus.None;
			}
		}
	}
}
