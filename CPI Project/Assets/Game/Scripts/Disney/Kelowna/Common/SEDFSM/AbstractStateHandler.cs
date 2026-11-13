using System.Collections;

namespace Disney.Kelowna.Common.SEDFSM
{
	public abstract class AbstractStateHandler : PassiveStateHandler
	{
		public string HandledState;

		protected StateMachine rootStateMachine;

		public bool IsInHandledState
		{
			get;
			private set;
		}

		protected virtual void OnEnter()
		{
		}

		protected virtual void OnExit()
		{
		}

		public IEnumerator Start()
		{
			IsInHandledState = false;
			while (GetComponent<StateMachine>() == null)
			{
				yield return null;
			}
			rootStateMachine = GetComponent<StateMachine>();
		}

		public override void HandleStateChange(string state)
		{
			if (state == HandledState && !IsInHandledState)
			{
				IsInHandledState = true;
				OnEnter();
			}
			else if (state != HandledState && IsInHandledState)
			{
				OnExit();
				IsInHandledState = false;
			}
		}
	}
}
