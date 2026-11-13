using ClubPenguin.Analytics;
using ClubPenguin.UI;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin
{
	public class LoginPopupCredentialsController : LoginPopupContentController
	{
		[Header("Credentials")]
		public InputFieldValidator UsernameField;

		public InputFieldValidator PasswordField;

		private bool credentialsError = false;

		public override void Start()
		{
			Service.Get<ICPSwrveService>().Funnel(Service.Get<MembershipService>().AccountFunnelName, "10", "login");
			base.Start();
		}

		public void Update()
		{
			if (credentialsError)
			{
				if (!UsernameField.HasError)
				{
					PasswordField.StartValidation();
				}
				if (!PasswordField.HasError)
				{
					UsernameField.StartValidation();
				}
				if (!PasswordField.HasError && !UsernameField.HasError)
				{
					credentialsError = false;
				}
			}
		}

		protected override void showUsernameError(string error)
		{
			UsernameField.HasError = !string.IsNullOrEmpty(error);
			UsernameField.ShowError(error);
		}

		protected override void showPasswordError(string error)
		{
			PasswordField.HasError = !string.IsNullOrEmpty(error);
			PasswordField.ShowError(error);
		}

		protected override void onInvalidCredentialsError(string error)
		{
			PasswordField.HasError = !string.IsNullOrEmpty(error);
			UsernameField.HasError = !string.IsNullOrEmpty(error);
			UsernameField.ShowError(string.Empty);
			PasswordField.ShowError(string.Empty);
			credentialsError = true;
		}

		public void OnLoginClicked()
		{
			performLogin(new DLoginPayload(UsernameField.TextInput.text, PasswordField.TextInput.text));
		}
	}
}
