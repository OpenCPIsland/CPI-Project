using ClubPenguin.UI;
using Disney.Mix.SDK;
using System;
using System.Runtime.InteropServices;

namespace ClubPenguin.Net
{
	public static class SessionErrorEvents
	{
		public struct AccountBannedEvent
		{
			public readonly AlertType Category;

			public readonly DateTime? ExpirationDate;

			public AccountBannedEvent(AlertType category, DateTime? expirationDate)
			{
				Category = category;
				ExpirationDate = expirationDate;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct AuthenticationRequiresParentalConsentEvent
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct AuthenticationRevokedEvent
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct AuthenticationUnavailableEvent
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct SessionTerminated
		{
		}

		public struct SessionDataCorrupted
		{
			public readonly bool Recovered;

			public SessionDataCorrupted(bool recovered)
			{
				Recovered = recovered;
			}
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct NoNetworkOnResumeError
		{
		}

		[StructLayout(LayoutKind.Sequential, Size = 1)]
		public struct NoSessionOnResumeError
		{
		}

		public struct RegistrationConfigError
		{
			public readonly string TitleToken;

			public readonly string MessageToken;

			public readonly string Step;

			public RegistrationConfigError(string titleToken, string messageToken, string step)
			{
				TitleToken = titleToken;
				MessageToken = messageToken;
				Step = step;
			}
		}

		public static void AccountBannedPromptSetup(DPrompt promptData, AlertType bannedCategory, DateTime? expirationDate)
		{
			string text = string.Empty;
			if (expirationDate.HasValue)
			{
				TimeSpan value = (expirationDate - DateTime.Now).Value;
				if (value.TotalHours >= 0.0)
				{
					text = string.Format("{0}hrs {1}m", (int)value.TotalHours, Math.Max(1, value.Minutes));
				}
			}
			promptData.SetText("Moderation.Text.Time", text, true);
			string i18nText = string.IsNullOrEmpty(text) ? "Account.PermaBan.Body" : "Account.TemporaryBan.Body";
			promptData.SetText(DPrompt.PROMPT_TEXT_BODY, i18nText);
		}
	}
}
