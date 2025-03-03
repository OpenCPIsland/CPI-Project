using ClubPenguin.Net;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.Mix.SDK;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ClubPenguin.UI
{
	public class ServerSideDisplayNameValidatonAction : InputFieldValidatonAction
	{
		private SessionManager sessionManager;

		public bool EnableNameSuggestions = true;

		public int MaxSuggestedNames = 3;

		public int MaxAppendedNumbers = 5;

		public int MaxSuggestedUsernameLength = 14;

		public int MaxAttemptsIncrementing = 2;

		public int MaxAttempts3Digits = 5;

		public int MaxAttempts4Digits = 3;

		public int MaxSuggestionRounds = 1;

		private IEnumerable<string> found;

		private string lastInputString;

		private int numSuggestionRounds = 0;

		private bool isBaseValidationDone;

		private int currentNumber;

		private Stopwatch sw;

		private int maxTime;

		private AccountFlowData accountFlowData;

		public override IEnumerator Execute(ScriptableActionPlayer player)
		{
			validator = (player as InputFieldValidator);
			inputString = validator.TextInput.text;
			sessionManager = Service.Get<SessionManager>();
			sw = new Stopwatch();
			sw.Start();
			maxTime = 30000;
			isBaseValidationDone = false;
			if (inputString == sessionManager.LocalUser.DisplayName.Text)
			{
				HasError = false;
				yield break;
			}
			accountFlowData = Service.Get<MembershipService>().GetAccountFlowData();
			if (!string.IsNullOrEmpty(inputString) && accountFlowData.PreValidatedDisplayNames.Contains(inputString))
			{
				HasError = false;
			}
			if (found != null && found.Contains(inputString))
			{
				HasError = false;
				yield break;
			}
			if (lastInputString != null && inputString == lastInputString)
			{
				HasError = true;
				yield break;
			}
			sessionManager.LocalUser.ValidateDisplayNameV2(inputString, onValidateComplete);
			while (!isBaseValidationDone && sw.ElapsedMilliseconds < maxTime)
			{
				yield return null;
			}
			if (!isBaseValidationDone)
			{
				HasError = false;
			}
		}

		public void onValidateComplete(IValidateDisplayNameResult validateResult)
		{
			if (validateResult.Success)
			{
				lastInputString = null;
				HasError = false;
				accountFlowData.PreValidatedDisplayNames.Add(inputString);
			}
			else
			{
				HasError = true;
				i18nErrorMessage = "Account.DisplayName.Validation.InvalidDisplayName";
				switch (validateResult.GetType().Name)
				{
				case "IValidateDisplayNameExistsResult":
				case "ValidateDisplayNameExistsResult":
					i18nErrorMessage = "Acount.Displayname.Validation.AlreadyUsed";
					foreach (string suggestedDisplayName in validateResult.SuggestedDisplayNames)
					{
						accountFlowData.PreValidatedDisplayNames.Add(suggestedDisplayName);
					}
					break;
				case "IValidateDisplayNameFailedModerationResult":
				case "ValidateDisplayNameFailedModerationResult":
					i18nErrorMessage = "Acount.Displayname.Validation.NotAllowed";
					break;
				default:
					i18nErrorMessage = "Account.Create.Validation.UnknownError";
					break;
				}
			}
			isBaseValidationDone = true;
		}

		public override string GetErrorMessage()
		{
			AccountFlowData accountFlowData = Service.Get<MembershipService>().GetAccountFlowData();
			if (i18nErrorMessage == "Acount.Displayname.Validation.AlreadyUsed" && accountFlowData.PreValidatedDisplayNames != null && accountFlowData.PreValidatedDisplayNames.Count() > 0)
			{
				i18nErrorMessage = "Acount.Displayname.Validation.AlreadyUsedWithSuggestions";
				string text = "";
				int num = 0;
				foreach (string preValidatedDisplayName in accountFlowData.PreValidatedDisplayNames)
				{
					text = text + "\n\t<a href=\"#suggestedName\">" + preValidatedDisplayName + "</a>";
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
