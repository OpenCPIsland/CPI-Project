using SwrveUnity.Helpers;
using SwrveUnity.REST;
using SwrveUnityMiniJSON;
using System;
using System.Collections.Generic;
using System.Text;

namespace SwrveUnity.Messaging
{
	public class SwrveQAUser
	{
		private const int ApiVersion = 1;

		private const long SessionInterval = 1000L;

		private const long TriggerInterval = 500L;

		private const long PushNotificationInterval = 1000L;

		private const string PushTrackingKey = "_p";

		private readonly SwrveSDK swrve;

		private readonly IRESTClient restClient;

		private readonly string loggingUrl;

		private long lastSessionRequestTime;

		private long lastTriggerRequestTime;

		private long lastPushNotificationRequestTime = 0L;

		public readonly bool ResetDevice;

		public readonly bool Logging;

		public Dictionary<int, string> campaignReasons = new Dictionary<int, string>();

		public Dictionary<int, SwrveBaseMessage> campaignMessages = new Dictionary<int, SwrveBaseMessage>();

		public SwrveQAUser(SwrveSDK swrve, Dictionary<string, object> jsonQa)
		{
			this.swrve = swrve;
			ResetDevice = MiniJsonHelper.GetBool(jsonQa, "reset_device_state", false);
			Logging = MiniJsonHelper.GetBool(jsonQa, "logging", false);
			if (Logging)
			{
				restClient = new RESTClient();
				loggingUrl = MiniJsonHelper.GetString(jsonQa, "logging_url", null);
				loggingUrl = loggingUrl.Replace("http://", "https://");
				if (!loggingUrl.EndsWith("/"))
				{
					loggingUrl += "/";
				}
			}
			campaignReasons = new Dictionary<int, string>();
			campaignMessages = new Dictionary<int, SwrveBaseMessage>();
		}

		protected string getEndpoint(string path)
		{
			while (path.StartsWith("/"))
			{
				path = path.Substring(1);
			}
			return loggingUrl + path;
		}

		public void TalkSession(Dictionary<int, string> campaignsDownloaded)
		{
			try
			{
				if (CanMakeSessionRequest())
				{
					lastSessionRequestTime = SwrveHelper.GetMilliseconds();
					string endpoint = getEndpoint("talk/game/" + swrve.ApiKey + "/user/" + swrve.UserId + "/session");
					Dictionary<string, object> dictionary = new Dictionary<string, object>();
					IList<object> list = new List<object>();
					Dictionary<int, string>.Enumerator enumerator = campaignsDownloaded.GetEnumerator();
					while (enumerator.MoveNext())
					{
						int key = enumerator.Current.Key;
						string value = enumerator.Current.Value;
						Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
						dictionary2.Add("id", key);
						dictionary2.Add("reason", (value == null) ? string.Empty : value);
						dictionary2.Add("loaded", value == null);
						list.Add(dictionary2);
					}
					dictionary.Add("campaigns", list);
					Dictionary<string, string> deviceInfo = swrve.GetDeviceInfo();
					dictionary.Add("device", deviceInfo);
					MakeRequest(endpoint, dictionary);
				}
			}
			catch (Exception ex)
			{
				SwrveLog.LogError("QA request talk session failed: " + ex.ToString());
			}
		}

		public void UpdateDeviceInfo()
		{
			try
			{
				if (CanMakeRequest())
				{
					string endpoint = getEndpoint("talk/game/" + swrve.ApiKey + "/user/" + swrve.UserId + "/device_info");
					Dictionary<string, object> dictionary = new Dictionary<string, object>();
					Dictionary<string, string> deviceInfo = swrve.GetDeviceInfo();
					Dictionary<string, string>.Enumerator enumerator = deviceInfo.GetEnumerator();
					while (enumerator.MoveNext())
					{
						dictionary.Add(enumerator.Current.Key, enumerator.Current.Value);
					}
					MakeRequest(endpoint, dictionary);
				}
			}
			catch (Exception ex)
			{
				SwrveLog.LogError("QA request talk device info update failed: " + ex.ToString());
			}
		}

		private void MakeRequest(string endpoint, Dictionary<string, object> json)
		{
			json.Add("version", 1);
			json.Add("client_time", DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"));
			string s = Json.Serialize(json);
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("Content-Type", "application/json; charset=utf-8");
			Dictionary<string, string> headers = dictionary;
			swrve.Container.StartCoroutine(restClient.Post(endpoint, bytes, headers, RestListener));
		}

		public void TriggerFailure(string eventName, string globalReason)
		{
			try
			{
				if (CanMakeTriggerRequest())
				{
					string endpoint = getEndpoint("talk/game/" + swrve.ApiKey + "/user/" + swrve.UserId + "/trigger");
					Dictionary<string, object> dictionary = new Dictionary<string, object>();
					dictionary.Add("trigger_name", eventName);
					dictionary.Add("displayed", false);
					dictionary.Add("reason", globalReason);
					dictionary.Add("campaigns", new List<object>());
					MakeRequest(endpoint, dictionary);
				}
			}
			catch (Exception ex)
			{
				SwrveLog.LogError("QA request talk session failed: " + ex.ToString());
			}
		}

		public void Trigger(string eventName, SwrveBaseMessage baseMessage)
		{
			try
			{
				if (CanMakeTriggerRequest())
				{
					lastTriggerRequestTime = SwrveHelper.GetMilliseconds();
					Dictionary<int, string> dictionary = campaignReasons;
					Dictionary<int, SwrveBaseMessage> dictionary2 = campaignMessages;
					campaignReasons = new Dictionary<int, string>();
					campaignMessages = new Dictionary<int, SwrveBaseMessage>();
					string endpoint = getEndpoint("talk/game/" + swrve.ApiKey + "/user/" + swrve.UserId + "/trigger");
					Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
					dictionary3.Add("trigger_name", eventName);
					dictionary3.Add("displayed", baseMessage != null);
					dictionary3.Add("reason", (baseMessage == null) ? "The loaded campaigns returned no conversation or message" : string.Empty);
					IList<object> list = new List<object>();
					Dictionary<int, string>.Enumerator enumerator = dictionary.GetEnumerator();
					while (enumerator.MoveNext())
					{
						int key = enumerator.Current.Key;
						string value = enumerator.Current.Value;
						Dictionary<string, object> dictionary4 = new Dictionary<string, object>();
						dictionary4.Add("id", key);
						dictionary4.Add("displayed", false);
						dictionary4.Add("reason", (value == null) ? string.Empty : value);
						if (dictionary2.ContainsKey(key))
						{
							SwrveBaseMessage swrveBaseMessage = dictionary2[key];
							dictionary4.Add(swrveBaseMessage.GetBaseMessageType() + "_id", swrveBaseMessage.Id);
						}
						list.Add(dictionary4);
					}
					if (baseMessage != null)
					{
						Dictionary<string, object> dictionary4 = new Dictionary<string, object>();
						dictionary4.Add("id", baseMessage.Campaign.Id);
						dictionary4.Add("displayed", true);
						dictionary4.Add(baseMessage.GetBaseMessageType() + "_id", baseMessage.Id);
						dictionary4.Add("reason", string.Empty);
						list.Add(dictionary4);
					}
					dictionary3.Add("campaigns", list);
					MakeRequest(endpoint, dictionary3);
				}
			}
			catch (Exception ex)
			{
				SwrveLog.LogError("QA request talk session failed: " + ex.ToString());
			}
		}

		private bool CanMakeRequest()
		{
			return swrve != null && Logging;
		}

		private bool CanMakeTimedRequest(long lastTime, long intervalTime)
		{
			if (CanMakeRequest() && (lastTime == 0 || SwrveHelper.GetMilliseconds() - lastTime > 1000))
			{
				return true;
			}
			return false;
		}

		private bool CanMakeSessionRequest()
		{
			return CanMakeTimedRequest(lastSessionRequestTime, 1000L);
		}

		private bool CanMakeTriggerRequest()
		{
			return CanMakeTimedRequest(lastTriggerRequestTime, 500L);
		}

		private bool CanMakePushNotificationRequest()
		{
			return CanMakeTimedRequest(lastPushNotificationRequestTime, 1000L);
		}

		private void RestListener(RESTResponse response)
		{
			if (response.Error != 0)
			{
				SwrveLog.LogError("QA request to failed with error code " + response.Error.ToString() + ": " + response.Body);
			}
		}
	}
}
