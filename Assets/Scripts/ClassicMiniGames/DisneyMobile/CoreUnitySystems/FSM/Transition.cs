using UnityEngine;

namespace DisneyMobile.CoreUnitySystems.FSM
{
	public abstract class Transition : MonoBehaviour, ITransition
	{
		protected EventDispatcher mEventDispatcher = new EventDispatcher();

		public EventDispatcher EventDispatcher
		{
			get
			{
				return mEventDispatcher;
			}
		}

		public abstract void Perform(StateChangeArgs stateChangeDetails);

		public abstract void Reset();

		protected void RaiseTransitionCompletedEvent()
		{
			if (mEventDispatcher != null)
			{
				mEventDispatcher.DispatchEvent(new TransitionCompletedEvent());
			}
		}
	}
}
