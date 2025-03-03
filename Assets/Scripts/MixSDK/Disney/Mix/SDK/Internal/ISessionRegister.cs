using System;
using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal
{
	public interface ISessionRegister
	{
		void Register(bool isTestProfile, string username, string password, string firstName, string lastName, string email, string parentEmail, string languagePreference, string assertedCountry, DateTime? dateOfBirth, IEnumerable<KeyValuePair<IMarketingItem, bool>> marketingAgreements, IEnumerable<ILegalDocument> acceptedLegalDocuments, Action<IRegisterResult> callback);
	}
}
