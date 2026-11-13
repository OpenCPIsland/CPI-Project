namespace DisneyMobile.CoreUnitySystems.FSM
{
	public class StateTraverserTransitionEndedEvent : BaseEvent
	{
		public State mPreviousState = null;

		public State mNewState = null;

		public Signal mSignal = null;

		public StateTraverserTransitionEndedEvent(State previousState, State newState, Signal signal)
		{
			mPreviousState = previousState;
			mNewState = newState;
			mSignal = signal;
		}
	}
}
