namespace DisneyMobile.CoreUnitySystems.FSM
{
	public class StateTraverserStateChangeEvent : BaseEvent
	{
		public State mNewState = null;

		public StateTraverserStateChangeEvent(State newState)
		{
			mNewState = newState;
		}
	}
}
