using Disney.Mix.SDK.Internal.MixDomain;
using System;
using System.Collections;
using System.Security.Cryptography;
using System.Threading;

namespace Disney.Mix.SDK.Internal
{
	public class MixSessionStarter : IMixSessionStarter
	{
		private readonly AbstractLogger logger;

		private readonly IRsaEncryptor rsaEncryptor;

		private readonly IDatabase database;

		private readonly IWebCallEncryptorFactory webCallEncryptorFactory;

		private readonly IWebCallEncryptor sessionStartEncryptor;

		private readonly IMixWebCallFactoryFactory mixWebCallFactoryFactory;

		private readonly IKeychain keychain;

		private readonly ICoroutineManager coroutineManager;

		private readonly ISessionRefresherFactory sessionRefresherFactory;

		private RSAParameters? rsaParameters;

		public MixSessionStarter(AbstractLogger logger, IRsaEncryptor rsaEncryptor, IDatabase database, IWebCallEncryptorFactory webCallEncryptorFactory, IWebCallEncryptor sessionStartEncryptor, IMixWebCallFactoryFactory mixWebCallFactoryFactory, IKeychain keychain, ICoroutineManager coroutineManager, ISessionRefresherFactory sessionRefresherFactory)
		{
			this.logger = logger;
			this.rsaEncryptor = rsaEncryptor;
			this.database = database;
			this.webCallEncryptorFactory = webCallEncryptorFactory;
			this.sessionStartEncryptor = sessionStartEncryptor;
			this.mixWebCallFactoryFactory = mixWebCallFactoryFactory;
			this.keychain = keychain;
			this.coroutineManager = coroutineManager;
			this.sessionRefresherFactory = sessionRefresherFactory;
		}

		public void Start(string swid, string guestControllerAccessToken, Action<MixSessionStartResult> successCallback, Action failureCallback)
		{
			RSAParameters? rSAParameters = database.GetRsaParameters();
			if (rSAParameters.HasValue)
			{
				Start(swid, guestControllerAccessToken, rSAParameters.Value, successCallback, failureCallback);
			}
			else
			{
				coroutineManager.Start(GenerateKeychainCoroutine(swid, guestControllerAccessToken, successCallback, failureCallback));
			}
		}

		private IEnumerator GenerateKeychainCoroutine(string swid, string guestControllerAccessToken, Action<MixSessionStartResult> successCallback, Action failureCallback)
		{
			Thread thread = new Thread((ThreadStart)delegate
			{
				rsaParameters = rsaEncryptor.GenerateKeypair();
			});
			thread.Start();
			while (!rsaParameters.HasValue)
			{
				yield return null;
			}
			database.SetRsaParameters(rsaParameters.Value);
			Start(swid, guestControllerAccessToken, rsaParameters.Value, successCallback, failureCallback);
		}

		private void Start(string swid, string guestControllerAccessToken, RSAParameters rsaParameters, Action<MixSessionStartResult> successCallback, Action failureCallback)
		{
			try
			{
				ISessionRefresher sessionRefresher = sessionRefresherFactory.Create(this);
				IMixWebCallFactory mixWebCallFactory = mixWebCallFactoryFactory.Create(sessionStartEncryptor, swid, guestControllerAccessToken, sessionRefresher);
				StartUserSessionRequest request = BuildRequest(swid, rsaParameters);
				IWebCall<StartUserSessionRequest, StartUserSessionResponse> webCall = mixWebCallFactory.SessionUserPut(request);
				webCall.OnResponse += delegate(object sender, WebCallEventArgs<StartUserSessionResponse> e)
				{
					StartUserSessionResponse response = e.Response;
					if (!ValidateResponse(response))
					{
						logger.Critical("Error parsing the session start response: " + JsonParser.ToJson(response));
						failureCallback();
					}
					else
					{
						long sessionId = response.SessionId.Value;
						byte[] ciphertext = Convert.FromBase64String(response.EncryptedSymmetricKey);
						byte[] symmetricKey = rsaEncryptor.Decrypt(ciphertext, rsaParameters);
						keychain.PushNotificationKey = symmetricKey;
						database.UpdateSessionDocument(swid, delegate(SessionDocument doc)
						{
							doc.PreviousSymmetricEncryptionKey = doc.CurrentSymmetricEncryptionKey;
							doc.CurrentSymmetricEncryptionKey = symmetricKey;
							doc.SessionId = sessionId;
							doc.LatestNotificationSequenceNumber = 0L;
						});
						IWebCallEncryptor webCallEncryptor = webCallEncryptorFactory.Create(symmetricKey, sessionId);
						MixSessionStartResult obj = new MixSessionStartResult(webCallEncryptor);
						successCallback(obj);
					}
				};
				webCall.OnError += delegate
				{
					failureCallback();
				};
				webCall.Execute(true);
			}
			catch (Exception arg)
			{
				logger.Critical("Unhandled exception: " + arg);
				failureCallback();
			}
		}

		private StartUserSessionRequest BuildRequest(string swid, RSAParameters rsaParameters)
		{
			long sessionGroupId = database.GetSessionGroupId();
			string publicKeyExponent = Convert.ToBase64String(rsaParameters.Exponent);
			string publicKeyModulus = Convert.ToBase64String(rsaParameters.Modulus);
			StartUserSessionRequest startUserSessionRequest = new StartUserSessionRequest();
			startUserSessionRequest.PublicKeyExponent = publicKeyExponent;
			startUserSessionRequest.PublicKeyModulus = publicKeyModulus;
			startUserSessionRequest.SessionGroupId = sessionGroupId;
			startUserSessionRequest.UserId = swid;
			startUserSessionRequest.ProtocolVersion = 3;
			return startUserSessionRequest;
		}

		private static bool ValidateResponse(StartUserSessionResponse response)
		{
			return response.SessionId.HasValue && response.EncryptedSymmetricKey != null;
		}
	}
}
