using ClubPenguin.Core;
using ClubPenguin.UI;
using ClubPenguin.World;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Quest")]
	public class StartQuestCinematicModeAction : FsmStateAction
	{
		public bool SuppressQuestNotifier = true;

		public bool RemoveControls = true;

		public bool HideRemotePlayers = false;

		public bool HideLocalPlayer = false;

		public bool HideInWorldText = true;

		public bool DisablePlayerCard = true;

		public bool HideActionIndicators = true;

		private EventDispatcher dispatcher;

		public override void OnEnter()
		{
			dispatcher = Service.Get<EventDispatcher>();
			if (SuppressQuestNotifier)
			{
				dispatcher.DispatchEvent(new HudEvents.SuppressQuestNotifier(true, true));
			}
			if (RemoveControls)
			{
				GameObject gameObject = GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root);
				if (gameObject != null)
				{
					StateMachineContext component = gameObject.GetComponent<StateMachineContext>();
					component.SendEvent(new ExternalEvent("Root", "minnpc"), true);
				}
				Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElementGroup("MainNavButtons"));
				Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.DisableUIElement("ChatButtons"));
			}
			if (HideRemotePlayers)
			{
				RemotePlayerVisibilityState.HideRemotePlayers();
			}
			if (HideLocalPlayer)
			{
				Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("LocalPlayer"));
			}
			if (HideInWorldText)
			{
				dispatcher.DispatchEvent(default(InWorldUIEvents.DisableInWorldText));
			}
			if (DisablePlayerCard)
			{
				dispatcher.DispatchEvent(new PlayerCardEvents.SetEnablePlayerCard(false));
			}
			if (HideActionIndicators)
			{
				dispatcher.DispatchEvent(default(InWorldUIEvents.DisableActionIndicators));
			}
			Finish();
		}
	}
}
