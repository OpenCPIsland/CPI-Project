using Disney.Kelowna.Common;
using HutongGames.PlayMaker;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Adventure
{
	[ActionCategory("GUI")]
	public class PressAnywhereAction : FsmStateAction
	{
		public enum ETouchType
		{
			DOWN,
			UP
		}

		public ETouchType TouchType = ETouchType.UP;

		public FsmEvent FinishEvent;

		public override void OnEnter()
		{
			CoroutineRunner.Start(waitForPress(), this, "");
		}

		public IEnumerator waitForPress()
		{
			if (TouchType == ETouchType.DOWN)
			{
				while (!hasPressedDown())
				{
					yield return null;
				}
			}
			else if (TouchType == ETouchType.UP)
			{
				while (!hasPressedUp())
				{
					yield return null;
				}
			}
			yield return null;
			if (FinishEvent != null)
			{
				base.Fsm.Event(FinishEvent);
			}
			else
			{
				Finish();
			}
		}

		private bool hasPressedDown()
		{
			bool flag = InputWrapper.GetMouseButtonDown(0);
			if (UnityEngine.Input.touchSupported && !flag)
			{
				flag = (InputWrapper.touchCount > 0 && InputWrapper.GetTouch(0).phase == TouchPhase.Began);
			}
			return flag;
		}

		private bool hasPressedUp()
		{
			bool flag = InputWrapper.GetMouseButtonUp(0);
			if (UnityEngine.Input.touchSupported && !flag)
			{
				flag = (InputWrapper.touchCount > 0 && InputWrapper.GetTouch(0).phase == TouchPhase.Ended);
			}
			return flag;
		}
	}
}
