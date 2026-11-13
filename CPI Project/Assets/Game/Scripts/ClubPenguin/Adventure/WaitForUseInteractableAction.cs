using ClubPenguin.Core;
using ClubPenguin.Props;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("Interactables")]
	public class WaitForUseInteractableAction : FsmStateAction
	{
		[RequiredField]
		public PropDefinition propDefinition;

		public bool anyAction = true;

		public InputEvents.Actions action;

		public FsmEvent UseEvent;

		public FsmEvent CancelEvent;

		private EventDispatcher dispatcher;

		public override void OnEnter()
		{
			dispatcher = Service.Get<EventDispatcher>();
			dispatcher.AddListener<InputEvents.ActionEvent>(onActionEvent);
		}

		public override void OnExit()
		{
			dispatcher.RemoveListener<InputEvents.ActionEvent>(onActionEvent);
		}

		private bool onActionEvent(InputEvents.ActionEvent evt)
		{
			if (getCurrentPropName() == propDefinition.GetNameOnServer())
			{
				if (evt.Action == InputEvents.Actions.Cancel)
				{
					base.Fsm.Event(CancelEvent);
				}
				else if ((anyAction && (evt.Action == InputEvents.Actions.Action1 || evt.Action == InputEvents.Actions.Action2 || evt.Action == InputEvents.Actions.Action3)) || (!anyAction && action == evt.Action))
				{
					base.Fsm.Event(UseEvent);
				}
			}
			return false;
		}

		private string getCurrentPropName()
		{
			GameObject localPlayerGameObject = SceneRefs.ZoneLocalPlayerManager.LocalPlayerGameObject;
			PropUser component = localPlayerGameObject.GetComponent<PropUser>();
			if (component != null && component.Prop != null)
			{
				return component.Prop.PropId;
			}
			return "";
		}
	}
}
