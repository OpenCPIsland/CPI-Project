using System.Collections;

namespace Disney.Kelowna.Common.SEDFSM
{
	public abstract class AbstractMultiStateHandler : PassiveStateHandler
	{
		public string[] HandledStates;

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
			int num;
			if (!IsInHandledState)
			{
				num = HandledStates.Length;
				int num2 = 0;
				while (true)
				{
					if (num2 < num)
					{
						if (state == HandledStates[num2])
						{
							break;
						}
						num2++;
						continue;
					}
					return;
				}
				IsInHandledState = true;
				OnEnter();
				return;
			}
			bool flag = true;
			num = HandledStates.Length;
			for (int num2 = 0; num2 < num; num2++)
			{
				if (state == HandledStates[num2])
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				OnExit();
				IsInHandledState = false;
			}
		}
	}
}
