using ClubPenguin.Analytics;
using DCPI.Platforms.SwrveManager.Utils;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using SwrveUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitContentSchedulerServiceAction))]
	[RequireComponent(typeof(InitCoreServicesAction))]
	[RequireComponent(typeof(InitLocalizerSetupAction))]
	[RequireComponent(typeof(InitDiagnosticsAction))]
	[RequireComponent(typeof(InitConfiguratorAction))]
	public class InitAnalyticsAction : InitActionComponent
	{
		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct AnalyticsInitialized
		{
		}

		private const string BUILD_SETTINGS_KEY = "mobilenetwork.dmoAnalytics.key";

		private const string BUILD_SETTINGS_SECRET = "mobilenetwork.dmoAnalytics.secret";

		[Tweakable("Debug.Analytics.EnableAnalyticsLogging", Description = "Requires application restart.")]
		public static bool EnableAnalyticsLogging
		{
			get
			{
				return Service.Get<GameSettings>().EnableAnalyticsLogging.Value;
			}
			set
			{
				Service.Get<GameSettings>().EnableAnalyticsLogging.Value = value;
			}
		}

		public override bool HasSecondPass
		{
			get
			{
				return true;
			}
		}

		public override bool HasCompletedPass
		{
			get
			{
				return false;
			}
		}

		public override IEnumerator PerformFirstPass()
		{
			yield return CoroutineRunner.Start(initSwrve(), this, "initSwrve");
			yield return CoroutineRunner.Start(initTechAnalytics(), this, "initTechAnalytics");
		}

		public override IEnumerator PerformSecondPass()
		{
			Service.Get<ICPSwrveService>().Action("view.boot");
			logDeviceDetails();
			new EventLogger(Service.Get<EventDispatcher>());
			yield break;
		}

		private void logDeviceDetails()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("system_memory", SystemInfo.systemMemorySize.ToString());
			dictionary.Add("graphics_memory", SystemInfo.graphicsMemorySize.ToString());
			dictionary.Add("graphics_type", SystemInfo.graphicsDeviceType.ToString());
			dictionary.Add("graphics_multithreaded", SystemInfo.graphicsMultiThreaded.ToString().ToLower());
			dictionary.Add("graphics_shaderlevel", SystemInfo.graphicsShaderLevel.ToString());
			dictionary.Add("processor_frequency", SystemInfo.processorFrequency.ToString());
			dictionary.Add("processor_count", SystemInfo.processorCount.ToString());
			dictionary.Add("processor_type", SystemInfo.processorType);
			dictionary.Add("rendertargets_max", SystemInfo.supportedRenderTargetCount.ToString());
			dictionary.Add("screen_width", Screen.width.ToString());
			dictionary.Add("screen_height", Screen.height.ToString());
			dictionary.Add("screen_dpi", Screen.dpi.ToString());
			Dictionary<string, string> dictionary2 = dictionary;
			ICPSwrveService iCPSwrveService = Service.Get<ICPSwrveService>();
			foreach (KeyValuePair<string, string> item in dictionary2)
			{
				iCPSwrveService.Action(item.Key, item.Value);
			}
		}

		private IEnumerator initSwrve()
		{
			DateTime dateTime = new DateTime(2018, 12, 21, 0, 0, 0);
			ICPSwrveService instance;
			if (EnableAnalyticsLogging && dateTime.Date > DateTime.Now.Date && !Service.Get<GameSettings>().OfflineMode)
			{
				SwrveLog.OnLog += delegate(SwrveLog.LogLevel level, object message, string tag)
				{
					switch (level)
					{
					case SwrveLog.LogLevel.Verbose:
						break;
					case SwrveLog.LogLevel.Info:
						break;
					case SwrveLog.LogLevel.Warning:
						break;
					case SwrveLog.LogLevel.Error:
						Log.LogNetworkError("Swrve", message.ToString());
						break;
					}
				};
				GameObject gameObject = Service.Get<GameObject>();
				SwrveComponent swrveComponent = gameObject.AddComponent<SwrveComponent>();
				Configurator configurator = Service.Get<Configurator>();
				IDictionary<string, object> dictionaryForSystem = configurator.GetDictionaryForSystem("SwrveConfig");
				string key = "PROD";
				IDictionary<string, object> dictionary = (IDictionary<string, object>)dictionaryForSystem[key];
				string key2 = "windows";
				IDictionary<string, object> dictionary2 = (IDictionary<string, object>)dictionary[key2];
				SwrveConfig swrveConfig = new SwrveConfig();
				swrveConfig.AppVersion = ClientInfo.Instance.BuildVersion;
				swrveConfig.AutomaticSessionManagement = true;
				swrveConfig.AutoDownloadCampaignsAndResources = true;
				swrveConfig.NewSessionInterval = 1;
				swrveConfig.UseHttpsForEventsServer = true;
				swrveConfig.UseHttpsForContentServer = true;
				swrveConfig.MessagingEnabled = false;
				swrveConfig.ConversationsEnabled = false;
				swrveComponent.SDK.PushNotificationListener = new CPSwrvePushNotificationListener();
				swrveComponent.Init((int)dictionary2["AppId"], (string)dictionary2["APIKey"], swrveConfig);
				Dictionary<string, string> deviceInfo = swrveComponent.SDK.GetDeviceInfo();
				string text = deviceInfo["swrve.device_name"];
				string value = deviceInfo["swrve.os"];
				string value2 = deviceInfo["swrve.device_dpi"];
				string value3 = deviceInfo["swrve.device_width"];
				string value4 = deviceInfo["swrve.device_height"];
				Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
				dictionary3.Add("device_name", text);
				dictionary3.Add("os", value);
				dictionary3.Add("device_dpi", value2);
				dictionary3.Add("device_width", value3);
				dictionary3.Add("device_height", value4);
				if (!string.IsNullOrEmpty(swrveComponent.SDK.UserId))
				{
					dictionary3.Add("swrve_user_id", swrveComponent.SDK.UserId);
				}
				dictionary3.Add("jailbroken.is_jailbroken", SwrveManagerUtils.GetIsJailBroken());
				dictionary3.Add("lat.is_lat", SwrveManagerUtils.GetIsLat().ToString());
				string key3 = string.Empty;
				if (Application.platform == RuntimePlatform.Android)
				{
					key3 = "gida";
				}
				else if (Application.platform == RuntimePlatform.IPhonePlayer)
				{
					key3 = "idfa";
				}
				string value5 = SwrveManagerUtils.AESEncrypt(text, SwrveManagerUtils.GetAdvertiserID());
				dictionary3.Add(key3, value5);
				string rSAEncryptedKey = SwrveManagerUtils.GetRSAEncryptedKey();
				dictionary3.Add("esk", rSAEncryptedKey);
				swrveComponent.SDK.UserUpdate(dictionary3);
				instance = new CPSwrveService(swrveComponent);
			}
			else
			{
				instance = new NullCPSwrveService();
			}
			Service.Set(instance);
			yield break;
		}

		private IEnumerator initTechAnalytics()
		{
			GameObject gameObject = Service.Get<GameObject>();
			gameObject.AddComponent<TechAnalytics>();
			yield break;
		}

		public void LoggerDelegate(object sourceObject, string message, LogType logType)
		{
			switch (logType)
			{
			case LogType.Warning:
				break;
			case LogType.Log:
				break;
			case LogType.Exception:
				Log.LogFatal(sourceObject, message);
				break;
			case LogType.Error:
			case LogType.Assert:
				Log.LogError(sourceObject, message);
				break;
			}
		}
	}
}
