namespace DisneyMobile.CoreUnitySystems.FSM
{
	public class StateTraverserTransitionBeganEvent : BaseEvent
	{
		public State mPreviousState = null;

		public State mNewState = null;

		public Signal mSignal = null;

		public StateTraverserTransitionBeganEvent(State previousState, State newState, Signal signal)
		{
			mPreviousState = previousState;
			mNewState = newState;
			mSignal = signal;
		}
	}
}
