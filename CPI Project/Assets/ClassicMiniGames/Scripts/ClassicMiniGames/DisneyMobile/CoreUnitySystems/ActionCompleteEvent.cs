namespace DisneyMobile.CoreUnitySystems
{
	public class ActionCompleteEvent : BaseEvent
	{
		private IAction mAction = null;

		public IAction Action
		{
			get
			{
				return mAction;
			}
		}

		public ActionCompleteEvent(IAction action)
		{
			mAction = action;
		}
	}
}
