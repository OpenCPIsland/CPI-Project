using ClubPenguin.Mix;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using System.Collections;
using System.Diagnostics;

namespace ClubPenguin.UI
{
	public class ServerSidePasswordValidatonAction : InputFieldValidatonAction
	{
		private MixLoginCreateService loginService;

		private bool isBaseValidationDone;

		private bool isWaiting = false;

		private Stopwatch sw;

		private int maxTime;

		public override IEnumerator Execute(ScriptableActionPlayer player)
		{
			if (!isWaiting)
			{
				setup(player);
				isBaseValidationDone = false;
			}
			loginService = Service.Get<MixLoginCreateService>();
			while (loginService.ValidationInProgress)
			{
				yield return null;
			}
			loginService.ValidationInProgress = true;
			loginService.OnValidationSuccess += onValidationSuccess;
			loginService.OnValidationFailed += onValidationFailed;
			loginService.ValidateUsernamePassword("K4fR0VfK4MToQslVupGkGvAKFqw3HBXOfkpXalYUX1Kv5kbKL08MNxk3W2gfjk0", inputString);
			isWaiting = true;
			sw = new Stopwatch();
			sw.Start();
			maxTime = 30000;
			while (!isBaseValidationDone && sw.ElapsedMilliseconds < maxTime)
			{
				yield return null;
			}
			if (!isBaseValidationDone)
			{
				loginService.OnValidationSuccess -= onValidationSuccess;
				loginService.OnValidationFailed -= onValidationFailed;
				HasError = false;
			}
			isWaiting = false;
			loginService.ValidationInProgress = false;
		}

		private void onValidationSuccess()
		{
			loginService.OnValidationSuccess -= onValidationSuccess;
			loginService.OnValidationFailed -= onValidationFailed;
			HasError = false;
			isBaseValidationDone = true;
		}

		private void onValidationFailed(IValidateNewAccountResult result)
		{
			loginService.OnValidationSuccess -= onValidationSuccess;
			loginService.OnValidationFailed -= onValidationFailed;
			HasError = true;
			string text = string.Empty;
			if (result.Errors != null)
			{
				foreach (IValidateNewAccountError error in result.Errors)
				{
					object obj = text;
					text = string.Concat(obj, "\t[", error, "] ");
					if (error is IValidateNewAccountNothingToValidateError)
					{
						i18nErrorMessage = Service.Get<Localizer>().GetTokenTranslationFormatted("Account.Create.Validation.Empty", "Account.Create.Validation.PasswrodField");
					}
					else if (error is IValidateNewAccountPassswordMissingCharactersError)
					{
						i18nErrorMessage = "Account.Create.Validation.PasswordComplexity";
					}
					else if (error is IValidateNewAccountPasswordCommonError)
					{
						i18nErrorMessage = "Acount.Create.Validate.TooCommon";
					}
					else if (error is IValidateNewAccountPasswordPhoneError)
					{
						i18nErrorMessage = "Account.Create.Validation.PasswordPhoneNumber";
					}
					else if (error is IValidateNewAccountPasswordSizeError)
					{
						i18nErrorMessage = "Account.Create.Validation.PasswordLength";
					}
					else if (error is IValidateNewAccountRateLimitedError)
					{
						string type = "";
						string format = "Account.General.Error.RateLimited";
						Service.Get<EventDispatcher>().DispatchEvent(new ApplicationService.Error(type, format));
					}
					text += i18nErrorMessage;
					text += "\n";
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				i18nErrorMessage = "Account.Create.Validation.UnknownError";
				text += i18nErrorMessage;
				text += "\n";
			}
			isBaseValidationDone = true;
		}

		public override string GetErrorMessage()
		{
			return Service.Get<Localizer>().GetTokenTranslation(i18nErrorMessage);
		}
	}
}
