using ClubPenguin.Actions;
using ClubPenguin.Cinematography;
using ClubPenguin.Core;
using ClubPenguin.UI;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	public class EndMascotInteractionAction : ClubPenguin.Actions.Action
	{
		public CameraController Controller;

		public string ScreenName;

		private Mascot mascot;

		protected override void OnEnable()
		{
			if (!Owner.CompareTag("Player"))
			{
				return;
			}
			EventDispatcher eventDispatcher = Service.Get<EventDispatcher>();
			MascotController componentInParent = SceneRefs.ActionSequencer.GetTrigger(Owner).GetComponentInParent<MascotController>();
			mascot = componentInParent.Mascot;
			if (mascot.InteractionBehaviours.ZoomOut && Controller != null)
			{
				CinematographyEvents.CameraLogicResetEvent evt = default(CinematographyEvents.CameraLogicResetEvent);
				evt.Controller = Controller;
				eventDispatcher.DispatchEvent(evt);
			}
			if (mascot.InteractionBehaviours.RestoreTray)
			{
				GameObject gameObject = GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root);
				if (gameObject != null)
				{
					StateMachineContext component = gameObject.GetComponent<StateMachineContext>();
					component.SendEvent(new ExternalEvent("Root", "exit_cinematic"));
					if (!string.IsNullOrEmpty(ScreenName))
					{
						component.SendEvent(new ExternalEvent("ScreenContainerContent", ScreenName));
					}
				}
				eventDispatcher.DispatchEvent(new PlayerCardEvents.SetEnablePlayerCard(true));
				eventDispatcher.DispatchEvent(new UIDisablerEvents.EnableUIElementGroup("MainNavButtons"));
			}
			if (mascot.InteractionBehaviours.RestoreQuestNotifier)
			{
				eventDispatcher.DispatchEvent(new HudEvents.SuppressQuestNotifier(false));
			}
		}

		protected override void Update()
		{
			if (mascot != null)
			{
				mascot.InteractionBehaviours.Reset();
			}
			Completed();
		}

		protected override void CopyTo(ClubPenguin.Actions.Action _dest)
		{
			EndMascotInteractionAction endMascotInteractionAction = _dest as EndMascotInteractionAction;
			endMascotInteractionAction.Controller = Controller;
			endMascotInteractionAction.ScreenName = ScreenName;
			base.CopyTo(_dest);
		}
	}
}
