using Disney.LaunchPadFramework;
using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEngine;

namespace Disney.Kelowna.Common
{
	public class GcsAccessTokenService : IGcsAccessTokenService
	{
		private const long EXPIRE_SOON_DURATION = 60L;

		private const string TARGET_ASSERTION_DESCRIPTOR = "https://www.googleapis.com/oauth2/v4/token";

		private bool isGettingAuthToken = false;

		private GcsAccessTokenResponse cachedAccessTokenResponse;

		private string gcsServiceAccountName;

		private GcsP12FileLoader gcsP12FileLoader;

		private string applicationRequestScope;

		public GcsAccessType AccessType
		{
			set
			{
				switch (value)
				{
				case GcsAccessType.READ_ONLY:
					applicationRequestScope = "https://www.googleapis.com/auth/devstorage.read_only";
					break;
				case GcsAccessType.READ_WRITE:
					applicationRequestScope = "https://www.googleapis.com/auth/devstorage.read_write";
					break;
				}
			}
		}

		public GcsAccessTokenService(string gcsServiceAccountName, GcsP12FileLoader gcsP12FileLoader)
		{
			this.gcsServiceAccountName = gcsServiceAccountName;
			this.gcsP12FileLoader = gcsP12FileLoader;
			AccessType = GcsAccessType.READ_ONLY;
		}

		public IEnumerator GetAccessToken(GcsAccessTokenResponse gcsAccessTokenResponseBuffer)
		{
			if (gcsServiceAccountName == null)
			{
				gcsAccessTokenResponseBuffer.Clear();
				yield break;
			}
			while (isGettingAuthToken)
			{
				yield return null;
			}
			if (isCachedTokenAboutToExpire())
			{
				cachedAccessTokenResponse = null;
			}
			if (cachedAccessTokenResponse == null)
			{
				try
				{
					isGettingAuthToken = true;
					cachedAccessTokenResponse = null;
					yield return GetOAuthRequest();
				}
				finally
				{
					isGettingAuthToken = false;
				}
			}
			gcsAccessTokenResponseBuffer.Copy(cachedAccessTokenResponse);
		}

		private bool isCachedTokenAboutToExpire()
		{
			if (cachedAccessTokenResponse == null)
			{
				return true;
			}
			long unixEpochTimeSecs = GetUnixEpochTimeSecs();
			return cachedAccessTokenResponse.TimeOfExpiration - 60 < unixEpochTimeSecs;
		}

		private IEnumerator GetOAuthRequest()
		{
			IDictionary<string, object> claimsetMap = new Dictionary<string, object>();
			byte[] serviceAccountPrivate = gcsP12FileLoader.Load();
			claimsetMap["iss"] = gcsServiceAccountName;
			claimsetMap["scope"] = applicationRequestScope;
			claimsetMap["aud"] = "https://www.googleapis.com/oauth2/v4/token";
			long assertionTime = GetUnixEpochTimeSecs();
			claimsetMap["iat"] = assertionTime;
			claimsetMap["exp"] = assertionTime + 3600;
			string headerClaim = string.Concat(str2: Base64Encode(JsonMapper.ToJson(claimsetMap)), str0: Base64Encode("{\"alg\":\"RS256\",\"typ\":\"JWT\"}"), str1: ".");
			X509Certificate2 cert = new X509Certificate2(serviceAccountPrivate, "notasecret", X509KeyStorageFlags.Exportable);
			RSACryptoServiceProvider rsa = cert.PrivateKey as RSACryptoServiceProvider;
			byte[] toSign = Encoding.UTF8.GetBytes(headerClaim);
			string sgn = Convert.ToBase64String(rsa.SignData(toSign, "SHA256"));
			string jwt = headerClaim + "." + sgn;
			WWWForm form = new WWWForm();
			form.AddField("grant_type", "assertion");
			form.AddField("assertion_type", "http://oauth.net/grant_type/jwt/1.0/bearer");
			form.AddField("assertion", jwt);
			WWW www = new WWW(headers: new Dictionary<string, string>
			{
				{
					"Content-Type",
					"application/x-www-form-urlencoded"
				}
			}, url: "https://www.googleapis.com/oauth2/v4/token", postData: form.data);
			yield return www;
			if (string.IsNullOrEmpty(www.error))
			{
				string @string = Encoding.UTF8.GetString(www.bytes);
				if (@string != null)
				{
					try
					{
						RawGcsAuthResponse rawGcsAuthResponse = JsonMapper.ToObject<RawGcsAuthResponse>(@string);
						cachedAccessTokenResponse = new GcsAccessTokenResponse(rawGcsAuthResponse, assertionTime);
					}
					catch (JsonException ex)
					{
						cachedAccessTokenResponse = null;
						Log.LogError(this, ex.Message);
					}
				}
			}
			else
			{
				Log.LogNetworkErrorFormatted(this, "Assets Token request had error: {0}", www.error);
				foreach (KeyValuePair<string, string> responseHeader in www.responseHeaders)
				{
					Log.LogNetworkErrorFormatted(this, "{0} : {1}", responseHeader.Key, responseHeader.Value);
				}
			}
		}

		private static long GetUnixEpochTimeSecs()
		{
			return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
		}

		private static string Base64Encode(string inputString)
		{
			if (inputString == null)
			{
				return null;
			}
			byte[] bytes = Encoding.UTF8.GetBytes(inputString);
			return Convert.ToBase64String(bytes);
		}
	}
}
