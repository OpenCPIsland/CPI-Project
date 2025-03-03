using ClubPenguin.Mix;
using ClubPenguin.Net;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using Disney.Native;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	public abstract class LoginPopupContentController : MonoBehaviour
	{
		[Header("General Error")]
		public GameObject GeneralErrorBox;

		public Text GeneralErrorText;

		private InputFieldValidatorGroup inputFieldValidatorGroup;

		protected InteractableGroup interactableGroup;

		private OnOffGameObjectSelector logInToggleSelector;

		private AbstractLoginController loginController;

		private DLoginPayload loginPayload;

		private bool isPinging = false;

		private bool isLoggingIn;

		public virtual void Awake()
		{
			inputFieldValidatorGroup = GetComponent<InputFieldValidatorGroup>();
			interactableGroup = GetComponent<InteractableGroup>();
			logInToggleSelector = GetComponent<OnOffGameObjectSelector>();
		}

		public virtual void Start()
		{
			loginController = GetComponentInParent<AbstractLoginController>();
		}

		public virtual void OnEnable()
		{
			toggleInteraction(true);
		}

		protected virtual void toggleInteraction(bool isInteractable)
		{
			isLoggingIn = !isInteractable;
			if (interactableGroup != null)
			{
				interactableGroup.IsInteractable = isInteractable;
			}
			if (logInToggleSelector != null)
			{
				logInToggleSelector.IsOn = !isInteractable;
			}
		}

		public void OnDestroy()
		{
			loginController.OnLoginError -= onLoginError;
		}

		protected void performLogin(DLoginPayload loginPayload)
		{
			if (!isLoggingIn)
			{
				loginController.OnLoginError += onLoginError;
				this.loginPayload = loginPayload;
				toggleInteraction(false);
				showGeneralError(string.Empty);
				CoroutineRunner.StopAllForOwner(this);
				CoroutineRunner.Start(submitActions(loginPayload), this, "LoginFormSubmitValidation");
				AccountFlowData accountFlowData = Service.Get<MembershipService>().GetAccountFlowData();
				accountFlowData.FlowType = AccountFlowType.login;
			}
		}

		protected virtual void showGeneralError(string error)
		{
			GeneralErrorBox.gameObject.SetActive(!string.IsNullOrEmpty(error));
			GeneralErrorText.text = error;
		}

		private IEnumerator submitActions(DLoginPayload loginPayload)
		{
			if (inputFieldValidatorGroup != null)
			{
				inputFieldValidatorGroup.StartValidation();
				while (!inputFieldValidatorGroup.IsValidationComplete())
				{
					yield return null;
				}
				if (inputFieldValidatorGroup.CheckForValidationErrors())
				{
					toggleInteraction(true);
					inputFieldValidatorGroup.ResetValidationComplete();
					yield break;
				}
			}
			loginController.Login(loginPayload);
		}

		private void onLoginError(ILoginResult result)
		{
			toggleInteraction(true);
			string errorMessage = "";
			if (result is ILoginCorruptionDetectedResult)
			{
				errorMessage = Service.Get<Localizer>().GetTokenTranslation("Oops! Something went wrong. Please try again.");
				showGeneralError(errorMessage);
				MonoSingleton<NativeAccessibilityManager>.Instance.Native.HandleError(GeneralErrorText.GetInstanceID());
			}
			else if (result is ILoginFailedAccountLockedOutResult)
			{
				errorMessage = Service.Get<Localizer>().GetTokenTranslation("Acount.Login.Errors.LockedOut");
				showGeneralError(errorMessage);
				MonoSingleton<NativeAccessibilityManager>.Instance.Native.HandleError(GeneralErrorText.GetInstanceID());
			}
			else if (result is ILoginFailedAuthenticationResult)
			{
				showUsernameError(string.Empty);
				if (!isPinging)
				{
					toggleInteraction(false);
					isPinging = true;
					Service.Get<ConnectionManager>().DoPingCheck(delegate(ConnectionManager.NetworkConnectionState connectionState)
					{
						isPinging = false;
						toggleInteraction(true);
						if (connectionState == ConnectionManager.NetworkConnectionState.NoConnection)
						{
							Service.Get<PromptManager>().ShowError("GlobalUI.ErrorMessages.NetworkConnectionError", "GlobalUI.ErrorMessages.CheckNetworkConnection");
						}
						else
						{
							errorMessage = Service.Get<Localizer>().GetTokenTranslation("Acount.Login.Errors.InvalidCredentials");
							showGeneralError(errorMessage);
							MonoSingleton<NativeAccessibilityManager>.Instance.Native.HandleError(GeneralErrorText.GetInstanceID());
							onInvalidCredentialsError(errorMessage);
						}
					});
				}
			}
			else if (result is ILoginFailedMultipleAccountsResult)
			{
				Service.Get<MixLoginCreateService>().OnMultipleAccountsResolutionSuccess += onSendSuccess;
				Service.Get<MixLoginCreateService>().MultipleAccountsResolutionSend(loginPayload.Username);
				Service.Get<PromptManager>().ShowError("Account.Login.Error.MultipleAccountsSameEmail", "Account.Login.Error.ResolveInstructionsEmailSent");
			}
			else if (result is ILoginFailedGatedLocationResult)
			{
				Service.Get<PromptManager>().ShowError("GlobalUI.Homescreen.GeoGate.Title", "GlobalUI.Homescreen.GeoGate.Body");
			}
			else if (result is ILoginFailedParentalConsentResult)
			{
				DPrompt data = new DPrompt("Acount.Login.Errors.ActivationNeeded", "Account.Login.Error.ActivationEmailResend", DPrompt.ButtonFlags.NO | DPrompt.ButtonFlags.YES);
				Service.Get<PromptManager>().ShowPrompt(data, resendActivationEmail);
			}
			else if (result is ILoginFailedProfileDisabledResult)
			{
				errorMessage = Service.Get<Localizer>().GetTokenTranslation("Acount.Login.Errors.BanPermanent");
				showGeneralError(errorMessage);
				MonoSingleton<NativeAccessibilityManager>.Instance.Native.HandleError(GeneralErrorText.GetInstanceID());
			}
			else if (result is ILoginFailedTemporaryBanResult)
			{
				onTemporaryBanError();
			}
			else if (result is ILoginMissingInfoResult)
			{
				if (string.IsNullOrEmpty(loginPayload.Username))
				{
					errorMessage = string.Format(Service.Get<Localizer>().GetTokenTranslation("Account.Create.Validation.Empty"), Service.Get<Localizer>().GetTokenTranslation("Account.Create.Validation.UsernameField"));
					showUsernameError(errorMessage);
				}
				if (string.IsNullOrEmpty(loginPayload.Password))
				{
					errorMessage = string.Format(Service.Get<Localizer>().GetTokenTranslation("Account.Create.Validation.Empty"), Service.Get<Localizer>().GetTokenTranslation("Account.Create.Validation.PasswordField"));
					showPasswordError(errorMessage);
				}
				onMissingInfoError(errorMessage);
			}
			else if (result is ILoginFailedRateLimitedResult || result is ILoginSecurityUpdateResult)
			{
				string type = "";
				string format = "Account.General.Error.RateLimited";
				Service.Get<EventDispatcher>().DispatchEvent(new ApplicationService.Error(type, format));
			}
			else
			{
				errorMessage = Service.Get<Localizer>().GetTokenTranslation("Account.Create.Validation.UnknownError");
				showGeneralError(errorMessage);
				MonoSingleton<NativeAccessibilityManager>.Instance.Native.HandleError(GeneralErrorText.GetInstanceID());
				if (Service.Get<MixLoginCreateService>().RegistrationConfig == null || Service.Get<MixLoginCreateService>().RegistrationAgeBand == null || Service.Get<MixLoginCreateService>().UpdateAgeBand == null)
				{
					string step = (Service.Get<MixLoginCreateService>().RegistrationConfig == null) ? "config" : "ageband";
					string type = "Account.Create.Error.OneID.Title";
					string format = "Account.Create.Error.OneID";
					Service.Get<EventDispatcher>().DispatchEvent(new SessionErrorEvents.RegistrationConfigError(type, format, step));
				}
			}
			loginController.OnLoginError -= onLoginError;
		}

		protected virtual void onInvalidCredentialsError(string error)
		{
		}

		protected virtual void onMissingInfoError(string error)
		{
		}

		protected virtual void onTemporaryBanError()
		{
			showAccountBannedPrompt();
		}

		protected void showAccountBannedPrompt(AlertType category = AlertType.Unknown, DateTime? expirationDate = null)
		{
			PromptDefinition promptDefinition = Service.Get<PromptManager>().GetPromptDefinition("ModerationCriticalPrompt");
			PromptLoaderCMD promptLoaderCMD = new PromptLoaderCMD(this, promptDefinition, delegate(PromptLoaderCMD loader)
			{
				onAccountBannedPromptLoaded(loader, category, expirationDate);
			});
			promptLoaderCMD.Execute();
		}

		protected virtual void onAccountBannedPromptLoaded(PromptLoaderCMD promptLoader, AlertType category, DateTime? expirationDate)
		{
			SessionErrorEvents.AccountBannedPromptSetup(promptLoader.PromptData, category, expirationDate);
			Service.Get<PromptManager>().ShowPrompt(promptLoader.PromptData, null, promptLoader.Prefab);
		}

		protected virtual void showUsernameError(string error)
		{
		}

		protected virtual void showPasswordError(string error)
		{
		}

		private void onSendSuccess()
		{
			Service.Get<MixLoginCreateService>().OnParentalApprovalEmailSendSuccess -= onSendSuccess;
			string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation("Account.Login.Error.CheckEmail");
			showGeneralError(tokenTranslation);
			MonoSingleton<NativeAccessibilityManager>.Instance.Native.HandleError(GeneralErrorText.GetInstanceID());
		}

		private void resendActivationEmail(DPrompt.ButtonFlags button)
		{
			if (button == DPrompt.ButtonFlags.YES)
			{
				Service.Get<MixLoginCreateService>().OnParentalApprovalEmailSendSuccess += onSendSuccess;
				Service.Get<MixLoginCreateService>().ParentalApprovalEmailSend(Service.Get<SessionManager>().LocalUser);
			}
		}
	}
}
