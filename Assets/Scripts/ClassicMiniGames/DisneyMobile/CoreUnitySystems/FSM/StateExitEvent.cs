namespace DisneyMobile.CoreUnitySystems.FSM
{
	public class StateExitEvent : BaseEvent
	{
		public StateChangeArgs mArgs = null;

		public StateExitEvent(StateChangeArgs args)
		{
			mArgs = args;
		}
	}
}
