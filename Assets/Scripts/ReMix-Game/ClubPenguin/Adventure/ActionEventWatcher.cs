using ClubPenguin.Core;
using ClubPenguin.Net.Domain;
using Disney.LaunchPadFramework;
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

		private HashSet<InputEvents.Actions> actionsSet;

		public override object GetExportParameters()
		{
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
			actionsSet = new HashSet<InputEvents.Actions>(Actions);
			base.dispatcher.AddListener<InputEvents.ActionEvent>(onActionEvent);
		}

		public override void OnDeactivate()
		{
			base.OnDeactivate();
			base.dispatcher.RemoveListener<InputEvents.ActionEvent>(onActionEvent);
		}

		private bool onActionEvent(InputEvents.ActionEvent evt)
		{
			if (actionsSet.Contains(evt.Action))
			{
				taskIncrement();
			}
			return false;
		}
	}
}
