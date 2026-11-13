using ClubPenguin.Analytics;
using ClubPenguin.Mix;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	public abstract class CreatePopupContentController : MonoBehaviour
	{
		[Serializable]
		protected struct InputFields
		{
			public InputFieldValidator[] Validators;
		}

		protected enum ButtonState
		{
			disabled,
			enabled,
			loading
		}

		[Header("Input Fields")]
		[SerializeField]
		protected InputFields[] shardInputFields;

		[SerializeField]
		protected InputFieldValidator usernameInputField = null;

		[SerializeField]
		protected InputFieldValidator passwordInputField = null;

		[SerializeField]
		protected InputFieldValidator passwordConfirmInputField = null;

		[SerializeField]
		protected InputFieldValidator parentEmailInputField = null;

		[SerializeField]
		protected InputFieldValidator firstNameInputField = null;

		[SerializeField]
		protected LegalCheckboxesValidator legalCheckBoxes = null;

		[SerializeField]
		[Header("Create Button")]
		private Button createButton = null;

		protected int shard;

		private AbstractCreateController createController;

		private ButtonState currentButtonState;

		private OnOffGameObjectSelector createToggleSelector;

		private void OnValidate()
		{
		}

		private void Awake()
		{
			createToggleSelector = createButton.GetComponent<OnOffGameObjectSelector>();
		}

		protected virtual void Start()
		{
			Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().AccountFunnelName, "03", "create");
			createController = GetComponentInParent<AbstractCreateController>();
			createController.OnCreateError += onCreateError;
			createController.CanShowAccountError = true;
			createController.CheckRegConfigReady();
		}

		protected virtual void OnEnable()
		{
			setButtonInteraction(ButtonState.disabled);
		}

		private void OnDestroy()
		{
			createController.CanShowAccountError = false;
			createController.OnCreateError -= onCreateError;
		}

		private void Update()
		{
			if (currentButtonState == ButtonState.disabled)
			{
				if (inputFieldsValidationComplete() && !inputFieldsHasError() && legalCheckBoxes.AllBoxesAreChecked)
				{
					setButtonInteraction(ButtonState.enabled);
				}
			}
			else if (currentButtonState == ButtonState.enabled && (inputFieldsHasError() || !legalCheckBoxes.AllBoxesAreChecked))
			{
				setButtonInteraction(ButtonState.disabled);
			}
		}

		protected void setButtonInteraction(ButtonState state)
		{
			currentButtonState = state;
			switch (state)
			{
			case ButtonState.disabled:
				setButtonsDisabled();
				break;
			case ButtonState.enabled:
				setButtonsEnabled();
				break;
			case ButtonState.loading:
				setButtonsLoading();
				break;
			}
		}

		protected virtual void setButtonsDisabled()
		{
			createButton.interactable = false;
			createToggleSelector.IsOn = false;
		}

		protected virtual void setButtonsEnabled()
		{
			createButton.interactable = true;
			createToggleSelector.IsOn = false;
		}

		protected virtual void setButtonsLoading()
		{
			createButton.interactable = false;
			createToggleSelector.IsOn = true;
		}

		public void OnCreateClicked()
		{
			if (createController.CheckRegConfigReady())
			{
				Service.Get<ICPSwrveService>().Action("game.account_creation", "submit_clicked");
				CoroutineRunner.StopAllForOwner(this);
				CoroutineRunner.Start(submitActions(), this, "CreateFormSubmitValidation");
			}
			AccountFlowData accountFlowData = Service.Get<MembershipService>().GetAccountFlowData();
			accountFlowData.FlowType = AccountFlowType.create;
		}

		protected IEnumerator submitActions()
		{
			setButtonInteraction(ButtonState.loading);
			startInputValidation();
			if (!isValidationComplete())
			{
				yield return null;
			}
			if (checkForValidationErrors())
			{
				setButtonInteraction(ButtonState.disabled);
				resetValidation();
			}
			else
			{
				createAccount();
			}
		}

		private bool inputFieldsHasError()
		{
			for (int i = 0; i < shardInputFields[shard].Validators.Length; i++)
			{
				if (shardInputFields[shard].Validators[i].HasError)
				{
					return true;
				}
			}
			return false;
		}

		private bool inputFieldsValidationComplete()
		{
			for (int i = 0; i < shardInputFields[shard].Validators.Length; i++)
			{
				if (!shardInputFields[shard].Validators[i].IsValidationComplete)
				{
					return false;
				}
			}
			return true;
		}

		private void createAccount()
		{
			DCreateAccountPayload payload = default(DCreateAccountPayload);
			payload.Username = usernameInputField.TextInput.text;
			payload.Password = passwordInputField.TextInput.text;
			payload.FirstName = (string.IsNullOrEmpty(firstNameInputField.TextInput.text) ? null : firstNameInputField.TextInput.text);
			payload.ParentEmail = parentEmailInputField.TextInput.text;
			payload.AcceptedLegalDocs = Service.Get<MixLoginCreateService>().RegistrationAgeBand.LegalDocuments;
			payload.LangPref = Service.Get<Localizer>().LanguageStringOneID;
			createController.Create(payload);
		}

		private void onCreateError(IRegisterResult result)
		{
			string text = string.Empty;
			setButtonInteraction(ButtonState.disabled);
			string type;
			string format;
			if (result.Errors != null)
			{
				foreach (IInvalidProfileItemError error in result.Errors)
				{
					object obj = text;
					text = string.Concat(obj, "\t[", error, "]\n");
					if (error is IInvalidUsernameError)
					{
						string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation("Account.Create.Validation.InvalidUsername");
						showUsernameError(tokenTranslation);
					}
					else if (error is IInvalidPasswordError || error is IPasswordMatchesOtherProfileInfoError || error is IPasswordUsesProfileInformationError)
					{
						string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation("Account.Create.Validation.InvalidPassword");
						showPasswordError(tokenTranslation);
					}
					else if (error is IInvalidParentEmailError)
					{
						string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation("Account.Create.Validation.InvalidEmail");
						showParentEmailError(tokenTranslation);
					}
					else if (error is IParentEmailBannedError)
					{
						string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation("Account.Create.Validation.ParentEmailBanned");
						showParentEmailError(tokenTranslation);
					}
					else if (error is IInvalidFirstNameError)
					{
						string tokenTranslation2 = Service.Get<Localizer>().GetTokenTranslation("Account.Create.Validation.EnterValid");
						string tokenTranslation = string.Format(tokenTranslation2, Service.Get<Localizer>().GetTokenTranslation("Account.Create.Validation.FirstNameField"));
						showFirstNameError(tokenTranslation);
					}
					else if (error is IPasswordTooCommonError)
					{
						string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation("Acount.Create.Validate.TooCommon");
						showPasswordError(tokenTranslation);
					}
					else if (error is IPasswordSizeError)
					{
						string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation("Account.Create.Validation.PasswordLength");
						showPasswordError(tokenTranslation);
					}
					else if (error is IPasswordLikePhoneNumberError)
					{
						string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation("Account.Create.Validation.PasswordPhoneNumber");
						showPasswordError(tokenTranslation);
					}
					else if (error is IPasswordMissingExpectedCharactersError)
					{
						string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation("Account.Create.Validation.PasswordComplexity");
						showPasswordError(tokenTranslation);
					}
					else if (error is IRegisterRateLimitedResult)
					{
						type = "";
						format = "Account.General.Error.RateLimited";
						Service.Get<EventDispatcher>().DispatchEvent(new ApplicationService.Error(type, format));
					}
					else if (error is IRegisterEmbargoedCountryResult || error is IRegisterGatedLocationResult)
					{
						type = "GlobalUI.Homescreen.GeoGate.Title";
						format = "GlobalUI.Homescreen.GeoGate.Body";
						Service.Get<PromptManager>().ShowError(type, format);
						showUsernameError(Service.Get<Localizer>().GetTokenTranslation(type));
					}
					else
					{
						string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation("Account.Create.Validation.UnknownError");
						showUsernameError(tokenTranslation);
					}
				}
				return;
			}
			type = "Account.Create.Error.OneID.Title";
			format = "Account.Create.Error.OneID";
			Service.Get<EventDispatcher>().DispatchEvent(new ApplicationService.Error(type, format));
		}

		protected virtual void showUsernameError(string errorMessage)
		{
			usernameInputField.HasError = true;
			usernameInputField.ShowError(errorMessage);
		}

		protected virtual void showPasswordError(string errorMessage)
		{
			passwordInputField.HasError = true;
			passwordInputField.ShowError(errorMessage);
		}

		protected virtual void showParentEmailError(string errorMessage)
		{
			parentEmailInputField.HasError = true;
			parentEmailInputField.ShowError(errorMessage);
		}

		protected virtual void showFirstNameError(string errorMessage)
		{
			firstNameInputField.HasError = true;
			firstNameInputField.ShowError(errorMessage);
		}

		protected abstract void startInputValidation();

		protected abstract bool isValidationComplete();

		protected abstract bool checkForValidationErrors();

		protected abstract void resetValidation();
	}
}
