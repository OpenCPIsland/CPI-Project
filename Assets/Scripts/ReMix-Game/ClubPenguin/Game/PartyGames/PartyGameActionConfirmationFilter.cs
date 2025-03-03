using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.PartyGames;
using ClubPenguin.Props;
using ClubPenguin.UI;
using DevonLocalization.Core;
using Disney.MobileNetwork;
using System;

namespace ClubPenguin.Game.PartyGames
{
	public class PartyGameActionConfirmationFilter : IActionConfirmationFilter
	{
		public const string FILTER_ID = "party_game_action_filter";

		private const string PROMPT_ID = "PartyGameExitPrompt";

		private PartyGameDefinition partyGameDefinition;

		private string promptId = "PartyGameExitPrompt";

		public PartyGameActionConfirmationFilter(PartyGameDefinition definition, string promptId = "PartyGameExitPrompt")
		{
			partyGameDefinition = definition;
			this.promptId = promptId;
		}

		public bool IsActionValid(Type type, object payload)
		{
			bool result = true;
			if (type == typeof(PropCancel) || type == typeof(ZoneTransition) || type == typeof(PlayerCardEvents.JoinPlayer))
			{
				result = false;
			}
			return result;
		}

		public void ShowConfirmation(Type type, ActionConfirmationService.FilterCallback callback)
		{
			string tokenTranslation = Service.Get<Localizer>().GetTokenTranslation(partyGameDefinition.Name);
			Service.Get<PromptManager>().ShowPrompt(promptId, tokenTranslation, null, delegate(DPrompt.ButtonFlags result)
			{
				if (result == DPrompt.ButtonFlags.YES)
				{
					Service.Get<CPDataEntityCollection>().GetComponent<HeldObjectsData>(Service.Get<CPDataEntityCollection>().LocalPlayerHandle).HeldObject = null;
					logPlayerQuitBI();
				}
				callback(result == DPrompt.ButtonFlags.YES);
			});
		}

		public string GetFilterId()
		{
			return "party_game_action_filter";
		}

		private void logPlayerQuitBI()
		{
			Service.Get<ICPSwrveService>().Action("quit_game", partyGameDefinition.Name);
		}
	}
}
