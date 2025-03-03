using ClubPenguin.Net;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.Mix.SDK;
using Disney.Mix.SDK.Internal;
using Disney.Mix.SDK.Internal.GuestControllerDomain;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ClubPenguin.Mix
{
	public class OfflineLoginCreateService : MixLoginCreateService
	{
		private const int MAX_RETRIES = 3;

		private const int RETRY_DELAY = 2;

		private const string LAST_LOGIN_USER = "ol.lastLoginUserName";

		private IKeychain keychainData;

		private string storageDir = Path.Combine(Application.persistentDataPath, "MixSDK");

		private NetworkServicesConfig config;

		private Localizer localizer;

		private ICoroutine retryCoroutine;

		public override bool NetworkConfigIsNotSet
		{
			get
			{
				return config.Equals(default(NetworkServicesConfig));
			}
		}

		public override bool RegistrationConfigIsNotSet
		{
			get
			{
				return base.RegistrationAgeBand == null;
			}
		}

		public OfflineLoginCreateService()
		{
			keychainData = Service.Get<IKeychain>();
			localizer = Service.Get<Localizer>();
		}

		public override void SetNetworkConfig(NetworkServicesConfig config)
		{
			this.config = config;
			base.IOSProvisioningId = config.iOSProvisioningId;
			base.GcmSenderId = config.GcmSenderId;
		}

		public override void GetRegistrationConfig(Action<IGetRegistrationConfigurationResult> callback = null, RetryOperation retryOperation = null)
		{
			if (NetworkConfigIsNotSet)
			{
				throw new NullReferenceException("NetworkServicesConfig not set");
			}
			base.IsFetchingRegConfig = true;
			if (callback == null)
			{
				onGetRegistrationConfigStart();
				CoroutineRunner.StartPersistent(registractionCallback(), this, "get registration");
			}
		}

		private IEnumerator registractionCallback()
		{
			yield return null;
			onGetRegistrationConfigComplete(true);
			base.IsFetchingRegConfig = false;
			base.RegistrationConfig = new RegistrationConfiguration(new SiteConfigurationData(), new OfflineAgeBandBuilder());
			string mixLanguage = Service.Get<Localizer>().LanguageString;
			getAgeBand(1, mixLanguage);
		}

		private void getAgeBand(int age, string languageCode, RetryOperation retryOperation = null)
		{
			onGetAgeBandStart();
			CoroutineRunner.StartPersistent(delayGetAgeBand(), this, "delay get age band");
		}

		private IEnumerator delayGetAgeBand()
		{
			yield return null;
			onGetAgeBandComplete(true);
			base.IsFetchingRegConfig = false;
			RegConfigAttempts = 0;
			base.RegistrationAgeBand = OfflineAgeBandBuilder.GenerateOfflineAgeBand();
			onRegistrationConfigUpdated(base.RegistrationConfig);
		}

		private void getUpdateAgeBand(int age, string languageCode, RetryOperation retryOperation = null)
		{
			onGetUpdateAgeBandStart();
			CoroutineRunner.StartPersistent(delayGetUpdateAgeBand(), this, "delay get age band");
		}

		private IEnumerator delayGetUpdateAgeBand()
		{
			yield return null;
			onGetUpdateAgeBandComplete(true);
			base.IsFetchingRegConfig = false;
			RegConfigAttempts = 0;
			base.UpdateAgeBand = OfflineAgeBandBuilder.GenerateOfflineAgeBand();
			onRegistrationConfigUpdated(base.RegistrationConfig);
		}

		public override void ResetUpdateAgeBand()
		{
			base.UpdateAgeBand = null;
		}

		public override void SoftLogin()
		{
			if (NetworkConfigIsNotSet)
			{
				throw new NullReferenceException("NetworkServicesConfig not set");
			}
			string languageString = Service.Get<Localizer>().LanguageString;
			onSoftLoginStart();
			CoroutineRunner.StartPersistent(restoreLastSession(), this, "restore last session");
		}

		private IEnumerator restoreLastSession()
		{
			yield return null;
			string userName = PlayerPrefs.GetString("ol.lastLoginUserName");
			if (!string.IsNullOrEmpty(userName))
			{
				onSoftLoginCompleteSuccess(0);
				if (base.RegistrationConfig != null)
				{
					getUpdateAgeBand(1, Service.Get<Localizer>().LanguageString);
				}
				onLoginSuccess(new OfflineSession(userName));
			}
			else
			{
				if (base.RegistrationConfig == null || base.RegistrationAgeBand == null)
				{
					GetRegistrationConfig();
				}
				onSoftLoginFailed(null);
			}
		}

		public override void Login(string username, string password)
		{
			if (NetworkConfigIsNotSet)
			{
				throw new NullReferenceException("NetworkServicesConfig not set");
			}
			string languageString = Service.Get<Localizer>().LanguageString;
			onLoginStart();
			CoroutineRunner.StartPersistent(delayLogin(username, password), this, "delay login");
		}

		private IEnumerator delayLogin(string username, string password)
		{
			yield return null;
			onLoginCompleteSuccess(0);
			if (base.RegistrationConfig != null)
			{
				getUpdateAgeBand(1, Service.Get<Localizer>().LanguageString);
			}
			PlayerPrefs.SetString("ol.lastLoginUserName", username);
			onLoginSuccess(new OfflineSession(username));
		}

		public override void LogoutLastSession(AsynchOnFinishedManifold asynchOnFinishedManifold = null)
		{
			if (NetworkConfigIsNotSet)
			{
				throw new NullReferenceException("NetworkServicesConfig not set");
			}
			string languageString = Service.Get<Localizer>().LanguageString;
			if (asynchOnFinishedManifold != null)
			{
				asynchOnFinishedManifold.AsynchStart();
			}
			CoroutineRunner.StartPersistent(delayLogoutLastSession(asynchOnFinishedManifold), this, "delayLogoutLastSession");
		}

		private IEnumerator delayLogoutLastSession(AsynchOnFinishedManifold asynchOnFinishedManifold)
		{
			yield return null;
			PlayerPrefs.DeleteKey("ol.lastLoginUserName");
			if (asynchOnFinishedManifold != null)
			{
				asynchOnFinishedManifold.AsynchFinished();
			}
		}

		public override void CreateChildAccount(string firstName, string username, string parentEmail, string password, string language, IEnumerable<KeyValuePair<IMarketingItem, bool>> marketing, IEnumerable<ILegalDocument> legal)
		{
			if (NetworkConfigIsNotSet)
			{
				throw new NullReferenceException("NetworkServicesConfig not set");
			}
			onCreateChildAccountStart();
			CoroutineRunner.StartPersistent(registerAccount(firstName, username, parentEmail, password), this, "register account");
		}

		private IEnumerator registerAccount(string firstName, string username, string parentEmail, string password)
		{
			OfflineRegistrationProfile profile = OfflineRegistrationProfile.Load(username);
			profile.FirstName = firstName;
			profile.Username = username;
			profile.ParentEmail = parentEmail;
			profile.Password = password;
			profile.Save();
			yield return null;
			ISession session = new OfflineSession(username);
			onCreateChildAccountComplete(new RegisterResult(true, session, null));
			onCreateSuccess(session);
		}

		public override void UpdateProfile(string firstName, string parentEmail, IEnumerable<ILegalDocument> legal, ILocalUser localUser)
		{
			if (NetworkConfigIsNotSet)
			{
				throw new NullReferenceException("NetworkServicesConfig not set");
			}
			CoroutineRunner.StartPersistent(delayProfileUpdated(), this, "delay profile update");
		}

		private IEnumerator delayProfileUpdated()
		{
			yield return null;
			onProfileUpdateSuccess();
		}

		public override void PasswordRecoverySend(string lookupValue)
		{
			if (NetworkConfigIsNotSet)
			{
				throw new NullReferenceException("NetworkServicesConfig not set");
			}
			CoroutineRunner.StartPersistent(delayRecovery(), this, "delay password recovery");
		}

		private IEnumerator delayRecovery()
		{
			yield return null;
			onRecoverySuccess();
		}

		public override void UsernameRecoverySend(string lookupValue)
		{
			if (NetworkConfigIsNotSet)
			{
				throw new NullReferenceException("NetworkServicesConfig not set");
			}
			CoroutineRunner.StartPersistent(delayRecovery(), this, "delay username recovery");
		}

		public override void MultipleAccountsResolutionSend(string lookupValue)
		{
			if (NetworkConfigIsNotSet)
			{
				throw new NullReferenceException("NetworkServicesConfig not set");
			}
			CoroutineRunner.StartPersistent(delayMultipleAccountsResolution(), this, "delay MultipleAccountsResolution");
		}

		private IEnumerator delayMultipleAccountsResolution()
		{
			yield return null;
			onMultipleAccountsResolutionSuccess();
		}

		public override void NonRegisteredTransactorUpgradeSend(string lookupValue)
		{
			if (NetworkConfigIsNotSet)
			{
				throw new NullReferenceException("NetworkServicesConfig not set");
			}
			CoroutineRunner.StartPersistent(delayNonRegisteredTransactorUpgrade(), this, "delay NonRegisteredTransactorUpgrade");
		}

		private IEnumerator delayNonRegisteredTransactorUpgrade()
		{
			yield return null;
			onNRTUpgradeSuccess();
		}

		public override void ParentalApprovalEmailSend(ILocalUser localUser)
		{
			if (NetworkConfigIsNotSet)
			{
				throw new NullReferenceException("NetworkServicesConfig not set");
			}
			CoroutineRunner.StartPersistent(delayParentalApprovalEmail(), this, "delay ParentalApprovalEmail");
		}

		private IEnumerator delayParentalApprovalEmail()
		{
			yield return null;
			onParentalApprovalEmailSendSuccess();
		}

		public override void ValidateUsernamePassword(string username, string password)
		{
			CoroutineRunner.StartPersistent(delayValidateUsernamePassword(), this, "delay ValidateUsernamePassword");
		}

		private IEnumerator delayValidateUsernamePassword()
		{
			yield return null;
			onValidationSuccess();
		}
	}
}
