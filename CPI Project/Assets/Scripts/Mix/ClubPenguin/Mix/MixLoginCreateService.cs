using ClubPenguin.Analytics;
using ClubPenguin.Net;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Tweaker.Core;
using UnityEngine;

namespace ClubPenguin.Mix
{
	public class MixLoginCreateService
	{
		public class RetryOperation
		{
			public MethodInfo Method;

			public object[] Args;

			public int retryAttempt;

			public int retryDelay;

			public int maxRetries;
		}

		private const int MAX_RETRIES = 3;

		private const int RETRY_DELAY = 2;

		public bool ValidationInProgress;

		private INewAccountValidator accountValidator;

		private ISessionStarter sessionStarter;

		private IKeychain keychainData;

		private string storageDir = Path.Combine(Application.persistentDataPath, "MixSDK");

		private MixCoroutineManager coroutineManager;

		private NetworkServicesConfig config;

		private MixLogger logger;

		private Localizer localizer;

		public int RegConfigAttempts = 0;

		private ICoroutine retryCoroutine;

		private static DevCacheableType<string> overrideCountryCode;

		private static DevCacheableType<string> spoofedGeoIp;

		public IAgeBand RegistrationAgeBand
		{
			get;
			protected set;
		}

		public IAgeBand UpdateAgeBand
		{
			get;
			protected set;
		}

		public IRegistrationConfiguration RegistrationConfig
		{
			get;
			protected set;
		}

		public bool IsEmbargoed
		{
			get;
			protected set;
		}

		public string IOSProvisioningId
		{
			get;
			protected set;
		}

		public string GcmSenderId
		{
			get;
			protected set;
		}

		public bool IsFetchingRegConfig
		{
			get;
			protected set;
		}

		[Tweakable("Session.CountryCode")]
		public static string OverrideCountryCode
		{
			get
			{
				return overrideCountryCode.Value;
			}
			set
			{
				overrideCountryCode.SetValue(value);
				Service.Get<MixLoginCreateService>().GetRegistrationConfig();
			}
		}

		[Tweakable("Session.SpoofedGeoIP", Description = "Set a Geo IP to override current location")]
		public static string SpoofedGeoIP
		{
			get
			{
				return string.IsNullOrEmpty(spoofedGeoIp.Value) ? null : spoofedGeoIp.Value;
			}
			set
			{
				spoofedGeoIp.Value = value;
				Service.Get<MixLoginCreateService>().GetRegistrationConfig();
			}
		}

		public virtual bool NetworkConfigIsNotSet
		{
			get
			{
				return config.Equals(default(NetworkServicesConfig));
			}
		}

		public virtual bool RegistrationConfigIsNotSet
		{
			get
			{
				return RegistrationAgeBand == null;
			}
		}

		public event Action<ISession> OnLoginSuccess;

		public event Action<ILoginResult> OnLoginFailed;

		public event Action<IRestoreLastSessionResult> OnSoftLoginFailed;

		public event Action<ISession> OnCreateSuccess;

		public event Action<ISession> OnMissingInfoLoginSuccess;

		public event Action<ISession> OnRequiresLegalMarketingUpdateLoginSuccess;

		public event Action<ISession> OnParentalConsentRequiredLoginSuccess;

		public event Action<IRegisterResult> OnCreateFailed;

		public event System.Action OnValidationSuccess;

		public event Action<IValidateNewAccountResult> OnValidationFailed;

		public event Action<IRegistrationConfiguration> OnRegistrationConfigUpdated;

		public event Action<string> OnRegistrationConfigError;

		public event System.Action OnRecoverySuccess;

		public event Action<ISendUsernameRecoveryResult> OnUsernameRecoveryFailure;

		public event Action<ISendPasswordRecoveryResult> OnPasswordRecoveryFailure;

		public event System.Action OnMultipleAccountsResolutionSuccess;

		public event Action<ISendMultipleAccountsResolutionResult> OnMultipleAccountsResolutionFailure;

		public event System.Action OnNRTUpgradeSuccess;

		public event Action<ISendNonRegisteredTransactorUpgradeResult> OnNRTUpgradeFailure;

		public event System.Action OnParentalApprovalEmailSendSuccess;

		public event System.Action OnParentalApprovalEmailSendFailure;

		public event System.Action OnProfileUpdateSuccess;

		public event Action<IUpdateProfileResult> OnProfileUpdateFailed;

		public event System.Action OnGetRegistrationConfigStart;

		public event Action<bool> OnGetRegistrationConfigComplete;

		public event System.Action OnGetAgeBandStart;

		public event Action<bool> OnGetAgeBandComplete;

		public event System.Action OnGetUpdateAgeBandStart;

		public event Action<bool> OnGetUpdateAgeBandComplete;

		public event System.Action OnCreateChildAccountStart;

		public event Action<IRegisterResult> OnCreateChildAccountComplete;

		public event System.Action OnSoftLoginStart;

		public event Action<int> OnSoftLoginCompleteSuccess;

		public event System.Action OnLoginStart;

		public event Action<int> OnLoginCompleteSuccess;

		protected void onLoginSuccess(ISession value)
		{
			this.OnLoginSuccess.InvokeSafe(value);
		}

		protected void onLoginFailed(ILoginResult value)
		{
			this.OnLoginFailed.InvokeSafe(value);
		}

		protected void onSoftLoginFailed(IRestoreLastSessionResult value)
		{
			this.OnSoftLoginFailed.InvokeSafe(value);
		}

		protected void onCreateSuccess(ISession value)
		{
			this.OnCreateSuccess.InvokeSafe(value);
		}

		protected void onValidationSuccess()
		{
			this.OnValidationSuccess.InvokeSafe();
		}

		protected void onRegistrationConfigUpdated(IRegistrationConfiguration value)
		{
			this.OnRegistrationConfigUpdated.InvokeSafe(value);
		}

		protected void onRecoverySuccess()
		{
			this.OnRecoverySuccess.InvokeSafe();
		}

		protected void onMultipleAccountsResolutionSuccess()
		{
			this.OnMultipleAccountsResolutionSuccess.InvokeSafe();
		}

		protected void onNRTUpgradeSuccess()
		{
			this.OnNRTUpgradeSuccess.InvokeSafe();
		}

		protected void onParentalApprovalEmailSendSuccess()
		{
			this.OnParentalApprovalEmailSendSuccess.InvokeSafe();
		}

		protected void onProfileUpdateSuccess()
		{
			this.OnProfileUpdateSuccess.InvokeSafe();
		}

		protected void onGetRegistrationConfigStart()
		{
			this.OnGetRegistrationConfigStart.InvokeSafe();
		}

		protected void onGetRegistrationConfigComplete(bool value)
		{
			this.OnGetRegistrationConfigComplete.InvokeSafe(value);
		}

		protected void onGetAgeBandStart()
		{
			this.OnGetAgeBandStart.InvokeSafe();
		}

		protected void onGetAgeBandComplete(bool value)
		{
			this.OnGetAgeBandComplete.InvokeSafe(value);
		}

		protected void onGetUpdateAgeBandStart()
		{
			this.OnGetUpdateAgeBandStart.InvokeSafe();
		}

		protected void onGetUpdateAgeBandComplete(bool value)
		{
			this.OnGetUpdateAgeBandComplete.InvokeSafe(value);
		}

		protected void onCreateChildAccountStart()
		{
			this.OnCreateChildAccountStart.InvokeSafe();
		}

		protected void onCreateChildAccountComplete(IRegisterResult value)
		{
			this.OnCreateChildAccountComplete.InvokeSafe(value);
		}

		protected void onSoftLoginStart()
		{
			this.OnSoftLoginStart.InvokeSafe();
		}

		protected void onSoftLoginCompleteSuccess(int value)
		{
			this.OnSoftLoginCompleteSuccess.InvokeSafe(value);
		}

		protected void onLoginStart()
		{
			this.OnLoginStart.InvokeSafe();
		}

		protected void onLoginCompleteSuccess(int value)
		{
			this.OnLoginCompleteSuccess.InvokeSafe(value);
		}

		public MixLoginCreateService()
		{
			keychainData = Service.Get<IKeychain>();
			sessionStarter = new SessionStarter();
			coroutineManager = new MixCoroutineManager();
			logger = new MixLogger();
			localizer = Service.Get<Localizer>();
			Localizer obj = localizer;
			obj.TokensUpdated = (Localizer.TokensUpdatedDelegate)Delegate.Combine(obj.TokensUpdated, new Localizer.TokensUpdatedDelegate(onLanguageChanged));
			overrideCountryCode = new DevCacheableType<string>("mix.OverrideCountryCode", "");
			spoofedGeoIp = new DevCacheableType<string>("mix.SpoofedGeoIP", null);
			Service.Get<ICommonGameSettings>().RegisterSetting(overrideCountryCode, true);
			Service.Get<ICommonGameSettings>().RegisterSetting(spoofedGeoIp, true);
		}

		private void onLanguageChanged()
		{
			GetRegistrationConfig();
		}

		public virtual void SetNetworkConfig(NetworkServicesConfig config)
		{
			this.config = config;
			IOSProvisioningId = config.iOSProvisioningId;
			GcmSenderId = config.GcmSenderId;
			accountValidator = new NewAccountValidator(keychainData, logger, storageDir, config.GuestControllerHostUrl, null, config.DisneyIdClientId, coroutineManager);
		}

		public virtual void GetRegistrationConfig(Action<IGetRegistrationConfigurationResult> callback = null, RetryOperation retryOperation = null)
		{
			if (NetworkConfigIsNotSet)
			{
				throw new NullReferenceException("NetworkServicesConfig not set");
			}
			if (retryOperation == null)
			{
				retryOperation = new RetryOperation();
				retryOperation.Method = GetType().GetMethod("GetRegistrationConfig", BindingFlags.Instance | BindingFlags.Public);
				retryOperation.Args = new object[1]
				{
					callback
				};
				retryOperation.retryAttempt = 0;
				retryOperation.retryDelay = 2;
				retryOperation.maxRetries = 3;
			}
			RegistrationConfigurationGetter registrationConfigurationGetter = new RegistrationConfigurationGetter(keychainData, logger, storageDir, config.GuestControllerCDNUrl, SpoofedGeoIP, config.DisneyIdClientId, coroutineManager, config.MixAPIHostUrl, config.MixClientToken);
			IsFetchingRegConfig = true;
			if (callback == null)
			{
				if (this.OnGetRegistrationConfigStart != null)
				{
					this.OnGetRegistrationConfigStart();
				}
				Action<IGetRegistrationConfigurationResult> callback2 = delegate(IGetRegistrationConfigurationResult result)
				{
					if (this.OnGetRegistrationConfigComplete != null)
					{
						this.OnGetRegistrationConfigComplete(result.Success);
					}
					IsFetchingRegConfig = false;
					if (!result.Success)
					{
						if (result is IGetRegistrationConfigurationEmbargoedCountryResult)
						{
							IsEmbargoed = true;
						}
						else
						{
							if (retryCoroutine != null && !retryCoroutine.Disposed)
							{
								retryCoroutine.Stop();
							}
							retryCoroutine = CoroutineRunner.StartPersistent(executeRetryOperation(retryOperation), this, "Registration Config Retry");
						}
					}
					else
					{
						RegistrationConfig = result.Configuration;
						string languageString = Service.Get<Localizer>().LanguageString;
						getAgeBand(1, languageString);
					}
				};
				if (string.IsNullOrEmpty(OverrideCountryCode))
				{
					registrationConfigurationGetter.Get(callback2);
				}
				else
				{
					registrationConfigurationGetter.Get(OverrideCountryCode, callback2);
				}
			}
			else if (string.IsNullOrEmpty(OverrideCountryCode))
			{
				registrationConfigurationGetter.Get(callback);
			}
			else
			{
				registrationConfigurationGetter.Get(OverrideCountryCode, callback);
			}
		}

		private void getAgeBand(int age, string languageCode, RetryOperation retryOperation = null)
		{
			if (this.OnGetAgeBandStart != null)
			{
				this.OnGetAgeBandStart();
			}
			if (retryOperation == null)
			{
				retryOperation = new RetryOperation();
				retryOperation.Method = GetType().GetMethod("getAgeBand", BindingFlags.Instance | BindingFlags.NonPublic);
				retryOperation.Args = new object[2]
				{
					age,
					languageCode
				};
				retryOperation.retryAttempt = 0;
				retryOperation.retryDelay = 2;
				retryOperation.maxRetries = 3;
			}
			RegistrationConfig.GetRegistrationAgeBand(age, languageCode, delegate(IGetAgeBandResult result)
			{
				if (this.OnGetAgeBandComplete != null)
				{
					this.OnGetAgeBandComplete(result.Success);
				}
				IsFetchingRegConfig = false;
				if (result.Success)
				{
					RegConfigAttempts = 0;
					RegistrationAgeBand = result.AgeBand;
					Dictionary<string, string> attributes = new Dictionary<string, string>
					{
						{
							"swrve.device_region",
							RegistrationAgeBand.CountryCode
						}
					};
					Service.Get<ICPSwrveService>().UserUpdate(attributes);
					if (this.OnRegistrationConfigUpdated != null)
					{
						this.OnRegistrationConfigUpdated(RegistrationConfig);
					}
				}
				else
				{
					if (retryCoroutine != null && !retryCoroutine.Disposed)
					{
						retryCoroutine.Stop();
					}
					retryCoroutine = CoroutineRunner.StartPersistent(executeRetryOperation(retryOperation), this, "Registration AgeBand retry");
				}
			});
		}

		private void getUpdateAgeBand(int age, string languageCode, RetryOperation retryOperation = null)
		{
			if (this.OnGetUpdateAgeBandStart != null)
			{
				this.OnGetUpdateAgeBandStart();
			}
			if (retryOperation == null)
			{
				retryOperation = new RetryOperation();
				retryOperation.Method = GetType().GetMethod("getUpdateAgeBand", BindingFlags.Instance | BindingFlags.NonPublic);
				retryOperation.Args = new object[2]
				{
					age,
					languageCode
				};
				retryOperation.retryAttempt = 0;
				retryOperation.retryDelay = 2;
				retryOperation.maxRetries = 3;
			}
			RegistrationConfig.GetUpdateAgeBand(age, languageCode, delegate(IGetAgeBandResult result)
			{
				if (this.OnGetUpdateAgeBandComplete != null)
				{
					this.OnGetUpdateAgeBandComplete(result.Success);
				}
				IsFetchingRegConfig = false;
				if (result.Success)
				{
					RegConfigAttempts = 0;
					UpdateAgeBand = result.AgeBand;
					if (this.OnRegistrationConfigUpdated != null)
					{
						this.OnRegistrationConfigUpdated(RegistrationConfig);
					}
				}
				else
				{
					if (retryCoroutine != null && !retryCoroutine.Disposed)
					{
						retryCoroutine.Stop();
					}
					retryCoroutine = CoroutineRunner.StartPersistent(executeRetryOperation(retryOperation), this, "Update AgeBand retry");
				}
			});
		}

		public virtual void ResetUpdateAgeBand()
		{
			UpdateAgeBand = null;
		}

		public virtual void SoftLogin()
		{
			if (NetworkConfigIsNotSet)
			{
				throw new NullReferenceException("NetworkServicesConfig not set");
			}
			string languageString = Service.Get<Localizer>().LanguageString;
			if (this.OnSoftLoginStart != null)
			{
				this.OnSoftLoginStart();
			}
			sessionStarter.RestoreLastSession(config.GuestControllerHostUrl, SpoofedGeoIP, config.MixAPIHostUrl, config.DisneyIdClientId, config.MixClientToken, config.ClientVersion, coroutineManager, keychainData, logger, storageDir, languageString, delegate(IRestoreLastSessionResult result)
			{
				if (result.Success)
				{
					if (this.OnSoftLoginCompleteSuccess != null)
					{
						this.OnSoftLoginCompleteSuccess(result.Session.LocalUser.Friends.Count());
					}
					if (RegistrationConfig != null)
					{
						if (result.Session.LocalUser.RegistrationProfile.DateOfBirth.HasValue)
						{
							int age = CalculateAge(result.Session.LocalUser.RegistrationProfile.DateOfBirth.Value);
							getUpdateAgeBand(age, Service.Get<Localizer>().LanguageString);
						}
						else
						{
							getUpdateAgeBand(1, Service.Get<Localizer>().LanguageString);
						}
					}
					else if (result is IRestoreLastSessionSuccessRequiresLegalMarketingUpdateResult)
					{
						Log.LogError(this, "Logout after soft login due to missing config");
						result.Session.LogOut(delegate
						{
						});
						GetRegistrationConfig();
						if (this.OnSoftLoginFailed != null)
						{
							this.OnSoftLoginFailed(result);
						}
						return;
					}
					if (result is IRestoreLastSessionSuccessMissingInfoResult)
					{
						if (this.OnMissingInfoLoginSuccess != null)
						{
							this.OnMissingInfoLoginSuccess(result.Session);
						}
					}
					else if (result is IRestoreLastSessionSuccessRequiresLegalMarketingUpdateResult)
					{
						if (this.OnRequiresLegalMarketingUpdateLoginSuccess != null)
						{
							this.OnRequiresLegalMarketingUpdateLoginSuccess(result.Session);
						}
					}
					else if (result is IRestoreLastSessionFailedParentalConsentResult || result.Session.LocalUser.RegistrationProfile.AccountStatus == AccountStatus.AwaitingParentalConsent)
					{
						if (this.OnParentalConsentRequiredLoginSuccess != null)
						{
							this.OnParentalConsentRequiredLoginSuccess(result.Session);
						}
					}
					else if (this.OnLoginSuccess != null)
					{
						this.OnLoginSuccess(result.Session);
					}
				}
				else
				{
					if (RegistrationConfig == null || RegistrationAgeBand == null)
					{
						GetRegistrationConfig();
					}
					if (this.OnSoftLoginFailed != null)
					{
						this.OnSoftLoginFailed(result);
					}
				}
			});
		}

		public virtual int CalculateAge(DateTime BirthDate)
		{
			int num = DateTime.Now.Year - BirthDate.Year;
			if (DateTime.Now.Month < BirthDate.Month || (DateTime.Now.Month == BirthDate.Month && DateTime.Now.Day < BirthDate.Day))
			{
				num--;
			}
			return num;
		}

		public virtual void Login(string username, string password)
		{
			if (NetworkConfigIsNotSet)
			{
				throw new NullReferenceException("NetworkServicesConfig not set");
			}
			string languageString = Service.Get<Localizer>().LanguageString;
			if (this.OnLoginStart != null)
			{
				this.OnLoginStart();
			}
			sessionStarter.Login(config.GuestControllerHostUrl, SpoofedGeoIP, config.MixAPIHostUrl, config.DisneyIdClientId, config.MixClientToken, config.ClientVersion, coroutineManager, keychainData, logger, storageDir, username, password, languageString, delegate(ILoginResult result)
			{
				if (result.Success)
				{
					if (this.OnLoginCompleteSuccess != null)
					{
						this.OnLoginCompleteSuccess(result.Session.LocalUser.Friends.Count());
					}
					if (RegistrationConfig != null)
					{
						if (result.Session.LocalUser.RegistrationProfile.DateOfBirth.HasValue)
						{
							int age = CalculateAge(result.Session.LocalUser.RegistrationProfile.DateOfBirth.Value);
							getUpdateAgeBand(age, Service.Get<Localizer>().LanguageString);
						}
						else
						{
							getUpdateAgeBand(1, Service.Get<Localizer>().LanguageString);
						}
					}
					else if (result is ILoginRequiresLegalMarketingUpdateResult)
					{
						Log.LogError(this, "Logout after login due to missing config");
						result.Session.LogOut(delegate
						{
						});
						GetRegistrationConfig();
						if (this.OnLoginFailed != null)
						{
							this.OnLoginFailed(result);
						}
						return;
					}
					if (result is ILoginMissingInfoResult)
					{
						if (this.OnMissingInfoLoginSuccess != null)
						{
							this.OnMissingInfoLoginSuccess(result.Session);
						}
					}
					else if (result is ILoginRequiresLegalMarketingUpdateResult)
					{
						if (this.OnRequiresLegalMarketingUpdateLoginSuccess != null)
						{
							this.OnRequiresLegalMarketingUpdateLoginSuccess(result.Session);
						}
					}
					else if (result is ILoginFailedParentalConsentResult || result.Session.LocalUser.RegistrationProfile.AccountStatus == AccountStatus.AwaitingParentalConsent)
					{
						if (this.OnParentalConsentRequiredLoginSuccess != null)
						{
							this.OnParentalConsentRequiredLoginSuccess(result.Session);
						}
					}
					else if (this.OnLoginSuccess != null)
					{
						this.OnLoginSuccess(result.Session);
					}
				}
				else
				{
					if (RegistrationConfig == null || RegistrationAgeBand == null)
					{
						GetRegistrationConfig();
					}
					if (this.OnLoginFailed != null)
					{
						this.OnLoginFailed(result);
					}
				}
			});
		}

		public virtual void LogoutLastSession(AsynchOnFinishedManifold asynchOnFinishedManifold = null)
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
			sessionStarter.OfflineLastSession(config.GuestControllerHostUrl, SpoofedGeoIP, config.MixAPIHostUrl, config.DisneyIdClientId, config.MixClientToken, config.ClientVersion, coroutineManager, keychainData, logger, storageDir, languageString, delegate(IOfflineLastSessionResult result)
			{
				try
				{
					if (result.Success && result.Session != null)
					{
						result.Session.LogOut(delegate
						{
							LogoutLastSession(asynchOnFinishedManifold);
						});
					}
				}
				finally
				{
					if (asynchOnFinishedManifold != null)
					{
						asynchOnFinishedManifold.AsynchFinished();
					}
				}
			});
		}

		public virtual void CreateChildAccount(string firstName, string username, string parentEmail, string password, string language, IEnumerable<KeyValuePair<IMarketingItem, bool>> marketing, IEnumerable<ILegalDocument> legal)
		{
			if (NetworkConfigIsNotSet)
			{
				throw new NullReferenceException("NetworkServicesConfig not set");
			}
			if (this.OnCreateChildAccountStart != null)
			{
				this.OnCreateChildAccountStart();
			}
			sessionStarter.RegisterChildAccount(config.GuestControllerHostUrl, SpoofedGeoIP, config.MixAPIHostUrl, config.DisneyIdClientId, config.MixClientToken, config.ClientVersion, coroutineManager, keychainData, logger, storageDir, false, username, password, firstName, parentEmail, language, null, marketing, legal, delegate(IRegisterResult result)
			{
				if (this.OnCreateChildAccountComplete != null)
				{
					this.OnCreateChildAccountComplete(result);
				}
				if (result.Success)
				{
					if (this.OnCreateSuccess != null)
					{
						this.OnCreateSuccess(result.Session);
					}
				}
				else if (this.OnCreateFailed != null)
				{
					this.OnCreateFailed(result);
				}
			});
		}

		public virtual void UpdateProfile(string firstName, string parentEmail, IEnumerable<ILegalDocument> legal, ILocalUser localUser)
		{
			if (NetworkConfigIsNotSet)
			{
				throw new NullReferenceException("NetworkServicesConfig not set");
			}
			localUser.UpdateProfile(firstName, null, null, null, parentEmail, null, null, legal, onProfileUpdated);
		}

		private void onProfileUpdated(IUpdateProfileResult result)
		{
			if (result.Success)
			{
				if (this.OnProfileUpdateSuccess != null)
				{
					this.OnProfileUpdateSuccess();
				}
			}
			else if (this.OnProfileUpdateFailed != null)
			{
				this.OnProfileUpdateFailed(result);
			}
		}

		public virtual void PasswordRecoverySend(string lookupValue)
		{
			if (NetworkConfigIsNotSet)
			{
				throw new NullReferenceException("NetworkServicesConfig not set");
			}
			PasswordRecoverySender passwordRecoverySender = new PasswordRecoverySender(keychainData, logger, storageDir, config.GuestControllerHostUrl, SpoofedGeoIP, config.DisneyIdClientId, coroutineManager);
			passwordRecoverySender.Send(lookupValue, localizer.LanguageStringOneID, delegate(ISendPasswordRecoveryResult result)
			{
				if (result.Success)
				{
					if (this.OnRecoverySuccess != null)
					{
						this.OnRecoverySuccess();
					}
				}
				else if (this.OnUsernameRecoveryFailure != null)
				{
					this.OnPasswordRecoveryFailure(result);
				}
			});
		}

		public virtual void UsernameRecoverySend(string lookupValue)
		{
			if (NetworkConfigIsNotSet)
			{
				throw new NullReferenceException("NetworkServicesConfig not set");
			}
			UsernameRecoverySender usernameRecoverySender = new UsernameRecoverySender(keychainData, logger, storageDir, config.GuestControllerHostUrl, SpoofedGeoIP, config.DisneyIdClientId, coroutineManager);
			usernameRecoverySender.Send(lookupValue, localizer.LanguageStringOneID, delegate(ISendUsernameRecoveryResult result)
			{
				if (result.Success)
				{
					if (this.OnRecoverySuccess != null)
					{
						this.OnRecoverySuccess();
					}
				}
				else if (this.OnUsernameRecoveryFailure != null)
				{
					this.OnUsernameRecoveryFailure(result);
				}
			});
		}

		public virtual void MultipleAccountsResolutionSend(string lookupValue)
		{
			if (NetworkConfigIsNotSet)
			{
				throw new NullReferenceException("NetworkServicesConfig not set");
			}
			MultipleAccountsResolutionSender multipleAccountsResolutionSender = new MultipleAccountsResolutionSender(keychainData, logger, storageDir, config.GuestControllerHostUrl, SpoofedGeoIP, config.DisneyIdClientId, coroutineManager);
			multipleAccountsResolutionSender.Send(lookupValue, localizer.LanguageStringOneID, delegate(ISendMultipleAccountsResolutionResult result)
			{
				if (result.Success)
				{
					if (this.OnMultipleAccountsResolutionSuccess != null)
					{
						this.OnMultipleAccountsResolutionSuccess();
					}
				}
				else if (this.OnMultipleAccountsResolutionFailure != null)
				{
					this.OnMultipleAccountsResolutionFailure(result);
				}
			});
		}

		public virtual void NonRegisteredTransactorUpgradeSend(string lookupValue)
		{
			if (NetworkConfigIsNotSet)
			{
				throw new NullReferenceException("NetworkServicesConfig not set");
			}
			NonRegisteredTransactorUpgradeSender nonRegisteredTransactorUpgradeSender = new NonRegisteredTransactorUpgradeSender(keychainData, logger, storageDir, config.GuestControllerHostUrl, SpoofedGeoIP, config.DisneyIdClientId, coroutineManager);
			nonRegisteredTransactorUpgradeSender.Send(lookupValue, localizer.LanguageStringOneID, delegate(ISendNonRegisteredTransactorUpgradeResult result)
			{
				if (result.Success)
				{
					if (this.OnNRTUpgradeSuccess != null)
					{
						this.OnNRTUpgradeSuccess();
					}
				}
				else if (this.OnNRTUpgradeFailure != null)
				{
					this.OnNRTUpgradeFailure(result);
				}
			});
		}

		public virtual void ParentalApprovalEmailSend(ILocalUser localUser)
		{
			if (NetworkConfigIsNotSet)
			{
				throw new NullReferenceException("NetworkServicesConfig not set");
			}
			localUser.SendParentalApprovalEmail(localizer.LanguageStringOneID, delegate(ISendParentalApprovalEmailResult result)
			{
				if (result.Success)
				{
					if (this.OnParentalApprovalEmailSendSuccess != null)
					{
						this.OnParentalApprovalEmailSendSuccess();
					}
				}
				else if (this.OnParentalApprovalEmailSendFailure != null)
				{
					this.OnParentalApprovalEmailSendFailure();
				}
			});
		}

		public virtual void ValidateUsernamePassword(string username, string password)
		{
			accountValidator.ValidateChildAccount(username, password, delegate(IValidateNewAccountResult result)
			{
				if (result.Success)
				{
					if (this.OnValidationSuccess != null)
					{
						this.OnValidationSuccess();
					}
				}
				else if (this.OnValidationFailed != null)
				{
					this.OnValidationFailed(result);
				}
			});
		}

		private IEnumerator executeRetryOperation(RetryOperation retryOperation)
		{
			IsFetchingRegConfig = true;
			if (retryOperation.retryAttempt >= retryOperation.maxRetries)
			{
				IsFetchingRegConfig = false;
				RegConfigAttempts++;
				if (this.OnRegistrationConfigError != null)
				{
					this.OnRegistrationConfigError("config");
				}
				yield break;
			}
			yield return new WaitForSeconds(retryOperation.retryAttempt * retryOperation.retryDelay);
			retryOperation.retryAttempt++;
			object[] args = new object[retryOperation.Args.Length + 1];
			if (retryOperation.Args.Length > 0)
			{
				Array.Copy(retryOperation.Args, args, retryOperation.Args.Length);
				args[retryOperation.Args.Length] = retryOperation;
			}
			else
			{
				args[0] = retryOperation;
			}
			retryOperation.Method.Invoke(this, args);
		}
	}
}
