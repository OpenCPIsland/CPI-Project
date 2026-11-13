using ClubPenguin.Mix;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class ServerSideUserNameValidatonAction : InputFieldValidatonAction
	{
		public bool EnableNameSuggestions = true;

		public int MaxSuggestedNames = 3;

		public int MaxSuggestedUsernameLength = 14;

		public int MaxAttemptsIncrementing = 2;

		public int MaxAttempts3Digits = 5;

		public int MaxAttempts4Digits = 3;

		public int MaxSuggestionRounds = 1;

		private List<string> suggestedNames;

		private int numSuggestionRounds;

		private int currentNumber;

		private string currentSuggestedUsername;

		private MixLoginCreateService loginService;

		private bool isBaseValidationDone;

		private bool isNameSuggestionValidationDone;

		private string displayNameStub;

		private bool isWaiting = false;

		private Stopwatch sw;

		private int maxTime;

		private AccountFlowData accountFlowData;

		public override IEnumerator Execute(ScriptableActionPlayer player)
		{
			accountFlowData = Service.Get<MembershipService>().GetAccountFlowData();
			if (!isWaiting)
			{
				setup(player);
				isBaseValidationDone = false;
				isNameSuggestionValidationDone = false;
				if (accountFlowData.PreValidatedUserNames == null || accountFlowData.PreValidatedUserNames.Count == 0)
				{
					numSuggestionRounds = 0;
				}
			}
			loginService = Service.Get<MixLoginCreateService>();
			while (loginService.ValidationInProgress)
			{
				yield return null;
			}
			loginService.ValidationInProgress = true;
			loginService.OnValidationSuccess += onValidationSuccess;
			loginService.OnValidationFailed += onValidationFailed;
			loginService.ValidateUsernamePassword(inputString, "testing123");
			isWaiting = true;
			sw = new Stopwatch();
			sw.Start();
			maxTime = 30000;
			while ((!isBaseValidationDone || !isNameSuggestionValidationDone) && sw.ElapsedMilliseconds < maxTime)
			{
				yield return null;
			}
			if (!isBaseValidationDone)
			{
				loginService.OnValidationSuccess -= onValidationSuccess;
				loginService.OnValidationFailed -= onValidationFailed;
				HasError = false;
			}
			if (!isNameSuggestionValidationDone)
			{
				loginService.OnValidationSuccess -= onSuggestionValidationSuccess;
				loginService.OnValidationFailed -= onSuggestionValidationFailed;
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
			isNameSuggestionValidationDone = true;
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
					if (error is IValidateNewAccountAccountInUseError)
					{
						i18nErrorMessage = "Acount.Create.Validation.AlreadyUsed";
						displayNameStub = Regex.Replace(inputString, "[0-9]+$", string.Empty);
						if (EnableNameSuggestions && numSuggestionRounds < MaxSuggestionRounds && displayNameStub.Length < MaxSuggestedUsernameLength)
						{
							numSuggestionRounds++;
							isNameSuggestionValidationDone = false;
							currentNumber = 0;
							int.TryParse(Regex.Replace(inputString, displayNameStub, string.Empty), out currentNumber);
							suggestedNames = new List<string>();
							CoroutineRunner.Start(getDisplayNameSuggestions(), this, "SuggestedNameValidation");
						}
						else
						{
							isNameSuggestionValidationDone = true;
						}
					}
					else if (error is IValidateNewAccountMultipleAccountsError)
					{
						isNameSuggestionValidationDone = true;
						Service.Get<MixLoginCreateService>().MultipleAccountsResolutionSend(inputString);
						Service.Get<PromptManager>().ShowError("Account.Login.Error.MultipleAccountsSameEmail", "Account.Login.Error.ResolveInstructionsEmailSent");
						i18nErrorMessage = "Account.Login.Error.MultipleAccountsSameEmail";
					}
					else if (error is IValidateNewAccountNotRegisteredTransactorError)
					{
						isNameSuggestionValidationDone = true;
						Service.Get<MixLoginCreateService>().NonRegisteredTransactorUpgradeSend(inputString);
						Service.Get<PromptManager>().ShowError("Account.Login.Error.NonRegisteredTransactor", "Account.Login.Error.ResolveInstructionsEmailSent");
						i18nErrorMessage = "Account.Login.Error.NonRegisteredTransactor";
					}
					else if (error is IValidateNewAccountNothingToValidateError)
					{
						isNameSuggestionValidationDone = true;
						i18nErrorMessage = Service.Get<Localizer>().GetTokenTranslationFormatted("Account.Create.Validation.Empty", "Account.Create.Validation.UsernameField");
					}
					else if (error is IValidateNewAccountUsernameError)
					{
						isNameSuggestionValidationDone = true;
						i18nErrorMessage = "Account.Create.Validation.InvalidUsername";
					}
					else if (error is IValidateNewAccountRateLimitedError)
					{
						isNameSuggestionValidationDone = true;
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
				isNameSuggestionValidationDone = true;
				i18nErrorMessage = "Account.Create.Validation.UnknownError";
				text += i18nErrorMessage;
				text += "\n";
			}
			isBaseValidationDone = true;
		}

		private IEnumerator getDisplayNameSuggestions()
		{
			UnityEngine.Random.InitState(DateTime.Now.Millisecond);
			yield return CoroutineRunner.Start(getNextDisplayNameSuggestion(), this, "SuggestedNameValidation");
			while (!isNameSuggestionValidationDone)
			{
				yield return null;
			}
		}

		private void onSuggestionValidationSuccess()
		{
			loginService.OnValidationSuccess -= onSuggestionValidationSuccess;
			loginService.OnValidationFailed -= onSuggestionValidationFailed;
			accountFlowData.PreValidatedUserNames.Add(currentSuggestedUsername);
			CoroutineRunner.Start(getNextDisplayNameSuggestion(), this, "SuggestedNameValidation");
		}

		private void onSuggestionValidationFailed(IValidateNewAccountResult result)
		{
			loginService.OnValidationSuccess -= onSuggestionValidationSuccess;
			loginService.OnValidationFailed -= onSuggestionValidationFailed;
			CoroutineRunner.Start(getNextDisplayNameSuggestion(), this, "SuggestedNameValidation");
		}

		private IEnumerator getNextDisplayNameSuggestion()
		{
			currentSuggestedUsername = "";
			if (accountFlowData.PreValidatedUserNames.Count < 1 && suggestedNames.Count < MaxAttemptsIncrementing)
			{
				currentNumber++;
				currentSuggestedUsername = displayNameStub + currentNumber;
			}
			else if (accountFlowData.PreValidatedUserNames.Count < MaxSuggestedNames && suggestedNames.Count < MaxAttemptsIncrementing + MaxAttempts3Digits)
			{
				currentSuggestedUsername = displayNameStub + UnityEngine.Random.Range(1, 999);
				if (currentSuggestedUsername.Length > MaxSuggestedUsernameLength)
				{
					currentSuggestedUsername = currentSuggestedUsername.Substring(0, MaxSuggestedUsernameLength);
				}
			}
			else if (accountFlowData.PreValidatedUserNames.Count < MaxSuggestedNames && suggestedNames.Count < MaxAttemptsIncrementing + MaxAttempts3Digits + MaxAttempts4Digits)
			{
				currentSuggestedUsername = displayNameStub + UnityEngine.Random.Range(1, 9999);
				if (currentSuggestedUsername.Length > MaxSuggestedUsernameLength)
				{
					currentSuggestedUsername = currentSuggestedUsername.Substring(0, MaxSuggestedUsernameLength);
				}
			}
			if (!string.IsNullOrEmpty(currentSuggestedUsername))
			{
				maxTime += 5000;
				loginService.OnValidationSuccess += onSuggestionValidationSuccess;
				loginService.OnValidationFailed += onSuggestionValidationFailed;
				suggestedNames.Add(currentSuggestedUsername);
				loginService.ValidateUsernamePassword(currentSuggestedUsername, "testing123");
				yield return null;
			}
			else
			{
				isNameSuggestionValidationDone = true;
			}
		}

		public override string GetErrorMessage()
		{
			if (accountFlowData.PreValidatedUserNames != null && accountFlowData.PreValidatedUserNames.Any())
			{
				if (i18nErrorMessage == "Acount.Create.Validation.AlreadyUsed")
				{
					i18nErrorMessage = "Acount.Create.Validation.Username.AlreadyUsedWithSuggestions";
				}
				string text = "";
				int num = 0;
				foreach (string preValidatedUserName in accountFlowData.PreValidatedUserNames)
				{
					text = text + "\n\t<a href=\"#suggestedName\">" + preValidatedUserName + "</a>";
					if (++num > MaxSuggestedNames)
					{
						break;
					}
				}
				string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation(i18nErrorMessage);
				return string.Format(tokenTranslation, text);
			}
			return Service.Get<Localizer>().GetTokenTranslation(i18nErrorMessage);
		}
	}
}
