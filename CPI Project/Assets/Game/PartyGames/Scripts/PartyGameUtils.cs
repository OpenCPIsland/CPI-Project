using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using ClubPenguin.Participation;
using ClubPenguin.PartyGames;
using ClubPenguin.UI;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Game.PartyGames
{
	public static class PartyGameUtils
	{
		private const string WIN_VALUE = "win";

		private const string LOSE_VALUE = "lose";

		public static void LogGameStartBi(string partyGameName, int numPlayers)
		{
			Service.Get<ICPSwrveService>().Action("party_game", "start", partyGameName, numPlayers.ToString());
		}

		public static void LogGameEndBi(string partyGameName, int numPlayers)
		{
			Service.Get<ICPSwrveService>().Action("party_game", "finish", partyGameName, numPlayers.ToString());
		}

		public static void LogGameEndBi(string partyGameName, int numPlayers, bool won)
		{
			Service.Get<ICPSwrveService>().Action("party_game", "finish", partyGameName, numPlayers.ToString(), won ? "win" : "lose");
		}

		public static void StartBiTimer(string partyGameName, int sessionId)
		{
			string timerID = string.Format("partygame_{0}", sessionId);
			Service.Get<ICPSwrveService>().StartTimer(timerID, "party_game", null, partyGameName);
		}

		public static void StopBiTimer(int sessionId)
		{
			string timerID = string.Format("partygame_{0}", sessionId);
			Service.Get<ICPSwrveService>().EndTimer(timerID);
		}

		public static void LogBalkBi(string partyGameName)
		{
			Service.Get<ICPSwrveService>().Action("party_game", "balk", partyGameName);
		}

		public static PartyGameDefinition GetPartyGameForTriggerProp(int propId)
		{
			PartyGameDefinition result = null;
			Dictionary<int, PartyGameLauncherDefinition> dictionary = Service.Get<IGameData>().Get<Dictionary<int, PartyGameLauncherDefinition>>();
			for (int i = 0; i < dictionary.Count; i++)
			{
				if (dictionary[i].TriggerProp.Id == propId)
				{
					result = dictionary[i].PartyGame;
					break;
				}
			}
			return result;
		}

		public static PartyGameLauncherDefinition GetPartyGameLauncherForPartyGameId(int partyGameId)
		{
			PartyGameLauncherDefinition result = null;
			Dictionary<int, PartyGameLauncherDefinition> dictionary = Service.Get<IGameData>().Get<Dictionary<int, PartyGameLauncherDefinition>>();
			for (int i = 0; i < dictionary.Count; i++)
			{
				if (dictionary[i].PartyGame.Id == partyGameId)
				{
					result = dictionary[i];
					break;
				}
			}
			return result;
		}

		public static void AddParticipationFilter(ParticipationController participationController)
		{
			if (participationController != null)
			{
				participationController.AddParticipationFilter(new PartyGameParticipationFilter());
			}
		}

		public static void RemoveParticipationFilter(ParticipationController participationController)
		{
			if (participationController != null)
			{
				participationController.RemoveParticipationFilter("party_game");
			}
		}

		public static void AddActionConfirmationFilter(PartyGameDefinition definition)
		{
			Service.Get<ActionConfirmationService>().AddFilter(new PartyGameActionConfirmationFilter(definition));
		}

		public static void AddActionConfirmationFilter(PartyGameDefinition definition, string promptId)
		{
			Service.Get<ActionConfirmationService>().AddFilter(new PartyGameActionConfirmationFilter(definition, promptId));
		}

		public static void RemoveActionConfirmationFilter()
		{
			Service.Get<ActionConfirmationService>().RemoveFilter("party_game_action_filter");
		}

		public static void DisableMainNavigation()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElementGroup("MainNavButtons"));
		}

		public static void EnableMainNavigation()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElementGroup("MainNavButtons"));
		}

		public static void DisableLocomotionControls()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElementGroup("ControlsButtons"));
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement("Joystick"));
		}

		public static void EnableLocomotionControls()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElementGroup("ControlsButtons"));
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("Joystick"));
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("ActionButton"));
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("ControlsButton2"));
			SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<AbstractPenguinSnowballThrower>().EnableSnowballThrow(true);
			if (!(SceneRefs.ZoneLocalPlayerManager != null))
			{
				return;
			}
			GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			if (localPlayerGameObject != null)
			{
				LocomotionController currentController = LocomotionHelper.GetCurrentController(localPlayerGameObject);
				if (currentController is RunController)
				{
					(currentController as RunController).Behaviour.Reset();
				}
			}
			LocomotionHelper.GetCurrentController(SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject).LoadControlsLayout();
		}

		public static void DisableCellPhoneButton(bool hide = false)
		{
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement("CellphoneButton", hide));
		}

		public static void EnableCellPhoneButton()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("CellphoneButton"));
		}
	}
}
