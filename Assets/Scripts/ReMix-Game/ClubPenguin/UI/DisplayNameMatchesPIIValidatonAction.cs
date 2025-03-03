using ClubPenguin.Net;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using System.Collections;

namespace ClubPenguin.UI
{
	public class DisplayNameMatchesPIIValidatonAction : InputFieldValidatonAction
	{
		private bool readyToCheck = false;

		private bool profileUpdated = false;

		public override IEnumerator Execute(ScriptableActionPlayer player)
		{
			validator = (player as InputFieldValidator);
			inputString = validator.TextInput.text;
			SessionManager sessionManager = Service.Get<SessionManager>();
			if (sessionManager.LocalUser.RegistrationProfile.FirstName == null || sessionManager.LocalUser.RegistrationProfile.ParentEmail == null)
			{
				sessionManager.LocalUser.RefreshProfile(onProfileUpdated);
			}
			else
			{
				readyToCheck = true;
				profileUpdated = true;
			}
			while (!profileUpdated)
			{
				yield return null;
			}
			if (readyToCheck)
			{
				string text = inputString.ToLower();
				int num = -1;
				int num2 = -1;
				int num3 = -1;
				int num4 = -1;
				if (!string.IsNullOrEmpty(sessionManager.LocalUser.RegistrationProfile.FirstName))
				{
					string value = sessionManager.LocalUser.RegistrationProfile.FirstName.ToLower();
					num = inputString.IndexOf(value);
				}
				if (!string.IsNullOrEmpty(sessionManager.LocalUser.RegistrationProfile.ParentEmail))
				{
					string text2 = sessionManager.LocalUser.RegistrationProfile.ParentEmail.ToLower();
					string[] array = text2.Split('@');
					if (array.Length > 0)
					{
						num2 = text.IndexOf(array[0]);
					}
					if (array.Length > 1)
					{
						num3 = text.IndexOf(array[1]);
					}
					num4 = text2.IndexOf(inputString);
				}
				HasError = (num >= 0 || num2 >= 0 || num3 >= 0 || num4 >= 0);
			}
			else
			{
				HasError = false;
				Log.LogError(this, "Unable to validate PII data due to no registrationProfile data being available");
			}
		}

		private void onProfileUpdated(IRefreshProfileResult result)
		{
			readyToCheck = result.Success;
			profileUpdated = true;
		}

		public new string GetErrorMessage()
		{
			return Service.Get<Localizer>().GetTokenTranslation(i18nErrorMessage);
		}
	}
}
