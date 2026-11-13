using ClubPenguin.Core;
using Disney.Kelowna.Common.SEDFSM;
using HutongGames.PlayMaker;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("GUI")]
	public class CheckTrayStateAction : FsmStateAction
	{
		public FsmEvent UpEvent;

		public FsmEvent DownEvent;

		public override void OnEnter()
		{
			GameObject gameObject = GameObject.FindWithTag(UIConstants.Tags.UI_Tray_Root);
			if (gameObject != null)
			{
				StateMachine component = gameObject.GetComponent<StateMachine>();
				if (component.CurrentState.Name == "MinNPC")
				{
					base.Fsm.Event(DownEvent);
				}
				else
				{
					base.Fsm.Event(UpEvent);
				}
			}
			Finish();
		}
	}
}
