using ClubPenguin.Net.Client.Attributes;
using ClubPenguin.Net.Client.Mappers;
using ClubPenguin.Net.Client.Operations;
using ClubPenguin.Net.Domain;
using ClubPenguin.Net.Utils;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using hg.ApiWebKit;
using hg.ApiWebKit.core.http;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace ClubPenguin.Net.Client
{
	public class RecoverableOperationService
	{
		private struct PendingOperation
		{
			public HttpOperation Operation;

			public string[] Parameters;
		}

		private const int ONEID_AUTH_FAILURE_CODE = 3;

		private const int ENCRYPTION_KEY_EXPIRED_CODE = 1000;

		private ClubPenguinClient clubPenguinClient;

		private Queue<PendingOperation> pendingOperations = new Queue<PendingOperation>();

		private bool generatingKeyPair = false;

		private bool requestingEncryptionKey = false;

		private bool refreshingAccessToken = false;

		public RecoverableOperationService(ClubPenguinClient clubPenguinClient)
		{
			this.clubPenguinClient = clubPenguinClient;
		}

		public void SendOperation<T>(T operation, Action<T, HttpResponse> on_success = null, Action<T, HttpResponse> on_failure = null, Action<T, HttpResponse> on_complete = null, params string[] parameters) where T : HttpOperation
		{
			operation["on-complete"] = (Action<T, HttpResponse>)delegate(T self, HttpResponse response)
			{
				if (isMixSessionExpired(self, response))
				{
					clearAccessToken();
					setupAuthentication(new PendingOperation
					{
						Operation = self,
						Parameters = self.InParameters
					});
				}
				if (isEncryptionKeyExpired(response))
				{
					clearEncryptionKey();
					setupEncryption(new PendingOperation
					{
						Operation = self,
						Parameters = self.InParameters
					});
				}
				if (on_success != null && (response.Is2XX || response.Is100) && !self.IsFaulted)
				{
					on_success.InvokeSafe(self, response);
				}
				if (on_failure != null && ((!response.Is2XX && !response.Is100) || self.IsFaulted))
				{
					on_failure.InvokeSafe(self, response);
				}
				on_complete.InvokeSafe(self, response);
			};
			sendOperation(operation, parameters);
		}

		protected virtual void sendOperation(HttpOperation operation, params string[] parameters)
		{
			if ((!requiresAuthentication(operation) || setupAuthentication(new PendingOperation
			{
				Operation = operation,
				Parameters = parameters
			})) && (!requiresEncryption(operation) || setupEncryption(new PendingOperation
			{
				Operation = operation,
				Parameters = parameters
			})))
			{
				operation.Send(parameters);
			}
		}

		private bool requiresAuthentication(HttpOperation operation)
		{
			return operation.GetType().IsDefined(typeof(HttpBasicAuthorizationAttribute), false);
		}

		private bool haveAccessToken()
		{
			return clubPenguinClient.AccessToken != null;
		}

		private void clearAccessToken()
		{
			clubPenguinClient.AccessToken = null;
		}

		private bool requiresEncryption(HttpOperation operation)
		{
			return operation.GetType().IsDefined(typeof(EncryptedAttribute), false);
		}

		private bool haveKeyPair()
		{
			return clubPenguinClient.CPKeyValueDatabase.GetRsaParameters().HasValue;
		}

		private bool haveEncryptionKey()
		{
			return Configuration.HasSetting("cp-api-encryptor");
		}

		private void clearEncryptionKey()
		{
			Configuration.RemoveSetting("cp-api-encryptor");
		}

		private bool isMixSessionExpired(HttpOperation operation, HttpResponse httpResponse)
		{
			bool result = false;
			if (httpResponse.StatusCode == HttpStatusCode.Unauthorized && requiresAuthentication(operation))
			{
				result = true;
				try
				{
					ErrorResponse errorResponse = Service.Get<JsonService>().Deserialize<ErrorResponse>(httpResponse.Text);
					result = (errorResponse.code == 3);
				}
				catch (Exception ex)
				{
					Log.LogErrorFormatted(this, "Unable to parse unauthorized response {0}, {1}", httpResponse, ex);
				}
			}
			return result;
		}

		private bool isEncryptionKeyExpired(HttpResponse httpResponse)
		{
			return httpResponse.StatusCode == HttpStatusCode.BadRequest && httpResponse.Headers != null && httpResponse.Headers.ContainsKey("X-Encryption-Error");
		}

		private bool setupAuthentication(PendingOperation pendingOperation)
		{
			if (refreshingAccessToken)
			{
				pendingOperations.Enqueue(pendingOperation);
				return false;
			}
			if (!haveAccessToken())
			{
				pendingOperations.Enqueue(pendingOperation);
				refreshingAccessToken = true;
				return false;
			}
			return true;
		}

		public void AccessTokenRefreshed()
		{
			refreshingAccessToken = false;
			sendPendingOperations();
		}

		private bool setupEncryption(PendingOperation pendingOperation)
		{
			if (generatingKeyPair || requestingEncryptionKey)
			{
				pendingOperations.Enqueue(pendingOperation);
				return false;
			}
			if (!haveKeyPair())
			{
				pendingOperations.Enqueue(pendingOperation);
				generatingKeyPair = true;
				clubPenguinClient.GenerateKeyPair(delegate(RSAParameters rsaParams)
				{
					generatingKeyPair = false;
					clubPenguinClient.CPKeyValueDatabase.SetRsaParameters(rsaParams);
					sendPendingOperations();
				}, delegate
				{
					generatingKeyPair = false;
					failPendingOperations();
				});
				return false;
			}
			if (!haveEncryptionKey())
			{
				pendingOperations.Enqueue(pendingOperation);
				requestingEncryptionKey = true;
				RSAParameters rsaParameters = clubPenguinClient.CPKeyValueDatabase.GetRsaParameters().Value;
				string publicKeyExponent = Convert.ToBase64String(rsaParameters.Exponent);
				string publicKeyModulus = Convert.ToBase64String(rsaParameters.Modulus);
				APICall<GetEncryptionKeyOperation> encryptionKey = clubPenguinClient.EncryptionApi.GetEncryptionKey(publicKeyExponent, publicKeyModulus);
				encryptionKey.OnResponse += delegate(GetEncryptionKeyOperation op, HttpResponse resp)
				{
					requestingEncryptionKey = false;
					try
					{
						string keyId = op.ResponseBody.keyId;
						byte[] ciphertext = Convert.FromBase64String(op.ResponseBody.encryptedSymmetricKey);
						byte[] symmetricEncryptionKey = RsaEncryptor.Decrypt(ciphertext, rsaParameters);
						updateEncryptionKey(keyId, symmetricEncryptionKey);
						sendPendingOperations();
					}
					catch (Exception ex)
					{
						Log.LogErrorFormatted(this, "Failed to decrypt symmetric key from server. Operations requiring encryption will fail. Exception: {0}", ex);
						failPendingOperations();
					}
				};
				encryptionKey.OnError += delegate
				{
					requestingEncryptionKey = false;
					failPendingOperations();
				};
				encryptionKey.Execute();
				return false;
			}
			return true;
		}

		private void updateEncryptionKey(string keyId, byte[] symmetricEncryptionKey)
		{
			CPEncryptor value = new CPEncryptor(keyId, symmetricEncryptionKey);
			Configuration.SetSetting("cp-api-encryptor", value);
		}

		private void sendPendingOperations()
		{
			while (pendingOperations.Count > 0)
			{
				PendingOperation pendingOperation = pendingOperations.Dequeue();
				sendOperation(pendingOperation.Operation, pendingOperation.Parameters);
			}
		}

		private void failPendingOperations()
		{
			string text = "Failed to resolve recoverable error conditions. Pending operations will not be sent.";
			HttpStatusCode statusCode = HttpStatusCode.PreconditionFailed;
			while (pendingOperations.Count > 0)
			{
				PendingOperation pendingOperation = pendingOperations.Dequeue();
				HttpResponse httpResponse = new HttpResponse(pendingOperation.Operation.Request, 0f, new Dictionary<string, string>(), text, text, null, statusCode);
				pendingOperation.Operation.InvokeUserAction("on-complete", pendingOperation.Operation, httpResponse);
			}
		}
	}
}
