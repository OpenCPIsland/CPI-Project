namespace DisneyMobile.CoreUnitySystems.FSM
{
	public class StatePreExitEvent : BaseEvent
	{
		public StateChangeArgs mArgs = null;

		public StatePreExitEvent(StateChangeArgs args)
		{
			mArgs = args;
		}
	}
}
