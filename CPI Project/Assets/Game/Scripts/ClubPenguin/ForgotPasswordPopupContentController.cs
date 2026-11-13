using ClubPenguin.Analytics;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	public class ForgotPasswordPopupContentController : MonoBehaviour
	{
		public InputFieldValidator UsernameInputField;

		public Button ContinueButton;

		public GameObject FormContainer;

		public GameObject SuccessContainer;

		private AbstractLoginController loginController;

		private void Start()
		{
			Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().AccountFunnelName, "11", "forgot_password");
			Service.Get<ICPSwrveService>().Action("view.forgot", "password");
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
			loginController.PasswordRecoverySend(UsernameInputField.TextInput.text);
		}

		private void onRecoveryFailed()
		{
			loginController.OnRecoveryFailed -= onRecoveryFailed;
			loginController.OnRecoverySuccess -= onRecoverySuccess;
			string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation("Account.Login.ForgotPassword.NoAccountFound");
			UsernameInputField.ShowError(tokenTranslation);
		}

		private void onRecoverySuccess()
		{
			Service.Get<ICPSwrveService>().Action("recovery_triggered", "password");
			loginController.OnRecoveryFailed -= onRecoveryFailed;
			loginController.OnRecoverySuccess -= onRecoverySuccess;
			FormContainer.gameObject.SetActive(false);
			SuccessContainer.gameObject.SetActive(true);
		}
	}
}
