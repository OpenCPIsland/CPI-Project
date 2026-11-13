using UnityEngine;

namespace Disney.Kelowna.Common.SEDFSM
{
	public class StateMachineOnExitHelper : MonoBehaviour
	{
		private AbstractEnterExitStateHandler activeStateHandler;

		public void Push(AbstractEnterExitStateHandler stateHandler)
		{
			if (activeStateHandler != null)
			{
				Pop().OnExit();
			}
			activeStateHandler = stateHandler;
		}

		public AbstractEnterExitStateHandler Pop()
		{
			AbstractEnterExitStateHandler result = activeStateHandler;
			activeStateHandler = null;
			return result;
		}
	}
}
