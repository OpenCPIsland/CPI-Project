namespace DisneyMobile.CoreUnitySystems.FSM
{
	public class StatePostExitEvent : BaseEvent
	{
		public StateChangeArgs mArgs = null;

		public StatePostExitEvent(StateChangeArgs args)
		{
			mArgs = args;
		}
	}
}
