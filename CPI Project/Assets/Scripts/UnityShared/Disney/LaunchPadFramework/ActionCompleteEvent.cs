namespace Disney.LaunchPadFramework
{
	public struct ActionCompleteEvent
	{
		private IAction mAction;

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
