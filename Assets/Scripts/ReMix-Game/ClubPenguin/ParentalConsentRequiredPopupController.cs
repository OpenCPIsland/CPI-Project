using ClubPenguin.Analytics;
using ClubPenguin.Mix;
using ClubPenguin.Net;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using Disney.Native;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class ParentalConsentRequiredPopupController : MonoBehaviour
	{
		[Header("Containers")]
		public GameObject NoticeContainer;

		public GameObject ResendContainer;

		public GameObject SentContainer;

		public Text SentText;

		[Header("Buttons")]
		public Button ResendButton;

		public Text ResendButtonText;

		public GameObject ResendButtonPreloader;

		[Header("Parent Email")]
		public InputFieldValidator ParentEmailInputField;

		public Text[] ParentEmailTextObjects;

		private AccountFlowData accountFlowData;

		private SessionManager sessionManager;

		private MixLoginCreateService mixLoginCreateService;

		private void Start()
		{
			Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().AccountFunnelName, "15", "parental_consent_required");
		}

		private void OnEnable()
		{
			accountFlowData = Service.Get<MembershipService>().GetAccountFlowData();
			if (accountFlowData.FlowType == AccountFlowType.create)
			{
				NoticeContainer.SetActive(true);
				ResendContainer.SetActive(false);
				SentContainer.SetActive(false);
			}
			else
			{
				NoticeContainer.SetActive(false);
				ResendContainer.SetActive(true);
				ResendButton.interactable = true;
				SentContainer.SetActive(false);
			}
			sessionManager = Service.Get<SessionManager>();
			mixLoginCreateService = Service.Get<MixLoginCreateService>();
			Text[] parentEmailTextObjects = ParentEmailTextObjects;
			foreach (Text text in parentEmailTextObjects)
			{
				text.text = sessionManager.LocalUser.RegistrationProfile.ParentEmail;
			}
		}

		private void OnDisable()
		{
			ResendButton.interactable = false;
		}

		public void OnResendButtonClicked()
		{
			ResendButtonText.gameObject.SetActive(false);
			ResendButtonPreloader.SetActive(true);
			ResendButton.interactable = false;
			if (string.IsNullOrEmpty(ParentEmailInputField.TextInput.text) || ParentEmailInputField.TextInput.text == sessionManager.LocalUser.RegistrationProfile.ParentEmail)
			{
				mixLoginCreateService.OnParentalApprovalEmailSendSuccess += onSendSuccess;
				mixLoginCreateService.ParentalApprovalEmailSend(Service.Get<SessionManager>().LocalUser);
			}
			else
			{
				mixLoginCreateService.OnProfileUpdateSuccess += onParentEmailUpdateSuccess;
				mixLoginCreateService.OnProfileUpdateFailed += onParentEmailUpdateError;
				mixLoginCreateService.UpdateProfile(null, ParentEmailInputField.TextInput.text, null, Service.Get<SessionManager>().LocalUser);
			}
		}

		private void onSendSuccess()
		{
			mixLoginCreateService.OnParentalApprovalEmailSendSuccess -= onSendSuccess;
			MonoSingleton<NativeAccessibilityManager>.Instance.Native.HandleError(SentText.GetInstanceID());
			Service.Get<ICPSwrveService>().Action("parental_consent_required", "resent_email");
			ResendContainer.gameObject.SetActive(false);
			ResendButtonText.gameObject.SetActive(true);
			ResendButtonPreloader.SetActive(false);
			SentContainer.gameObject.SetActive(true);
		}

		private void onParentEmailUpdateSuccess()
		{
			mixLoginCreateService.OnProfileUpdateSuccess -= onParentEmailUpdateSuccess;
			mixLoginCreateService.OnProfileUpdateFailed -= onParentEmailUpdateError;
			mixLoginCreateService.OnParentalApprovalEmailSendSuccess += onSendSuccess;
			mixLoginCreateService.ParentalApprovalEmailSend(Service.Get<SessionManager>().LocalUser);
			Text[] parentEmailTextObjects = ParentEmailTextObjects;
			foreach (Text text in parentEmailTextObjects)
			{
				text.text = ParentEmailInputField.TextInput.text;
			}
		}

		public void onParentEmailUpdateError(IUpdateProfileResult result)
		{
			mixLoginCreateService.OnProfileUpdateSuccess -= onParentEmailUpdateSuccess;
			mixLoginCreateService.OnProfileUpdateFailed -= onParentEmailUpdateError;
			ResendContainer.gameObject.SetActive(true);
			ResendButtonText.gameObject.SetActive(true);
			ResendButtonPreloader.SetActive(false);
			ResendButton.interactable = true;
			string text = string.Empty;
			foreach (IInvalidProfileItemError error in result.Errors)
			{
				object obj = text;
				text = string.Concat(obj, "\t[", error, "]\n");
				if (error is IInvalidParentEmailError)
				{
					ParentEmailInputField.HasError = true;
					string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation("Account.Create.Validation.InvalidEmail");
					ParentEmailInputField.ShowError(tokenTranslation);
				}
				else
				{
					ParentEmailInputField.HasError = true;
					string tokenTranslation = string.Empty;
					tokenTranslation += Service.Get<Localizer>().GetTokenTranslation("Account.Create.Validation.UnknownError");
					ParentEmailInputField.ShowError(tokenTranslation);
				}
			}
		}
	}
}
