using ClubPenguin.Analytics;
using ClubPenguin.Mix;
using ClubPenguin.Net;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class UpdateProfilePopupContentController : MonoBehaviour
	{
		public enum FormType
		{
			MissingInfo,
			LegalUpdate
		}

		[Header("Input Fields")]
		public InputFieldValidator ParentEmailInputField;

		private bool isParentEmailFieldDisplayed;

		public InputFieldValidator FirstNameInputField;

		private bool isFirstNameFieldDisplayed;

		public LegalCheckboxesValidator LegalPanel;

		private bool isLegalTextDisplayed;

		[Header("Buttons")]
		public Button UpdateButton;

		public Text UpdateButtonText;

		public GameObject PreloaderImage;

		[Header("Text")]
		public Text TitleText;

		public Text InstructionText;

		[Header("FormType")]
		public FormType WhichFields;

		private AbstractCreateController createController;

		private SessionManager sessionManager;

		private AccountPopupContentAnchors popupAnchors;

		private ICoroutine configCoroutine;

		private void OnEnable()
		{
			ToggleInteraction(true);
			if (popupAnchors != null)
			{
				popupAnchors.VerticalAlign = AccountPopupContentAnchors.VerticalAlignment.Bottom;
			}
		}

		private void Start()
		{
			popupAnchors = GetComponentInParent<AccountPopupContentAnchors>();
			popupAnchors.VerticalAlign = AccountPopupContentAnchors.VerticalAlignment.Bottom;
			createController = GetComponentInParent<AbstractCreateController>();
			createController.OnProfileUpdateError += onProfileUpdateError;
			sessionManager = Service.Get<SessionManager>();
			Localizer localizer = Service.Get<Localizer>();
			string text = "";
			string text2 = "";
			string text3 = "";
			if (WhichFields == FormType.LegalUpdate)
			{
				createController.CanShowAccountError = true;
				text2 = "legal_update";
				text3 = "14";
				SetLegalUpdate();
				TitleText.text = string.Format(localizer.GetTokenTranslation("Account.Login.PPU.LegalUpdate"), sessionManager.LocalUser.DisplayName.Text);
				LegalPanel.gameObject.SetActive(isLegalTextDisplayed);
			}
			else
			{
				createController.CanShowAccountError = false;
				text2 = "profile_update";
				text3 = "13";
				SetMissingInfo();
				TitleText.text = string.Format(localizer.GetTokenTranslation("Account.Login.PPU.MissingInfo.TitleText"), sessionManager.LocalUser.DisplayName.Text);
				text = localizer.GetTokenTranslation("Account.Login.PPU.MissingInfo.Instructions");
				ParentEmailInputField.gameObject.SetActive(isParentEmailFieldDisplayed);
				FirstNameInputField.gameObject.SetActive(isFirstNameFieldDisplayed);
				if (isParentEmailFieldDisplayed)
				{
					text = text + "\n" + localizer.GetTokenTranslation("Account.Login.PPU.MissingInfo.ParentEmailField");
				}
				if (isFirstNameFieldDisplayed)
				{
					text = text + "\n" + localizer.GetTokenTranslation("Account.Login.PPU.MissingInfo.FirstNameField");
				}
				InstructionText.text = text;
			}
			Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().AccountFunnelName, text3, text2);
		}

		private void OnDestroy()
		{
			createController.CanShowAccountError = false;
			popupAnchors.Reset();
			if (configCoroutine != null && !configCoroutine.Disposed)
			{
				configCoroutine.Stop();
				configCoroutine = null;
			}
			createController.OnProfileUpdateError -= onProfileUpdateError;
		}

		public void SetMissingInfo()
		{
			isFirstNameFieldDisplayed = string.IsNullOrEmpty(sessionManager.LocalUser.RegistrationProfile.FirstName);
			isParentEmailFieldDisplayed = (string.IsNullOrEmpty(sessionManager.LocalUser.RegistrationProfile.ParentEmail) && string.IsNullOrEmpty(sessionManager.LocalUser.RegistrationProfile.Email));
			isLegalTextDisplayed = false;
		}

		public void SetLegalUpdate()
		{
			isFirstNameFieldDisplayed = false;
			isParentEmailFieldDisplayed = false;
			isLegalTextDisplayed = true;
		}

		public void ToggleInteraction(bool isInteractable)
		{
			UpdateButton.interactable = isInteractable;
			UpdateButtonText.gameObject.SetActive(isInteractable);
			PreloaderImage.SetActive(!isInteractable);
		}

		public void OnUpdateClicked()
		{
			ToggleInteraction(false);
			CoroutineRunner.StopAllForOwner(this);
			CoroutineRunner.Start(submitActions(), this, "UpdateProfileFormSubmitValidation");
		}

		private IEnumerator submitActions()
		{
			if (isParentEmailFieldDisplayed && !ParentEmailInputField.IsValidationInProgress && !ParentEmailInputField.IsValidationComplete)
			{
				ParentEmailInputField.StartValidation();
			}
			if (isFirstNameFieldDisplayed && !FirstNameInputField.IsValidationInProgress && !FirstNameInputField.IsValidationComplete)
			{
				FirstNameInputField.StartValidation();
			}
			if (isLegalTextDisplayed && !LegalPanel.IsValidationInProgress && !LegalPanel.IsValidationComplete)
			{
				LegalPanel.ValidateLegalCheckBoxes();
			}
			while ((isParentEmailFieldDisplayed && !ParentEmailInputField.IsValidationComplete) || (isFirstNameFieldDisplayed && !FirstNameInputField.IsValidationComplete) || (isLegalTextDisplayed && !LegalPanel.IsValidationComplete))
			{
				yield return null;
			}
			if ((isParentEmailFieldDisplayed && ParentEmailInputField.HasError) || (isFirstNameFieldDisplayed && FirstNameInputField.HasError) || (isLegalTextDisplayed && LegalPanel.HasError))
			{
				ToggleInteraction(true);
			}
			else
			{
				updateAccount();
			}
		}

		private void updateAccount()
		{
			DUpdateProfilePayload payload = default(DUpdateProfilePayload);
			if (isFirstNameFieldDisplayed)
			{
				payload.FirstName = FirstNameInputField.TextInput.text;
			}
			else
			{
				payload.FirstName = null;
			}
			if (isParentEmailFieldDisplayed)
			{
				payload.ParentEmail = ParentEmailInputField.TextInput.text;
			}
			else
			{
				payload.ParentEmail = null;
			}
			if (isLegalTextDisplayed)
			{
				payload.AcceptedLegalDocs = Service.Get<MixLoginCreateService>().UpdateAgeBand.LegalDocuments;
			}
			else
			{
				payload.AcceptedLegalDocs = null;
			}
			createController.UpdateProfile(payload);
		}

		public void onProfileUpdateError(IUpdateProfileResult result)
		{
			ToggleInteraction(true);
			string text = string.Empty;
			foreach (IInvalidProfileItemError error in result.Errors)
			{
				object obj = text;
				text = string.Concat(obj, "\t[", error, "]\n");
				string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation("Account.Create.Validation.EnterValid");
				if (WhichFields == FormType.MissingInfo)
				{
					if (error is IInvalidParentEmailError)
					{
						ParentEmailInputField.HasError = true;
						string tokenTranslation2 = Service.Get<Localizer>().GetTokenTranslation("Account.Create.Validation.InvalidEmail");
						ParentEmailInputField.ShowError(tokenTranslation2);
					}
					else if (error is IInvalidFirstNameError)
					{
						FirstNameInputField.HasError = true;
						string tokenTranslation2 = string.Format(tokenTranslation, Service.Get<Localizer>().GetTokenTranslation("Account.Create.Validation.FirstNameField"));
						FirstNameInputField.ShowError(tokenTranslation2);
					}
					else if (error is IInvalidPrivacyPolicyError)
					{
						LegalPanel.HasError = true;
						string tokenTranslation2 = Service.Get<Localizer>().GetTokenTranslationFormatted("Account.Create.LegalDoc.errorString", "Account.Create.LegalDoc.ppV2");
						LegalPanel.ShowError(tokenTranslation2);
					}
					else
					{
						InputFieldValidator inputFieldValidator = FirstNameInputField;
						if (!isFirstNameFieldDisplayed)
						{
							inputFieldValidator = ParentEmailInputField;
						}
						inputFieldValidator.HasError = true;
						string tokenTranslation2 = string.Empty;
						tokenTranslation2 += Service.Get<Localizer>().GetTokenTranslation("Account.Create.Validation.UnknownError");
						inputFieldValidator.ShowError(tokenTranslation2);
					}
				}
				else if (error is IInvalidPrivacyPolicyError)
				{
					LegalPanel.HasError = true;
					string tokenTranslation2 = Service.Get<Localizer>().GetTokenTranslationFormatted("Account.Create.LegalDoc.errorString", "Account.Create.LegalDoc.ppV2");
					LegalPanel.ShowError(tokenTranslation2);
				}
				else if (error is IInvalidTermsOfUseError)
				{
					LegalPanel.HasError = true;
					string tokenTranslation2 = Service.Get<Localizer>().GetTokenTranslationFormatted("Account.Create.LegalDoc.errorString", "Account.Create.LegalDoc.GTOU");
					LegalPanel.ShowError(tokenTranslation2);
				}
				else
				{
					LegalPanel.HasError = true;
					string tokenTranslation2 = string.Empty;
					tokenTranslation2 += Service.Get<Localizer>().GetTokenTranslation("Account.Create.Validation.UnknownError");
					LegalPanel.ShowError(tokenTranslation2);
				}
			}
		}
	}
}
