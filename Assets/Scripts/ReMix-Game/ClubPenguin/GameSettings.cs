using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework.Utility;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin
{
	public class GameSettings : ICommonGameSettings
	{
		public enum ScreenOrientationOption
		{
			Potrait,
			Landscape
		}

		public const string CLEAR_PREFS_ARG = "-clear-prefs";

		private readonly HashSet<ICachableType> resetableGenericSettings;

		[CanReset]
		public CacheableType<int> NumDaysPlayed
		{
			get;
			set;
		}

		[CanReset]
		public CacheableType<string> LastDayPlayed
		{
			get;
			set;
		}

		[CanReset]
		public CacheableType<bool> SfxEnabled
		{
			get;
			private set;
		}

		[CanReset]
		public CacheableType<float> SfxVolume
		{
			get;
			private set;
		}

		[CanReset]
		public CacheableType<bool> MusicEnabled
		{
			get;
			private set;
		}

		[CanReset]
		public CacheableType<float> MusicVolume
		{
			get;
			private set;
		}

		[CanReset]
		public CacheableType<Language> SavedLanguage
		{
			get;
			private set;
		}

		[CanReset]
		public CacheableType<string> LastZone
		{
			get;
			private set;
		}

		[CanReset]
		public CacheableType<float> KeyboardHeight
		{
			get;
			private set;
		}

		[CanReset]
		public DevCacheableType<bool> AutoLogin
		{
			get;
			private set;
		}

		[CanReset]
		public CacheableType<bool> EnablePushNotifications
		{
			get;
			private set;
		}

		[CanReset]
		public DevCacheableType<bool> SkipFTUE
		{
			get;
			private set;
		}

		[CanReset]
		public DevCacheableType<bool> BypassCaptcha
		{
			get;
			private set;
		}

		[CanReset]
		public CacheableType<bool> FirstLoadOfApp
		{
			get;
			private set;
		}

		[CanReset]
		public CacheableType<string> GameServerHost
		{
			get;
			private set;
		}

		[CanReset]
		public CacheableType<string> CPAPIServicehost
		{
			get;
			private set;
		}

		[CanReset]
		public CacheableType<string> GuestControllerHostUrl
		{
			get;
			private set;
		}

		[CanReset]
		public CacheableType<string> GuestControllerCDNUrl
		{
			get;
			private set;
		}

		[CanReset]
		public CacheableType<string> MixAPIHostUrl
		{
			get;
			private set;
		}

		[CanReset]
		public CacheableType<string> CDN
		{
			get;
			private set;
		}

		[CanReset]
		public CacheableType<string> CPWebsiteAPIServicehost
		{
			get;
			private set;
		}

		public bool OfflineMode
		{
			get;
			private set;
		}

		public bool FirstSession
		{
			get;
			set;
		}

		[CanReset]
		public CacheableType<bool> SeenInAppPurchaseDisclaimerPrompt
		{
			get;
			private set;
		}

		[CanReset]
		public DevCacheableType<bool> EnableAnalyticsLogging
		{
			get;
			private set;
		}

		[Tweakable("Session.PushNotifications.EnablePushNotifications", Description = "This toggles push notifications on this device. Also available in-game in Settings.")]
		public bool TogglePushNotifications
		{
			get
			{
				return EnablePushNotifications.Value;
			}
			set
			{
				EnablePushNotifications.SetValue(value);
			}
		}

		[Tweakable("Network.Game Server", Description = "Miss seeing your friends in the world?  Me too.  Hopefully some Smart Fox can figure this out and allow us to play together again.")]
		[PublicTweak(2018, 12, 25)]
		public string ChangeGameServerHost
		{
			get
			{
				return GameServerHost.Value;
			}
			set
			{
				GameServerHost.Value = value;
			}
		}

		[Tweakable("Network.Web Services.Game", Description = "Web Services: the second key.  Everything game play related.")]
		[PublicTweak(2019, 1, 20)]
		public string ChangeCPAPIServicehost
		{
			get
			{
				return CPAPIServicehost.Value;
			}
			set
			{
				CPAPIServicehost.Value = formatUrl(value);
			}
		}

		[PublicTweak(2019, 1, 20)]
		[Tweakable("Network.Web Services.Login", Description = "Web Services: the second key.  Stores account information and authenticates users.")]
		public string ChangeGuestControllerHostUrl
		{
			get
			{
				return GuestControllerHostUrl.Value;
			}
			set
			{
				GuestControllerHostUrl.Value = formatUrl(value);
			}
		}

		[PublicTweak(2019, 1, 20)]
		[Tweakable("Network.Web Services.Login Part 2", Description = "Web Services: the second key.  Why do we need two endpoints for this? I guess Disney Accounts are somewhat complicated.")]
		public string ChangeGuestControllerCDNUrl
		{
			get
			{
				return GuestControllerCDNUrl.Value;
			}
			set
			{
				GuestControllerCDNUrl.Value = formatUrl(value);
			}
		}

		[PublicTweak(2019, 1, 20)]
		[Tweakable("Network.Web Services.Account", Description = "Web Services: the second key.  More account management and friend tracking.")]
		public string ChangeMixAPIHostUrl
		{
			get
			{
				return MixAPIHostUrl.Value;
			}
			set
			{
				MixAPIHostUrl.Value = formatUrl(value);
			}
		}

		[PublicTweak(2019, 3, 29)]
		[Tweakable("Network.Content", Description = "The final key.  Master this and you'll be able to control almost everything.  It's going to be really really hard though, not even sure I could do it myself.  Best of luck and Waddle On.")]
		public string ChangeCDN
		{
			get
			{
				return CDN.Value;
			}
			set
			{
				CDN.Value = formatUrl(value);
			}
		}

		[PublicTweak(2018, 12, 21)]
		[Tweakable("Network.News", Description = "The ClubPenguin Island blog is gone, but if there's another website you like to follow put the URL here.")]
		public string ChangeCPWebsiteAPIServicehost
		{
			get
			{
				return CPWebsiteAPIServicehost.Value;
			}
			set
			{
				CPWebsiteAPIServicehost.Value = formatUrl(value);
			}
		}

		internal void SetOfflineMode(bool value)
		{
			OfflineMode = value;
		}

		public GameSettings()
		{
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			if (commandLineArgs != null && commandLineArgs.Contains("-clear-prefs"))
			{
				PlayerPrefs.DeleteAll();
			}
			NumDaysPlayed = new CacheableType<int>("cp.NumDaysPlayed", 0);
			LastDayPlayed = new CacheableType<string>("cp.LastDayPlayed", string.Empty);
			SfxEnabled = new CacheableType<bool>("cp.SfxEnabled", true);
			SfxVolume = new CacheableType<float>("cp.SfxVolume", 1f);
			MusicEnabled = new CacheableType<bool>("cp.MusicEnabled", true);
			MusicVolume = new CacheableType<float>("cp.MusicVolume", 1f);
			SavedLanguage = new CacheableType<Language>("cp.SavedLanguage", Language.none);
			LastZone = new CacheableType<string>("cp.LastZone", "");
			KeyboardHeight = new CacheableType<float>("cp.KeyboardHeight", 0.38f);
			EnablePushNotifications = new CacheableType<bool>("cp.EnablePushNotifications", true);
			SkipFTUE = new DevCacheableType<bool>("cp.SkipFTUE", false);
			BypassCaptcha = new DevCacheableType<bool>("cp.BypassCaptcha", false);
			FirstLoadOfApp = new CacheableType<bool>("cp.FirstLoadOfApp", true);
			FirstSession = FirstLoadOfApp;
			FirstLoadOfApp.SetValue(false);
			SeenInAppPurchaseDisclaimerPrompt = new CacheableType<bool>("cp.SeenInAppPurchaseDisclaimerPrompt", false);
			EnableAnalyticsLogging = new DevCacheableType<bool>("cp.EnableAnalyticsLogging", true);
			GameServerHost = new CacheableType<string>("cp.network.GameServerHost", "");
			CPAPIServicehost = new CacheableType<string>("cp.network.CPAPIServicehost", "");
			GuestControllerHostUrl = new CacheableType<string>("cp.network.GuestControllerHostUrl", "");
			GuestControllerCDNUrl = new CacheableType<string>("cp.network.GuestControllerCDNUrl", "");
			MixAPIHostUrl = new CacheableType<string>("cp.network.MixAPIHostUrl", "");
			CDN = new CacheableType<string>("cp.network.CDN", "");
			CPWebsiteAPIServicehost = new CacheableType<string>("cp.network.CPWebsiteAPIServicehost", "");
			AutoLogin = new DevCacheableType<bool>("cp.AutoLogin", true);
			resetableGenericSettings = new HashSet<ICachableType>();
			if (DateTime.UtcNow > new DateTime(2018, 12, 21))
			{
				OfflineMode = true;
			}
		}

		public void RegisterSetting(ICachableType setting, bool canBeReset)
		{
			if (canBeReset)
			{
				resetableGenericSettings.Add(setting);
			}
		}

		[Invokable("Settings.ScreenOrientation", Description = "Set screen orientation")]
		[PublicTweak]
		public void SetScreenOrientation(ScreenOrientationOption option)
		{
			switch (option)
			{
			case ScreenOrientationOption.Landscape:
				Screen.autorotateToLandscapeLeft = true;
				Screen.autorotateToLandscapeRight = true;
				Screen.autorotateToPortrait = false;
				Screen.autorotateToPortraitUpsideDown = false;
				Screen.orientation = ScreenOrientation.LandscapeLeft;
				Screen.orientation = ScreenOrientation.AutoRotation;
				break;
			case ScreenOrientationOption.Potrait:
				Screen.autorotateToLandscapeLeft = false;
				Screen.autorotateToLandscapeRight = false;
				Screen.autorotateToPortrait = true;
				Screen.autorotateToPortraitUpsideDown = true;
				Screen.orientation = ScreenOrientation.Portrait;
				Screen.orientation = ScreenOrientation.AutoRotation;
				break;
			}
		}

		[Invokable("Settings.TimeScale", Description = "Custom time scale. Play fast or slow!")]
		[PublicTweak]
		public void SetTimeScale(float scale = 1f)
		{
			Time.timeScale = scale;
			Time.fixedDeltaTime = scale * 0.02f;
		}

		[Invokable("Settings.Reset", Description = "Reset all GameSettings (PlayerPrefs) to their default values.")]
		public void Reset()
		{
			foreach (PropertyInfo item in ReflectionHelper.GetInstancePropertiesWithAttribute<CanResetAttribute>(this))
			{
				if (typeof(ICachableType).IsAssignableFrom(item.PropertyType))
				{
					ICachableType cachableType = item.GetValue(this, null) as ICachableType;
					if (cachableType != null)
					{
						cachableType.Reset();
					}
				}
			}
			foreach (ICachableType resetableGenericSetting in resetableGenericSettings)
			{
				resetableGenericSetting.Reset();
			}
		}

		[PublicTweak]
		[Invokable("Settings.Localization.ChangeLanguage", Description = "Update all the tokens with a new language.")]
		public void ChangeLanguage([ArgDescription("Language")] Language language)
		{
			Service.Get<Localizer>().ChangeLanguage(language);
			SavedLanguage.SetValue(language);
		}

		private string formatUrl(string url)
		{
			if (url != null)
			{
				url = url.Trim();
			}
			if (string.IsNullOrEmpty(url))
			{
				return url;
			}
			if (!url.Contains("://"))
			{
				url = "https://" + url;
			}
			return url;
		}
	}
}
