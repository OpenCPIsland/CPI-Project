namespace DisneyMobile.CoreUnitySystems.FSM
{
	public class StatePreEnterEvent : BaseEvent
	{
		public StateChangeArgs mArgs = null;

		public StatePreEnterEvent(StateChangeArgs args)
		{
			mArgs = args;
		}
	}
}
