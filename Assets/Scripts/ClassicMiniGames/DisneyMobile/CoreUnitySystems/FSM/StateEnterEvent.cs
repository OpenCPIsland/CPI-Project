namespace DisneyMobile.CoreUnitySystems.FSM
{
	public class StateEnterEvent : BaseEvent
	{
		public StateChangeArgs mArgs = null;

		public StateEnterEvent(StateChangeArgs args)
		{
			mArgs = args;
		}
	}
}
