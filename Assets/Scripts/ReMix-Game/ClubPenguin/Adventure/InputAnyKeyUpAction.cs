using ClubPenguin.Input;
using HutongGames.PlayMaker;
using System;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("GUI")]
	public class InputAnyKeyUpAction : FsmStateAction
	{
		public FsmEvent FinishEvent;

		private AnyKeyInputActionHandler inputHandler;

		public override void OnEnter()
		{
			inputHandler = base.Owner.AddComponent<AnyKeyInputActionHandler>();
			AnyKeyInputActionHandler anyKeyInputActionHandler = inputHandler;
			anyKeyInputActionHandler.OnInputHandled = (Action)Delegate.Combine(anyKeyInputActionHandler.OnInputHandled, new Action(onInputHandled));
		}

		public override void OnExit()
		{
			AnyKeyInputActionHandler anyKeyInputActionHandler = inputHandler;
			anyKeyInputActionHandler.OnInputHandled = (Action)Delegate.Remove(anyKeyInputActionHandler.OnInputHandled, new Action(onInputHandled));
			UnityEngine.Object.Destroy(inputHandler);
		}

		private void onInputHandled()
		{
			if (FinishEvent != null)
			{
				base.Fsm.Event(FinishEvent);
			}
			else
			{
				Finish();
			}
		}
	}
}
