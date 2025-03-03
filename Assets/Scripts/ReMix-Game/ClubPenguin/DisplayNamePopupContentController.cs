using ClubPenguin.Analytics;
using ClubPenguin.Avatar;
using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class DisplayNamePopupContentController : MonoBehaviour
	{
		[Header("Input Fields")]
		public InputFieldValidator DisplayNameInputField;

		public InputFieldValidator ReferrerDisplayNameField;

		[Header("Buttons")]
		public Button SubmitButton;

		public Text SubmitButtonText;

		[Header("Status")]
		public Text Status;

		public GameObject PreloaderImage;

		[Header("Instructions")]
		public Text Instructions;

		[Header("Refer a Friend")]
		public GameObject ReferAFriendSection;

		[Header("Force Space for Keyboard")]
		public GameObject AvatarPanel;

		public GameObject SpacerSpace;

		private AbstractCreateController createController;

		private bool setDisplayNameValid;

		private bool referAFriendEnabled;

		public Text StatusText
		{
			get
			{
				return Status;
			}
		}

		private void OnEnable()
		{
			ToggleInteraction(true);
		}

		private void Start()
		{
			Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().AccountFunnelName, "05", "display_name");
			createController = GetComponentInParent<AbstractCreateController>();
			createController.OnUpdateDisplayNameError += OnUpdateDisplayNameError;
			referAFriendEnabled = true;
			if (Service.Get<SessionManager>().LocalUser != null && (Service.Get<SessionManager>().LocalUser.RegistrationProfile.DisplayNameProposedStatus == DisplayNameProposedStatus.Rejected || (Service.Get<SessionManager>().LocalUser.RegistrationProfile.DisplayNameProposedStatus == DisplayNameProposedStatus.Pending && Service.Get<SessionManager>().LocalUser.RegistrationProfile.ProposedDisplayName.StartsWith("DNAME-REJ-"))))
			{
				Instructions.text = Service.Get<Localizer>().GetTokenTranslation("Account.DisplayName.DisplayNameRejected");
				referAFriendEnabled = false;
			}
			if (ReferAFriendSection != null)
			{
				ReferAFriendSection.SetActive(referAFriendEnabled);
			}
			else
			{
				referAFriendEnabled = false;
			}
			setDisplayNameValid = false;
			AccountFlowData accountFlowData = Service.Get<MembershipService>().GetAccountFlowData();
			SessionManager sessionManager = Service.Get<SessionManager>();
			if (!string.IsNullOrEmpty(sessionManager.LocalUser.RegistrationProfile.Username) && accountFlowData.PreValidatedDisplayNames.Contains(sessionManager.LocalUser.RegistrationProfile.Username))
			{
				DisplayNameInputField.TextInput.text = sessionManager.LocalUser.RegistrationProfile.Username;
				DisplayNameInputField.HasError = false;
				DisplayNameInputField.IsValidationComplete = true;
				setDisplayNameValid = true;
			}
			AvatarRenderTextureComponent componentInChildren = GetComponentInChildren<AvatarRenderTextureComponent>();
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			DataEntityHandle localPlayerHandle = cPDataEntityCollection.LocalPlayerHandle;
			AvatarDetailsData component;
			if (!localPlayerHandle.IsNull && cPDataEntityCollection.TryGetComponent(localPlayerHandle, out component))
			{
				componentInChildren.RenderAvatar(component);
			}
			else
			{
				componentInChildren.RenderAvatar(new DCustomEquipment[0]);
			}
		}

		private void Update()
		{
			if (DisplayNameInputField.gameObject.activeInHierarchy && setDisplayNameValid)
			{
				DisplayNameInputField.OnValidationSuccess.Invoke();
				setDisplayNameValid = false;
			}
		}

		private void OnDestroy()
		{
			createController.OnUpdateDisplayNameError -= OnUpdateDisplayNameError;
		}

		public void ToggleInteraction(bool isInteractable)
		{
			SubmitButton.interactable = isInteractable;
			SubmitButtonText.gameObject.SetActive(isInteractable);
			PreloaderImage.SetActive(!isInteractable);
		}

		public void OnSubmitClicked()
		{
			if (SubmitButton.interactable)
			{
				ToggleInteraction(false);
				CoroutineRunner.StopAllForOwner(this);
				CoroutineRunner.Start(submitActions(), this, "DisplayNameFormSubmitValidation");
				Service.Get<ICPSwrveService>().Action("display_name_change");
			}
		}

		private IEnumerator submitActions()
		{
			if (!DisplayNameInputField.IsValidationInProgress && !DisplayNameInputField.IsValidationComplete)
			{
				DisplayNameInputField.StartValidation();
			}
			if (referAFriendEnabled && (!string.IsNullOrEmpty(ReferrerDisplayNameField.TextInput.text) || ReferrerDisplayNameField.HasError) && !ReferrerDisplayNameField.IsValidationInProgress && !ReferrerDisplayNameField.IsValidationComplete)
			{
				ReferrerDisplayNameField.StartValidation();
			}
			while (!DisplayNameInputField.IsValidationComplete && (!referAFriendEnabled || !ReferrerDisplayNameField.IsValidationComplete))
			{
				yield return null;
			}
			if (DisplayNameInputField.HasError || ReferrerDisplayNameField.HasError)
			{
				ToggleInteraction(true);
				DisplayNameInputField.IsValidationComplete = false;
				ReferrerDisplayNameField.IsValidationComplete = false;
			}
			else
			{
				setDisplayName();
				DisplayNameInputField.IsValidationComplete = false;
			}
		}

		private void setDisplayName()
		{
			string text = DisplayNameInputField.TextInput.text;
			string text2 = null;
			if (referAFriendEnabled)
			{
				text2 = ReferrerDisplayNameField.TextInput.text;
			}
			AccountFlowData accountFlowData = Service.Get<MembershipService>().GetAccountFlowData();
			accountFlowData.Referrer = text2;
			createController.UpdateDisplayName(text, text2);
		}

		public void OnUpdateDisplayNameError(IUpdateDisplayNameResult result)
		{
			ToggleInteraction(true);
			string errorMessage;
			if (result is IUpdateDisplayNameExistsResult)
			{
				errorMessage = Service.Get<Localizer>().GetTokenTranslation("Acount.Displayname.Validation.AlreadyUsed");
			}
			else if (result is IUpdateDisplayNameFailedModerationResult)
			{
				errorMessage = Service.Get<Localizer>().GetTokenTranslation("Acount.Displayname.Validation.NotAllowed");
			}
			else
			{
				errorMessage = "";
				errorMessage += Service.Get<Localizer>().GetTokenTranslation("Account.Create.Validation.UnknownError");
			}
			DisplayNameInputField.ShowError(errorMessage);
			Service.Get<ICPSwrveService>().Action("game.display_name_form.submit", "failed", result.ToString());
		}
	}
}
