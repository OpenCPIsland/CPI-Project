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
	public class EndQuestCinematicModeAction : FsmStateAction
	{
		public bool UnsuppressQuestNotifier = true;

		public bool ShowControls = true;

		public bool ShowRemotePlayers = false;

		public bool ShowLocalPlayer = false;

		public bool ShowInWorldText = true;

		public bool EnablePlayerCard = true;

		public bool ShowActionIndicators = true;

		private EventDispatcher dispatcher;

		public override void OnEnter()
		{
			dispatcher = Service.Get<EventDispatcher>();
			if (UnsuppressQuestNotifier)
			{
				dispatcher.DispatchEvent(new HudEvents.SuppressQuestNotifier(false));
			}
			if (ShowControls)
			{
				GameObject gameObject = GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root);
				if (gameObject != null)
				{
					StateMachineContext component = gameObject.GetComponent<StateMachineContext>();
					component.SendEvent(new ExternalEvent("Root", "exit_cinematic"));
				}
				Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElementGroup("MainNavButtons"));
				Service.Get<EventDispatcher>().DispatchEvent(new UIDisablerEvents.EnableUIElement("ChatButtons"));
			}
			if (ShowRemotePlayers)
			{
				RemotePlayerVisibilityState.ShowRemotePlayers();
			}
			if (ShowLocalPlayer)
			{
				Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("LocalPlayer");
			}
			if (ShowInWorldText)
			{
				dispatcher.DispatchEvent(default(InWorldUIEvents.EnableInWorldText));
			}
			if (EnablePlayerCard)
			{
				dispatcher.DispatchEvent(new PlayerCardEvents.SetEnablePlayerCard(true));
			}
			if (ShowActionIndicators)
			{
				dispatcher.DispatchEvent(default(InWorldUIEvents.EnableActionIndicators));
			}
			Finish();
		}
	}
}
