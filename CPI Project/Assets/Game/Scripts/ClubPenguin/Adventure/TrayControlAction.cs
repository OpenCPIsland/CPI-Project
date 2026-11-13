using ClubPenguin.Cinematography;
using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using ClubPenguin.UI;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using System;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("GUI")]
	public class TrayControlAction : FsmStateAction
	{
		public enum MainNavButtonActionType
		{
			none,
			enable,
			disable
		}

		public enum TrayControlActionType
		{
			none,
			selectScreen,
			maxCinematic,
			minCinematic,
			exitCinematic,
			minMainNav
		}

		public enum TrayScreens
		{
			none,
			quest,
			consumable,
			option,
			penguin
		}

		public MainNavButtonActionType MainNavActionType;

		public TrayControlActionType TrayActionType;

		public TrayScreens Screen;

		public string SubScreen;

		private StateMachineContext trayFSMContext;

		private bool canFinish = false;

		private bool isWaitingForFSM = false;

		private System.Action lastTrayAction = null;

		public override void OnEnter()
		{
			findWorldTrayFSMContext();
			canFinish = trayFSMContext.ContainsStateMachine("Root");
			switch (TrayActionType)
			{
			case TrayControlActionType.exitCinematic:
				lastTrayAction = exitCinematic;
				break;
			case TrayControlActionType.selectScreen:
				lastTrayAction = selectTrayScreen;
				break;
			case TrayControlActionType.maxCinematic:
				lastTrayAction = enterMaxCinematicAndSelectScreen;
				break;
			case TrayControlActionType.minCinematic:
				lastTrayAction = enterMinCinematic;
				break;
			case TrayControlActionType.minMainNav:
				lastTrayAction = enterMinMainNav;
				break;
			}
			switch (MainNavActionType)
			{
			case MainNavButtonActionType.disable:
				disableMainNav();
				break;
			case MainNavButtonActionType.enable:
				enableMainNav();
				break;
			}
			resetLocomotionState();
			if (canFinish)
			{
				if (lastTrayAction != null)
				{
					lastTrayAction();
				}
				Finish();
			}
			else if (!isWaitingForFSM)
			{
				trayFSMContext.StateMachineLoaded += FinishWhenFSMAdded;
				isWaitingForFSM = true;
			}
		}

		private void resetLocomotionState()
		{
			GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			if (localPlayerGameObject != null)
			{
				LocomotionController currentController = LocomotionHelper.GetCurrentController(localPlayerGameObject);
				if (currentController != null)
				{
					currentController.ResetState();
				}
			}
		}

		private void findWorldTrayFSMContext()
		{
			GameObject gameObject = GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root);
			if (gameObject != null)
			{
				trayFSMContext = gameObject.GetComponent<StateMachineContext>();
			}
		}

		private void enableMainNav()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerCardEvents.SetEnablePlayerCard(true));
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElementGroup("MainNavButtons"));
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("ChatButtons"));
		}

		private void disableMainNav()
		{
			Service.Get<EventDispatcher>().DispatchEvent(new PlayerCardEvents.SetEnablePlayerCard(false));
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElementGroup("MainNavButtons"));
			Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement("ChatButtons"));
		}

		private void enterMaxCinematic()
		{
			trayFSMContext.SendEvent(new ExternalEvent("Root", "maxnpc"));
		}

		private void enterMaxCinematicAndSelectScreen()
		{
			enterMaxCinematic();
			selectTrayScreen();
		}

		private void enterMinCinematic()
		{
			trayFSMContext.SendEvent(new ExternalEvent("Root", "minnpc"), true);
		}

		private void exitCinematic()
		{
			Service.Get<EventDispatcher>().DispatchEvent(default(CinematographyEvents.ClearGroupCullingOverride));
			trayFSMContext.SendEvent(new ExternalEvent("Root", "exit_cinematic"));
		}

		private void enterMinMainNav()
		{
			trayFSMContext.SendEvent(new ExternalEvent("Root", "min_mainnav"));
		}

		private void selectTrayScreen()
		{
			if (Screen != 0)
			{
				trayFSMContext.SendEvent(new ExternalEvent("Root", "maxnpc"));
				trayFSMContext.SendEvent(new ExternalEvent("ScreenContainerContent", Screen.ToString()));
			}
		}

		private void FinishWhenFSMAdded(string fsmName)
		{
			if (fsmName == "Root")
			{
				trayFSMContext.StateMachineLoaded -= FinishWhenFSMAdded;
				isWaitingForFSM = false;
				if (lastTrayAction != null)
				{
					lastTrayAction();
					lastTrayAction = null;
				}
				Finish();
			}
		}
	}
}
