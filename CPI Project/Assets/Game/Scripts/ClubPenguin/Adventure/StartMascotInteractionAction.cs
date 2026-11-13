using ClubPenguin.Actions;
using ClubPenguin.Cinematography;
using ClubPenguin.Core;
using ClubPenguin.UI;
using Disney.Kelowna.Common.SEDFSM;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	public class StartMascotInteractionAction : ClubPenguin.Actions.Action
	{
		public CameraController Controller;

		public bool TrayClosed;

		public bool MainNavDisabled;

		protected override void OnEnable()
		{
			if (!Owner.CompareTag("Player"))
			{
				return;
			}
			EventDispatcher eventDispatcher = Service.Get<EventDispatcher>();
			MascotController componentInParent = SceneRefs.ActionSequencer.GetTrigger(Owner).GetComponentInParent<MascotController>();
			Mascot mascot = componentInParent.Mascot;
			if (mascot.InteractionBehaviours.ZoomIn && Controller != null)
			{
				CinematographyEvents.CameraLogicChangeEvent evt = default(CinematographyEvents.CameraLogicChangeEvent);
				evt.Controller = Controller;
				eventDispatcher.DispatchEvent(evt);
			}
			if (mascot.InteractionBehaviours.LowerTray && TrayClosed)
			{
				GameObject gameObject = GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root);
				if (gameObject != null)
				{
					StateMachineContext component = gameObject.GetComponent<StateMachineContext>();
					component.SendEvent(new ExternalEvent("Root", "minnpc"));
				}
			}
			if (MainNavDisabled)
			{
				eventDispatcher.DispatchEvent(new PlayerCardEvents.SetEnablePlayerCard(false));
				eventDispatcher.DispatchEvent(new UIDisablerEvents.DisableUIElementGroup("MainNavButtons"));
			}
			if (mascot.InteractionBehaviours.SuppressQuestNotifier)
			{
				eventDispatcher.DispatchEvent(new HudEvents.SuppressQuestNotifier(true, true));
			}
			if (mascot.InteractionBehaviours.MoveToTalkSpot)
			{
				LocomoteToAction component2 = Owner.GetComponent<LocomoteToAction>();
				if (component2 != null)
				{
					component2.IsEnabled = true;
					if (mascot.InteractionBehaviours.OverrideInteracteeTxform)
					{
						component2.Waypoints = new List<Transform>(1);
						component2.IsEnabled = true;
						Transform transform = new GameObject().transform;
						transform.position = mascot.InteractionBehaviours.DesiredInteracteeTxform.position;
						transform.rotation = mascot.InteractionBehaviours.DesiredInteracteeTxform.rotation;
						component2.Waypoints.Add(transform);
					}
				}
			}
			else
			{
				LocomoteToAction component2 = Owner.GetComponent<LocomoteToAction>();
				if (component2 != null)
				{
					component2.IsEnabled = false;
				}
			}
		}

		protected override void Update()
		{
			Completed();
		}

		protected override void CopyTo(ClubPenguin.Actions.Action _dest)
		{
			StartMascotInteractionAction startMascotInteractionAction = _dest as StartMascotInteractionAction;
			startMascotInteractionAction.Controller = Controller;
			startMascotInteractionAction.TrayClosed = TrayClosed;
			startMascotInteractionAction.MainNavDisabled = MainNavDisabled;
			base.CopyTo(_dest);
		}
	}
}
