using ClubPenguin.UI;
using Disney.Kelowna.Common;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class CreatePopupScrollContentController : CreatePopupContentController
	{
		[Header("Sign In")]
		[SerializeField]
		public Button signInButton = null;

		[Header("Scrolling")]
		[SerializeField]
		private FormFieldVerticalScrollFocusPositioner scrollPositioner = null;

		protected override void OnEnable()
		{
			legalCheckBoxes.OnLegalDocumentsShown += onLegalDocumentsShown;
			base.OnEnable();
		}

		private void OnDisable()
		{
			legalCheckBoxes.OnLegalDocumentsShown -= onLegalDocumentsShown;
		}

		protected override void setButtonsDisabled()
		{
			signInButton.interactable = true;
			base.setButtonsDisabled();
		}

		protected override void setButtonsEnabled()
		{
			signInButton.interactable = true;
			base.setButtonsEnabled();
		}

		protected override void setButtonsLoading()
		{
			signInButton.interactable = false;
			base.setButtonsLoading();
		}

		protected override void startInputValidation()
		{
			if (!usernameInputField.IsValidationInProgress && !usernameInputField.IsValidationComplete)
			{
				usernameInputField.StartValidation();
			}
			if (!passwordInputField.IsValidationInProgress && !passwordInputField.IsValidationComplete)
			{
				passwordInputField.StartValidation();
			}
			if (!passwordConfirmInputField.IsValidationInProgress && !passwordConfirmInputField.IsValidationComplete)
			{
				passwordConfirmInputField.StartValidation();
			}
			if (!parentEmailInputField.IsValidationInProgress && !parentEmailInputField.IsValidationComplete)
			{
				parentEmailInputField.StartValidation();
			}
			if (!firstNameInputField.IsValidationInProgress && !firstNameInputField.IsValidationComplete)
			{
				firstNameInputField.StartValidation();
			}
			if (!legalCheckBoxes.IsValidationInProgress && !legalCheckBoxes.IsValidationComplete)
			{
				legalCheckBoxes.ValidateLegalCheckBoxes();
			}
		}

		protected override bool isValidationComplete()
		{
			return usernameInputField.IsValidationComplete && passwordInputField.IsValidationComplete && passwordConfirmInputField.IsValidationComplete && parentEmailInputField.IsValidationComplete && firstNameInputField.IsValidationComplete && legalCheckBoxes.IsValidationComplete;
		}

		protected override bool checkForValidationErrors()
		{
			bool result = false;
			IFormValidationItem[] fieldList = new IFormValidationItem[6]
			{
				usernameInputField,
				passwordInputField,
				passwordConfirmInputField,
				parentEmailInputField,
				firstNameInputField,
				legalCheckBoxes
			};
			GameObject firstError;
			if (fieldHasError(fieldList, out firstError))
			{
				result = true;
				scrollPositioner.ScrollTo(firstError);
				setButtonInteraction(ButtonState.disabled);
			}
			return result;
		}

		protected override void resetValidation()
		{
			usernameInputField.IsValidationComplete = false;
			passwordInputField.IsValidationComplete = false;
			passwordConfirmInputField.IsValidationComplete = false;
			parentEmailInputField.IsValidationComplete = false;
			firstNameInputField.IsValidationComplete = false;
			legalCheckBoxes.IsValidationComplete = false;
		}

		private void onLegalDocumentsShown()
		{
			if (scrollPositioner != null)
			{
				scrollPositioner.Recalculate();
			}
		}

		private bool fieldHasError(IFormValidationItem[] fieldList, out GameObject firstError)
		{
			bool result = false;
			int num = (fieldList[0] as MonoBehaviour).transform.parent.childCount - 1;
			firstError = null;
			for (int i = 0; i < fieldList.Length; i++)
			{
				int siblingIndex = (fieldList[i] as MonoBehaviour).transform.GetSiblingIndex();
				bool flag = false;
				GameObject gameObject = null;
				if (fieldList[i].GetType() == typeof(InputFieldValidator))
				{
					flag = (fieldList[i] as InputFieldValidator).HasError;
					gameObject = (fieldList[i] as InputFieldValidator).gameObject;
				}
				else if (fieldList[i].GetType() == typeof(LegalCheckboxesValidator))
				{
					flag = (fieldList[i] as LegalCheckboxesValidator).HasError;
					gameObject = (fieldList[i] as LegalCheckboxesValidator).gameObject;
				}
				if (flag)
				{
					if (siblingIndex < num || i == 0)
					{
						num = siblingIndex;
						firstError = gameObject;
					}
					result = true;
				}
			}
			return result;
		}
	}
}
