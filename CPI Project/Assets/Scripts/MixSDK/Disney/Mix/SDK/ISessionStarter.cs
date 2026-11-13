using System;
using System.Collections.Generic;

namespace Disney.Mix.SDK
{
	public interface ISessionStarter
	{
		void Login(string guestControllerHostUrl, string guestControllerSpoofedIpAddress, string mixApiHostUrl, string oneIdClientId, string mixClientToken, string clientVersion, ICoroutineManager coroutineManager, IKeychain keychain, AbstractLogger logger, string localStorageDirPath, string loginValue, string password, string languageCode, Action<ILoginResult> callback);

		void RegisterAdultAccount(string guestControllerHostUrl, string guestControllerSpoofedIpAddress, string mixApiHostUrl, string oneIdClientId, string mixClientToken, string clientVersion, ICoroutineManager coroutineManager, IKeychain keychain, AbstractLogger logger, string localStorageDirPath, bool isTestProfile, string password, string firstName, string lastName, string email, string languageCode, DateTime? dateOfBirth, IEnumerable<KeyValuePair<IMarketingItem, bool>> marketingAgreements, IEnumerable<ILegalDocument> acceptedLegalDocuments, Action<IRegisterResult> callback);

		void RegisterAdultAccount(string guestControllerHostUrl, string guestControllerSpoofedIpAddress, string mixApiHostUrl, string oneIdClientId, string mixClientToken, string clientVersion, ICoroutineManager coroutineManager, IKeychain keychain, AbstractLogger logger, string localStorageDirPath, bool isTestProfile, string password, string firstName, string lastName, string email, string languageCode, string assertedCountry, DateTime? dateOfBirth, IEnumerable<KeyValuePair<IMarketingItem, bool>> marketingAgreements, IEnumerable<ILegalDocument> acceptedLegalDocuments, Action<IRegisterResult> callback);

		void RegisterChildAccount(string guestControllerHostUrl, string guestControllerSpoofedIpAddress, string mixApiHostUrl, string oneIdClientId, string mixClientToken, string clientVersion, ICoroutineManager coroutineManager, IKeychain keychain, AbstractLogger logger, string localStorageDirPath, bool isTestProfile, string username, string password, string firstName, string parentEmail, string languageCode, DateTime? dateOfBirth, IEnumerable<KeyValuePair<IMarketingItem, bool>> marketingAgreements, IEnumerable<ILegalDocument> acceptedLegalDocuments, Action<IRegisterResult> callback);

		void RegisterChildAccount(string guestControllerHostUrl, string guestControllerSpoofedIpAddress, string mixApiHostUrl, string oneIdClientId, string mixClientToken, string clientVersion, ICoroutineManager coroutineManager, IKeychain keychain, AbstractLogger logger, string localStorageDirPath, bool isTestProfile, string username, string password, string firstName, string parentEmail, string languageCode, string assertedCountry, DateTime? dateOfBirth, IEnumerable<KeyValuePair<IMarketingItem, bool>> marketingAgreements, IEnumerable<ILegalDocument> acceptedLegalDocuments, Action<IRegisterResult> callback);

		void RestoreLastSession(string guestControllerHostUrl, string guestControllerSpoofedIpAddress, string mixApiHostUrl, string oneIdClientId, string mixClientToken, string clientVersion, ICoroutineManager coroutineManager, IKeychain keychain, AbstractLogger logger, string localStorageDirPath, string languageCode, Action<IRestoreLastSessionResult> callback);

		void ReuseExistingGuestControllerLogin(string guestControllerHostUrl, string guestControllerSpoofedIpAddress, string mixApiHostUrl, string oneIdClientId, string mixClientToken, string clientVersion, ICoroutineManager coroutineManager, IKeychain keychain, AbstractLogger logger, string localStorageDirPath, string swid, string accessToken, uint accessTokenTtl, string refreshToken, uint refreshTokenTtl, string displayName, string displayNameModeratedStatusDate, string proposedDisplayName, string displayNameProposedStatus, string etag, string ageBand, bool ageBandAssumed, string accountStatus, string dateOfBirth, string email, bool emailVerified, string firstName, string lastName, string middleName, string parentEmail, bool parentEmailVerified, string username, string languageCode, string countryCode, IEnumerable<KeyValuePair<string, bool>> marketingItems, Action<IReuseExistingGuestControllerLoginResult> callback);

		void OfflineLastSession(string guestControllerHostUrl, string guestControllerSpoofedIpAddress, string mixApiHostUrl, string oneIdClientId, string mixClientToken, string clientVersion, ICoroutineManager coroutineManager, IKeychain keychain, AbstractLogger logger, string localStorageDirPath, string languageCode, Action<IOfflineLastSessionResult> callback);
	}
}
