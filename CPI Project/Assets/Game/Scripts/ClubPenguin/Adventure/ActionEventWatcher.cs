using ClubPenguin.Core;
using ClubPenguin.Locomotion;
using ClubPenguin.Net.Domain;
using ClubPenguin.Props;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[Serializable]
	[CreateAssetMenu(menuName = "Watcher/ActionEvent")]
	public class ActionEventWatcher : TaskWatcher
	{
		[Tooltip("The collection of events to trigger this watcher off of, only those with corresponding network events are supported")]
		public InputEvents.Actions[] Actions;

		private LocomotionEventBroadcaster locomotionBroadcaster;

		private int lastSwimBoostFrame = -1;

		public override object GetExportParameters()
		{
			if (isOfflineSwimBoostActionConfigured())
			{
				return new List<LocomotionAction> { LocomotionAction.Torpedo };
			}

			List<LocomotionAction> list = new List<LocomotionAction>();
			InputEvents.Actions[] actions = Actions;
			foreach (InputEvents.Actions actions2 in actions)
			{
				switch (actions2)
				{
				case InputEvents.Actions.Action1:
					list.Add(LocomotionAction.Action1);
					break;
				case InputEvents.Actions.Action2:
					list.Add(LocomotionAction.Action2);
					break;
				case InputEvents.Actions.Action3:
					list.Add(LocomotionAction.Action3);
					break;
				case InputEvents.Actions.Interact:
					list.Add(LocomotionAction.Interact);
					break;
				case InputEvents.Actions.Jump:
					list.Add(LocomotionAction.Jump);
					break;
				case InputEvents.Actions.Snowball:
					list.Add(LocomotionAction.LaunchThrow);
					break;
				case InputEvents.Actions.Torpedo:
					list.Add(LocomotionAction.Torpedo);
					break;
				case InputEvents.Actions.None:
				case InputEvents.Actions.Cancel:
					Log.LogError(this, string.Concat("Unsupported action ", actions2, " in the ActionEventWatcher"));
					break;
				}
			}
			return list;
		}

		public override string GetWatcherType()
		{
			return "action";
		}

		public override void OnActivate()
		{
			base.OnActivate();
			if (locomotionBroadcaster != null)
			{
				locomotionBroadcaster.OnDoActionEvent -= onLocomotionAction;
			}
			locomotionBroadcaster = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<LocomotionEventBroadcaster>();
			if (locomotionBroadcaster != null)
			{
				locomotionBroadcaster.OnDoActionEvent += onLocomotionAction;
			}
		}

		public override void OnDeactivate()
		{
			base.OnDeactivate();
			if (locomotionBroadcaster != null)
			{
				locomotionBroadcaster.OnDoActionEvent -= onLocomotionAction;
				locomotionBroadcaster = null;
			}
			lastSwimBoostFrame = -1;
		}

		private void onLocomotionAction(LocomotionController.LocomotionAction action, object userData = null)
		{
			if (action == LocomotionController.LocomotionAction.Torpedo && isOfflineSwimBoostActionConfigured())
			{
				if (lastSwimBoostFrame == Time.frameCount)
				{
					return;
				}
				lastSwimBoostFrame = Time.frameCount;
			}
			if (MatchesAction(action) || matchesOfflineDurableGearAction(action))
			{
				taskIncrement();
			}
		}

		private bool matchesOfflineDurableGearAction(LocomotionController.LocomotionAction action)
		{
			if (action != LocomotionController.LocomotionAction.Action3 ||
				!Service.Get<ClubPenguin.GameSettings>().OfflineMode ||
				Actions == null ||
				Array.IndexOf(Actions, InputEvents.Actions.Action2) < 0 ||
				SceneRefs.ZoneLocalPlayerManager == null ||
				SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject == null)
			{
				return false;
			}

			PropUser propUser = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject.GetComponent<PropUser>();
			return propUser != null &&
				propUser.Prop != null &&
				propUser.Prop.PropDef != null &&
				propUser.Prop.PropDef.PropType == PropDefinition.PropTypes.Durable;
		}

		public bool MatchesAction(LocomotionController.LocomotionAction action)
		{
			if (isOfflineSwimBoostActionConfigured())
			{
				return action == LocomotionController.LocomotionAction.Torpedo;
			}

			InputEvents.Actions action2;
			switch (action)
			{
			case LocomotionController.LocomotionAction.Jump:
				action2 = InputEvents.Actions.Jump;
				break;
			case LocomotionController.LocomotionAction.Torpedo:
				action2 = InputEvents.Actions.Torpedo;
				break;
			case LocomotionController.LocomotionAction.Interact:
				action2 = InputEvents.Actions.Interact;
				break;
			case LocomotionController.LocomotionAction.Action1:
				action2 = InputEvents.Actions.Action1;
				break;
			case LocomotionController.LocomotionAction.Action2:
				action2 = InputEvents.Actions.Action2;
				break;
			case LocomotionController.LocomotionAction.Action3:
				action2 = InputEvents.Actions.Action3;
				break;
			case LocomotionController.LocomotionAction.LaunchThrow:
				action2 = InputEvents.Actions.Snowball;
				break;
			default:
				return false;
			}
			return Actions != null && Array.IndexOf(Actions, action2) >= 0;
		}

		private bool isOfflineSwimBoostActionConfigured()
		{
			return Service.Get<ClubPenguin.GameSettings>().OfflineMode && Actions != null && Array.IndexOf(Actions, InputEvents.Actions.Action3) >= 0;
		}
	}
}
