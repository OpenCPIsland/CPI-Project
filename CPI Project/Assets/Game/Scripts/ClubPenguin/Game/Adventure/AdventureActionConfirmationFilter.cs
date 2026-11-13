using ClubPenguin.Adventure;
using ClubPenguin.Core;
using ClubPenguin.Igloo.Catalog;
using ClubPenguin.Locomotion;
using ClubPenguin.Net.Locomotion;
using ClubPenguin.Props;
using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using UnityEngine;

namespace ClubPenguin.Game.Adventure
{
	public class AdventureActionConfirmationFilter : IActionConfirmationFilter
	{
		public const string FILTER_ID = "adventure_action_filter";

		private const string PROMPT_ID_BLOCKED_DURING_FTUE = "AdventureBlockedDuringFtuePrompt";

		private const string PROMPT_ID_EXIT_TO_PARTYGAME = "AdventureExitToPartyGamePrompt";

		public bool IsActionValid(Type action, object payload)
		{
			bool result = true;
			if (action == typeof(LocomotionBroadcaster.InteractActionFilterTag) || action == typeof(PenguinInteraction))
			{
				result = isInteractActionValid(payload);
			}
			else if (action == typeof(PropService.UserPropOnActionEventFilterTag))
			{
				result = isUsePropOnActionValid(payload);
			}
			else if (action == typeof(PlayerCardIglooButton) || action == typeof(MyIglooTransitionButton) || action == typeof(PlayerCardEvents.JoinPlayer) || action == typeof(IglooCatalogPurchaseConfirmation))
			{
				result = false;
			}
			return result;
		}

		public void ShowConfirmation(Type action, ActionConfirmationService.FilterCallback callback)
		{
			if (isFtueQuestActive())
			{
				Service.Get<PromptManager>().ShowPrompt("AdventureBlockedDuringFtuePrompt", delegate
				{
					callback(false);
				});
			}
			else
			{
				Service.Get<PromptManager>().ShowPrompt("AdventureExitToPartyGamePrompt", delegate(DPrompt.ButtonFlags result)
				{
					if (result == DPrompt.ButtonFlags.YES)
					{
						Service.Get<EventDispatcher>().DispatchEvent(new QuestEvents.SuspendQuest(Service.Get<QuestService>().ActiveQuest));
					}
					callback(result == DPrompt.ButtonFlags.YES);
				});
			}
		}

		public string GetFilterId()
		{
			return "adventure_action_filter";
		}

		private bool isUsePropOnActionValid(object payload)
		{
			bool result = true;
			PropService propService = Service.Get<PropService>();
			if (propService.LocalPlayerPropUser != null && propService.LocalPlayerPropUser.Prop != null)
			{
				PropDefinition propDef = Service.Get<PropService>().LocalPlayerPropUser.Prop.PropDef;
				if (propDef.PropType == PropDefinition.PropTypes.Consumable && propDef.ExperienceType == PropDefinition.ConsumableExperience.PartyGameLobby)
				{
					result = false;
				}
			}
			return result;
		}

		private bool isInteractActionValid(object payload)
		{
			bool flag = true;
			GameObject gameObject = (GameObject)payload;
			if (gameObject != null)
			{
				PropExperience componentInChildren = gameObject.GetComponentInChildren<PropExperience>(gameObject);
				if (componentInChildren != null && componentInChildren.PropDef.ExperienceType == PropDefinition.ConsumableExperience.PartyGameLobby)
				{
					flag = false;
				}
				if (flag)
				{
					AdventureDisabledBehaviourTag componentInChildren2 = gameObject.GetComponentInChildren<AdventureDisabledBehaviourTag>();
					if (componentInChildren2 != null)
					{
						flag = false;
					}
				}
			}
			return flag;
		}

		private bool isFtueQuestActive()
		{
			bool result = false;
			Quest activeQuest = Service.Get<QuestService>().ActiveQuest;
			if (activeQuest != null && activeQuest.Definition.name == Service.Get<GameStateController>().FTUEConfig.FtueQuestId)
			{
				result = true;
			}
			return result;
		}
	}
}
