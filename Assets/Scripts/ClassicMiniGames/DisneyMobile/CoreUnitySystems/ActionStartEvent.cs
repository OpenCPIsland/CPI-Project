namespace DisneyMobile.CoreUnitySystems
{
	public class ActionStartEvent : BaseEvent
	{
		private IAction mAction = null;

		public IAction Action
		{
			get
			{
				return mAction;
			}
		}

		public ActionStartEvent(IAction action)
		{
			mAction = action;
		}
	}
}
