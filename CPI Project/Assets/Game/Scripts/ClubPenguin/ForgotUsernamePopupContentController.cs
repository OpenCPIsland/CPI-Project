using ClubPenguin.Analytics;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class ForgotUsernamePopupContentController : MonoBehaviour
	{
		public InputFieldValidator EmailAddressInputField;

		public Button ContinueButton;

		public GameObject FormContainer;

		public GameObject SuccessContainer;

		private AbstractLoginController loginController;

		private void Start()
		{
			Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().AccountFunnelName, "12", "forgot_username");
			Service.Get<ICPSwrveService>().Action("view.forgot", "username");
			loginController = GetComponentInParent<AbstractLoginController>();
		}

		private void OnEnable()
		{
			ContinueButton.interactable = true;
		}

		private void OnDisable()
		{
			ContinueButton.interactable = false;
		}

		public void OnContinueButtonClicked()
		{
			loginController.OnRecoveryFailed += onRecoveryFailed;
			loginController.OnRecoverySuccess += onRecoverySuccess;
			loginController.UsernameRecoverySend(EmailAddressInputField.TextInput.text);
		}

		private void onRecoveryFailed()
		{
			loginController.OnRecoveryFailed -= onRecoveryFailed;
			loginController.OnRecoverySuccess -= onRecoverySuccess;
			string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation("Account.Login.ForgotPassword.NoAccountFound");
			EmailAddressInputField.ShowError(tokenTranslation);
		}

		private void onRecoverySuccess()
		{
			Service.Get<ICPSwrveService>().Action("recovery_triggered", "username");
			loginController.OnRecoveryFailed -= onRecoveryFailed;
			loginController.OnRecoverySuccess -= onRecoverySuccess;
			FormContainer.gameObject.SetActive(false);
			SuccessContainer.gameObject.SetActive(true);
		}
	}
}
