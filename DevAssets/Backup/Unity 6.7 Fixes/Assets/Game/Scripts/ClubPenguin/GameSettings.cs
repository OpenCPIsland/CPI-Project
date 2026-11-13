using ClubPenguin.Net;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework.Utility;
using Disney.MobileNetwork;
using Disney.LaunchPadFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tweaker.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using ClubPenguin.Core;
using ClubPenguin.Halloween;

namespace ClubPenguin
{
    public class GameSettings : ICommonGameSettings
    {
        public enum ScreenOrientationOption
        {
            Potrait,
            Landscape
        }

        public enum MembershipOverrideOption
        {
            NonMember,
            Member,
            AllAccess
        }

        private const string DIVING_UNLIMITED_AIR_KEY = "cp.DivingUnlimitedAir";
        private const string MEMBERSHIP_OVERRIDE_KEY = "cp.MembershipOverride";

        private static string GetPlatformKey(string key)
        {
#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
            if (UnityEngine.Application.isEditor)
            {
                return "Editor_" + key;
            }
#endif
            return key;
        }

        public static bool DivingUnlimitedAirSetting
        {
            get => PlayerPrefs.GetInt(GetPlatformKey(DIVING_UNLIMITED_AIR_KEY), 0) == 1;
            set
            {
                PlayerPrefs.SetInt(GetPlatformKey(DIVING_UNLIMITED_AIR_KEY), value ? 1 : 0);
                PlayerPrefs.Save();
            }
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

        [Tweakable("Network.GameServer", Description = "Miss seeing your friends in the world?  Me too.  Hopefully some Smart Fox can figure this out and allow us to play together again.")]
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

        [Tweakable("Network.WebServices.Game", Description = "Web Services: the second key.  Everything game play related.")]
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
        [Tweakable("Network.WebServices.Login", Description = "Web Services: the second key.  Stores account information and authenticates users.")]
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
        [Tweakable("Network.WebServices.LoginPart2", Description = "Web Services: the second key.  Why do we need two endpoints for this? I guess Disney Accounts are somewhat complicated.")]
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
        [Tweakable("Network.WebServices.Account", Description = "Web Services: the second key.  More account management and friend tracking.")]
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
        [Tweakable("Network.WebServices.Content", Description = "The final key.  Master this and you'll be able to control almost everything.  It's going to be really really hard though, not even sure I could do it myself.  Best of luck and Waddle On.")]
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

        [Tweakable("Network.WebServices.News", Description = "The ClubPenguin Island blog is gone, but if there's another website you like to follow put the URL here. Note: In this custom client, this feature no longer works. It was removed due to multiple security flaws.")]
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
            SfxVolume = new CacheableType<float>("cp.SfxVolume", 2f);
            MusicEnabled = new CacheableType<bool>("cp.MusicEnabled", true);
            MusicVolume = new CacheableType<float>("cp.MusicVolume", 2f);
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
            ClubPenguin.World.Activities.Diving.DivingGameController.DivingUnlimitedAir = DivingUnlimitedAirSetting;
            ApplySavedMembershipOverride();
            SceneManager.sceneLoaded += OnMembershipOverrideSceneLoaded;
        }

        private void OnMembershipOverrideSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            ApplySavedMembershipOverride();
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
        [Invokable("Settings.Reset.OfflineData.DeleteAllData", Description = "WARNING: This will delete all locally saved player data. This cannot be undone! Make sure to backup your data first before proceeding!")]
        public void DeleteAllPlayerData()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }

        [PublicTweak]
        [Invokable("Settings.Localization.ChangeLanguage", Description = "Update all the tokens with a new language.")]
        public void ChangeLanguage([ArgDescription("Language")] Language language)
        {
            Service.Get<Localizer>().ChangeLanguage(language);
            SavedLanguage.SetValue(language);
        }

        [PublicTweak]
        [Invokable("Settings.InactivityService.Enable", Description = "Enables the inactivity service.")]
        public void EnableInactivityService()
        {
            PlayerPrefs.SetInt(GetPlatformKey(InitInactivityServiceAction.InactivityServiceEnabledPlayerPrefsKey), 1);
            PlayerPrefs.Save();
            if (Service.IsSet<InactivityService>())
            {
                Service.Get<InactivityService>().SetTrackingEnabled(true);
            }
        }

        [PublicTweak]
        [Invokable("Settings.InactivityService.Disable", Description = "Disables the inactivity service.")]
        public void DisableInactivityService()
        {
            PlayerPrefs.SetInt(GetPlatformKey(InitInactivityServiceAction.InactivityServiceEnabledPlayerPrefsKey), 0);
            PlayerPrefs.Save();
            if (Service.IsSet<InactivityService>())
            {
                Service.Get<InactivityService>().SetTrackingEnabled(false);
            }
        }

        [PublicTweak]
        [Invokable("Avatar.Membership.SetOverride", Description = "Sets the local player's membership status. This is really for experimental reasons. It shouldn't be used for daily gameplay.")]
        public void SetMembershipOverride(MembershipOverrideOption option)
        {
            PlayerPrefs.SetInt(GetPlatformKey(MEMBERSHIP_OVERRIDE_KEY), (int)option);
            PlayerPrefs.Save();
            ApplyMembershipOverride(option);
        }

        public MembershipOverrideOption GetMembershipOverrideSetting()
        {
            return (MembershipOverrideOption)PlayerPrefs.GetInt(GetPlatformKey(MEMBERSHIP_OVERRIDE_KEY), (int)MembershipOverrideOption.Member);
        }

        private void ApplySavedMembershipOverride()
        {
            MembershipOverrideOption option = GetMembershipOverrideSetting();

            if (option != MembershipOverrideOption.Member)
            {
                ApplyMembershipOverride(option);
            }
        }

        private void ApplyMembershipOverride(MembershipOverrideOption option)
        {
            if (!Service.IsSet<CPDataEntityCollection>())
            {
                return;
            }
            CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
            if (cPDataEntityCollection == null || cPDataEntityCollection.LocalPlayerHandle.IsNull)
            {
                return;
            }
            MembershipData component;
            if (cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out component))
            {
                switch (option)
                {
                    case MembershipOverrideOption.NonMember:
                        component.IsMember = false;
                        component.MembershipType = MembershipType.None;
                        break;
                    case MembershipOverrideOption.Member:
                        component.IsMember = true;
                        component.MembershipType = MembershipType.Member;
                        break;
                    case MembershipOverrideOption.AllAccess:
                        component.IsMember = true;
                        component.MembershipType = MembershipType.AllAccessEventMember;
                        break;
                }
            }
        }

        public class AnnualEventKeyGenerator : NamedToggleValueAttribute.NamedToggleValueGenerator
        {
            public IEnumerable<NamedToggleValueAttribute.NamedToggleValue> GetNameToggleValues()
            {
                List<NamedToggleValueAttribute.NamedToggleValue> list = new List<NamedToggleValueAttribute.NamedToggleValue>();
                AnnualEventsController3000 controller = AnnualEventsController3000.Instance;
                if (controller != null && controller.events != null)
                {
                    foreach (AnnualEventsController3000.EventInfo @event in controller.events)
                    {
                        if (@event == null)
                        {
                            continue;
                        }

                        string id = @event.eventID ?? "";
                        string name = @event.eventName ?? "";
                        if (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(name))
                        {
                            continue;
                        }

                        string value = id + "|" + name;
                        string display = string.IsNullOrEmpty(name) ? id : name;
                        if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(name))
                        {
                            display = id + "_" + name;
                        }
                        list.Add(new NamedToggleValueAttribute.NamedToggleValue(display, value));
                    }
                }
                return list;
            }
        }

        [Invokable("PartySwitcher.Evergreen", Description = "Force regular (no parties).")]
        [PublicTweak]
        public void AnnualParties_End()
        {
            AnnualEventsController3000 controller = AnnualEventsController3000.Instance;
            if (controller != null && controller.ServerControlsParties)
            {
                return;
            }

            if (controller != null)
            {
                controller.ForceEndParty();
                PlayerPrefs.Save();
                return;
            }

            Debug.LogError("AnnualEventsController3000.Instance is null");
        }

        [Invokable("PartySwitcher.AnnualParties", Description = "Return to annual parties.")]
        [PublicTweak]
        public void AnnualParties_Default()
        {
            AnnualEventsController3000 controller = AnnualEventsController3000.Instance;
            if (controller != null && controller.ServerControlsParties)
            {
                return;
            }

            if (controller != null)
            {
                controller.SetDefaultMode();
                PlayerPrefs.Save();
                return;
            }

            Debug.LogError("AnnualEventsController3000.Instance is null");
        }

        [Invokable("PartySwitcher.Parties", Description = "Force a specific party on.")]
        [PublicTweak]
        public void AnnualParties_Switch([NamedToggleValue(typeof(AnnualEventKeyGenerator), 0u)][ArgDescription("Change rooms for this setting to take effect.")] string Party)
        {
            AnnualEventsController3000 controller = AnnualEventsController3000.Instance;
            if (controller != null && controller.ServerControlsParties)
            {
                return;
            }

            if (controller != null)
            {
                controller.ForceEndParty();
                controller.ForcePartyKey(Party);
                PlayerPrefs.Save();
                return;
            }

            Debug.LogError("AnnualEventsController3000.Instance is null");
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

        [PublicTweak]
        [Invokable("Avatar.DivingAir.ForceOn", Description = "Forces unlimited air for the local player in diving.")]
        public void ForceDivingAirOn()
        {
            PlayerPrefs.SetInt(GetPlatformKey(DIVING_UNLIMITED_AIR_KEY), 1);
            PlayerPrefs.Save();

            ClubPenguin.World.Activities.Diving.DivingGameController.DivingUnlimitedAir = true;

            Service.Get<EventDispatcher>().DispatchEvent(new ClubPenguin.World.Activities.Diving.DivingEvents.EnableLocalInfiniteAir());
        }

        [PublicTweak]
        [Invokable("Avatar.DivingAir.ForceDefault", Description = "Restores normal air depletion behavior.")]
        public void ForceDivingAirDefault()
        {
            PlayerPrefs.SetInt(GetPlatformKey(DIVING_UNLIMITED_AIR_KEY), 0);
            PlayerPrefs.Save();

            ClubPenguin.World.Activities.Diving.DivingGameController.DivingUnlimitedAir = false;

            Service.Get<EventDispatcher>().DispatchEvent(new ClubPenguin.World.Activities.Diving.DivingEvents.DisableLocalInfiniteAir());
        }

        [PublicTweak]
        [Invokable("CFC.ResetTotal", Description = "Resets the PlayerPrefs for the Coins for Change total back to 0. Note: This will reset all donations for every player.")]
        public void ResetCFCTotal()
        {
            ProgressOverTime.ResetDeviceDonationProgress();
            CoinsForChangeTracker cfc = UnityEngine.Object.FindFirstObjectByType<CoinsForChangeTracker>();
            if (cfc != null)
            {
                cfc.ResetCoinCount();
            }
            PlayerPrefs.DeleteKey(GetPlatformKey("ol.CFCDonationTotal.device"));
            PlayerPrefs.Save();
            Service.Get<INetworkServicesManager>().ScheduledEventService.GetCFCDonations();
        }

        [PublicTweak]
        [Invokable("CFC.SetMaxTotal", Description = "Sets the PlayerPrefs for the Coins for Change total to max value of 999999999.")]
        public void SetCFCTotalToMax()
        {
            PlayerPrefs.SetString(GetPlatformKey("ol.CFCDonationTotal.device"), "999999999");
            PlayerPrefs.Save();
            CoinsForChangeTracker cfc = UnityEngine.Object.FindAnyObjectByType<CoinsForChangeTracker>();
            if (cfc != null)
            {
                cfc.SetCoinCount(999999999);
            }
        }
    }
}