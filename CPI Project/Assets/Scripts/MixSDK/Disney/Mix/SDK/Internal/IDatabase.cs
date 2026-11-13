using Disney.Mix.SDK.Internal.GuestControllerDomain;
using System;
using System.Security.Cryptography;

namespace Disney.Mix.SDK.Internal
{
	public interface IDatabase
	{
		long GetSessionGroupId();

		RSAParameters? GetRsaParameters();

		void SetRsaParameters(RSAParameters rsaParameters);

		long? GetServerTimeOffsetMillis();

		void SetServerTimeOffsetMillis(long offsetMillis);

		void ClearServerTimeOffsetMillis();

		string GetGuestControllerApiKey();

		void SetGuestControllerApiKey(string apiKey);

		void ClearGuestControllerApiKey();

		void StoreSession(string swid, string accessToken, string refreshToken, string displayName, string firstName, string etag, string ageBand, string proposedDisplayName, string proposedDisplayNameStatus, string accountStatus, bool updateLastProfileRefreshTime, string countryCode);

		void UpdateGuestControllerToken(Token token, string etag);

		SessionDocument GetLastLoggedInSessionDocument();

		SessionDocument GetSessionDocument(string swid);

		void UpdateSessionDocument(string swid, Action<SessionDocument> updateCallback);

		void LogOutSession(string swid);

		void DeleteSession(string swid);
	}
}
