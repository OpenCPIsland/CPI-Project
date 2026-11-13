namespace Disney.LaunchPadFramework
{
	public struct ActionStartEvent
	{
		private IAction mAction;

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
