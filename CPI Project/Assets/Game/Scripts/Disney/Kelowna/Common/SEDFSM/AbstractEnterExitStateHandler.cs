using System.Collections;
using UnityEngine;

namespace Disney.Kelowna.Common.SEDFSM
{
	[RequireComponent(typeof(StateMachineOnExitHelper))]
	public abstract class AbstractEnterExitStateHandler : MonoBehaviour
	{
		protected StateMachine rootStateMachine;

		private bool isInHandledState = false;

		private StateMachineOnExitHelper exitHandler;

		public string HandledState;

		public abstract void OnEnter();

		public abstract void OnExit();

		public IEnumerator Start()
		{
			while (GetComponent<StateMachine>() == null)
			{
				yield return null;
			}
			rootStateMachine = GetComponent<StateMachine>();
			exitHandler = GetComponent<StateMachineOnExitHelper>();
		}

		public void OnStateChanged(string state)
		{
			if (state == HandledState && !isInHandledState)
			{
				isInHandledState = true;
				exitHandler.Push(this);
				OnEnter();
			}
			else if (state != HandledState && isInHandledState)
			{
				isInHandledState = false;
				AbstractEnterExitStateHandler x = exitHandler.Pop();
				if (x == this)
				{
					OnExit();
				}
			}
		}
	}
}
