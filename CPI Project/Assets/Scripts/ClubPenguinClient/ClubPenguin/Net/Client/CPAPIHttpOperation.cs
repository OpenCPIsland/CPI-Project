using ClubPenguin.Net.Client.Attributes;
using ClubPenguin.Net.Utils;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using hg.ApiWebKit;
using hg.ApiWebKit.core.http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClubPenguin.Net.Client
{
	public abstract class CPAPIHttpOperation : HttpOperation
	{
		public const string X_ENCRYPTION_KEY_ID_HEADER = "X-Encryption-Key-Id";

		public const string X_ENCRYPTION_ERROR_HEADER = "X-Encryption-Error";

		protected override HttpRequest ToRequest(params string[] parameters)
		{
			HttpRequest httpRequest = base.ToRequest(parameters);
			string setting = Configuration.GetSetting<string>("cp-api-client-token");
			if (httpRequest.RequestModelResult.Headers.ContainsKey("Authorization"))
			{
                Dictionary<string, string> headers = httpRequest.RequestModelResult.Headers;
                Dictionary<string, string> strs = headers;
                (headers = httpRequest.RequestModelResult.Headers)["Authorization"] = headers["Authorization"] + ", GAE " + setting;
			}
			else
			{
				httpRequest.RequestModelResult.Headers["Authorization"] = "GAE " + setting;
			}
			httpRequest.RequestModelResult.Headers["X-CP-Client-Version"] = Configuration.GetSetting<string>("cp-api-client-version");
			httpRequest.RequestModelResult.Headers["X-CP-Content-Version"] = Configuration.GetSetting<string>("cp-content-version");
			httpRequest.RequestModelResult.Headers["X-CP-Sub-Content-Version"] = Configuration.GetSetting<DateTime>("cp-content-version-date").ToString("yyyy-MM-dd");
			string verb = httpRequest.RequestModelResult.Verb;
			byte[] data = httpRequest.RequestModelResult.Data;
			int num;
			switch (verb)
			{
			case "POST":
			case "PUT":
			case "DELETE":
				num = ((data != null && data.Length != 0) ? 1 : 0);
				break;
			default:
				num = 1;
				break;
			}
			if (num == 0)
			{
				Log("Request body is empty. The cp-api services require an empty json object if there is no request payload. One will be added", LogSeverity.VERBOSE);
				httpRequest.RequestModelResult.Data = Encoding.ASCII.GetBytes("{}");
				httpRequest.RequestModelResult.Headers.Remove("Content-Type");
				httpRequest.RequestModelResult.Headers.Add("Content-Type", "application/json");
				Log(httpRequest.RequestModelResult.Summary(), LogSeverity.VERBOSE);
			}
			if (GetType().IsDefined(typeof(EncryptedAttribute), true))
			{
				CPEncryptor setting2 = Configuration.GetSetting<CPEncryptor>("cp-api-encryptor");
				if (setting2 == null)
				{
					Disney.LaunchPadFramework.Log.LogError(this, "CPEncryptor is not set. Will not be able to encrypt the request body.");
				}
				else
				{
					httpRequest.RequestModelResult.Headers.Add("X-Encryption-Key-Id", setting2.KeyId);
					byte[] data2 = setting2.Encrypt(httpRequest.RequestModelResult.Data);
					httpRequest.RequestModelResult.Data = data2;
				}
			}
			httpRequest.RequestModelResult.Headers.Remove("Cache-Control");
			httpRequest.RequestModelResult.Headers.Add("Cache-Control", "no-cache, no-store, must-revalidate");
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Additional request headers added by CPAPIHttpOperation:");
			foreach (string key in httpRequest.RequestModelResult.Headers.Keys)
			{
				stringBuilder.AppendLine("\t" + key + " : " + httpRequest.RequestModelResult.Headers[key]);
			}
			return httpRequest;
		}

		protected override void FromResponse(HttpResponse response)
		{
			if (GetType().IsDefined(typeof(EncryptedAttribute), true))
			{
				CPEncryptor setting = Configuration.GetSetting<CPEncryptor>("cp-api-encryptor");
				if (setting == null)
				{
					Disney.LaunchPadFramework.Log.LogError(this, "CPEncryptor is not set. Will not be able to decrypt the response body.");
				}
				else
				{
					try
					{
						byte[] bytes = response.Data = setting.Decrypt(response.Data);
						response.Text = Encoding.UTF8.GetString(bytes);
					}
					catch
					{
					}
				}
			}
			base.FromResponse(response);
		}

		internal HttpResponse GetOfflineResponse()
		{
			OfflineDatabase offlineDatabase = Service.Get<OfflineDatabase>();
			IOfflineDefinitionLoader offlineDefinitions = Service.Get<IOfflineDefinitionLoader>();
			PerformOfflineAction(offlineDatabase, offlineDefinitions);
			return CreateOfflineResponse();
		}

		protected abstract void PerformOfflineAction(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions);

		protected virtual HttpResponse CreateOfflineResponse()
		{
			return new HttpResponse(new HttpRequest(new HttpRequestModel.HttpRequestModelResult()), 0.1f, new Dictionary<string, string>(), null, null, null, HttpStatusCode.OK);
		}

		internal void UpdateOfflineData()
		{
			OfflineDatabase offlineDatabase = Service.Get<OfflineDatabase>();
			IOfflineDefinitionLoader offlineDefinitions = Service.Get<IOfflineDefinitionLoader>();
			SetOfflineData(offlineDatabase, offlineDefinitions);
		}

		protected virtual void SetOfflineData(OfflineDatabase offlineDatabase, IOfflineDefinitionLoader offlineDefinitions)
		{
		}
	}
}
