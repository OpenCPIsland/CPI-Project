namespace DisneyMobile.CoreUnitySystems.FSM
{
	public class StatePostEnterEvent : BaseEvent
	{
		public StateChangeArgs mArgs = null;

		public StatePostEnterEvent(StateChangeArgs args)
		{
			mArgs = args;
		}
	}
}
